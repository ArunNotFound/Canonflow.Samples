namespace FintechWallet.Api

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open FintechWallet.Domain
open System.Text.Json
open System.Text.Json.Serialization
open FSharp.SystemTextJson

module Handlers =
    // In-memory store for demonstration
    let wallets = System.Collections.Concurrent.ConcurrentDictionary<Guid, Wallet>()
    let balances = System.Collections.Concurrent.ConcurrentDictionary<Guid, WalletBalance>()
    let transactions = System.Collections.Concurrent.ConcurrentDictionary<Guid, LedgerTransaction>()

    [<CLIMutable>]
    type CreateWalletRequest = {
        CustomerId: Guid
        Currency: string
    }

    [<CLIMutable>]
    type CreateTransactionRequest = {
        Amount: decimal
        ReferenceId: string
    }

    let createWalletHandler =
        fun (next: HttpFunc) (ctx: Microsoft.AspNetCore.Http.HttpContext) ->
            task {
                let! req = ctx.BindJsonAsync<CreateWalletRequest>()
                let walletId = Guid.NewGuid()
                
                let wallet = {
                    WalletId = walletId
                    CustomerId = req.CustomerId
                    Currency = req.Currency
                    Status = "ACTIVE"
                    CreatedAt = DateTimeOffset.UtcNow
                }
                
                let balance = {
                    WalletId = walletId
                    AvailableBalance = 0m
                    LockedBalance = 0m
                    UpdatedAt = DateTimeOffset.UtcNow
                }
                
                wallets.TryAdd(walletId, wallet) |> ignore
                balances.TryAdd(walletId, balance) |> ignore
                
                return! json wallet next ctx
            }

    let getWalletBalanceHandler (walletId: Guid) =
        fun (next: HttpFunc) (ctx: Microsoft.AspNetCore.Http.HttpContext) ->
            match balances.TryGetValue(walletId) with
            | true, b -> json b next ctx
            | false, _ -> RequestErrors.NOT_FOUND "Wallet not found" next ctx

    let creditWalletHandler (walletId: Guid) =
        fun (next: HttpFunc) (ctx: Microsoft.AspNetCore.Http.HttpContext) ->
            task {
                let! req = ctx.BindJsonAsync<CreateTransactionRequest>()
                match wallets.TryGetValue(walletId) with
                | true, w ->
                    let tx = WalletRules.createCredit w req.Amount req.ReferenceId
                    
                    // Idempotency check omitted for brevity in demo, assuming unique RefId
                    transactions.TryAdd(tx.TransactionId, tx) |> ignore
                    
                    // Update balance
                    let mutable currentBalance = balances.[walletId]
                    let newBalance = { currentBalance with 
                                        AvailableBalance = currentBalance.AvailableBalance + tx.Amount
                                        UpdatedAt = DateTimeOffset.UtcNow }
                    balances.[walletId] <- newBalance
                    
                    return! json tx next ctx
                | false, _ -> 
                    return! RequestErrors.NOT_FOUND "Wallet not found" next ctx
            }

    let debitWalletHandler (walletId: Guid) =
        fun (next: HttpFunc) (ctx: Microsoft.AspNetCore.Http.HttpContext) ->
            task {
                let! req = ctx.BindJsonAsync<CreateTransactionRequest>()
                match wallets.TryGetValue(walletId), balances.TryGetValue(walletId) with
                | (true, w), (true, b) ->
                    if WalletRules.canDebit b req.Amount then
                        let tx = WalletRules.createDebit w req.Amount req.ReferenceId
                        transactions.TryAdd(tx.TransactionId, tx) |> ignore
                        
                        let newBalance = { b with 
                                            AvailableBalance = b.AvailableBalance - tx.Amount
                                            UpdatedAt = DateTimeOffset.UtcNow }
                        balances.[walletId] <- newBalance
                        
                        return! json tx next ctx
                    else
                        return! RequestErrors.BAD_REQUEST "Insufficient funds" next ctx
                | _ -> 
                    return! RequestErrors.NOT_FOUND "Wallet not found" next ctx
            }

    let webApp =
        choose [
            POST >=> route "/api/wallets" >=> createWalletHandler
            GET >=> routef "/api/wallets/%O/balance" getWalletBalanceHandler
            POST >=> routef "/api/wallets/%O/credit" creditWalletHandler
            POST >=> routef "/api/wallets/%O/debit" debitWalletHandler
        ]

module Program =
    let configureApp (app : IApplicationBuilder) =
        app.UseCors(fun b -> b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() |> ignore)
           .UseGiraffe Handlers.webApp

    let configureServices (services : IServiceCollection) =
        services.AddCors() |> ignore
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(fun webHost ->
                webHost.Configure(configureApp)
                       .ConfigureServices(configureServices)
                       .UseUrls("http://localhost:5000")
                       |> ignore)
            .Build()
            .Run()
        0
