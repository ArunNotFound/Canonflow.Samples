#r "nuget: Npgsql, 8.0.3"

open Npgsql
open System.IO

let connStr = "Host=localhost;Database=fintech;Username=postgres;Password=password"
use conn = new NpgsqlConnection(connStr)
conn.Open()

let runScript path =
    printfn "Running %s" path
    let sql = File.ReadAllText(path)
    use cmd = new NpgsqlCommand(sql, conn)
    cmd.ExecuteNonQuery() |> ignore

runScript "/root/repos/github/fsharp/Canonflow.Samples/sangam-credit/sangam/db/init/01-schema.sql"
runScript "/root/repos/github/fsharp/Canonflow.Samples/sangam-credit/sangam/db/init/02-seed.sql"
printfn "Done."
