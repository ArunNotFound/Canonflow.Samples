module DomainTests

open Xunit
open FsCheck
open FsCheck.Xunit
open WecarCore.Domain
open WecarCore.Domain.ValueObjects
open WecarCore.Domain.DomainModel
open WecarCore.Domain.ChatBehavior
open System
open System.Text.RegularExpressions

[<Property>]
let ``Username FsAssay creation blocks invalid format and lengths`` (usernameStr: string) =
    let res = Username.create usernameStr
    let regex = Regex(@"^[a-zA-Z0-9_]+$")
    if isNull usernameStr || usernameStr.Length < 3 then
        res = Error UsernameError.TooShort
    elif usernameStr.Length > 50 then
        res = Error UsernameError.TooLong
    elif not (regex.IsMatch(usernameStr)) then
        res = Error UsernameError.InvalidFormat
    else
        match res with
        | Ok u -> Username.value u = usernameStr
        | _ -> false

[<Property>]
let ``MessageContent FsAssay bounds content length`` (contentStr: string) =
    let res = MessageContent.create contentStr
    if isNull contentStr || contentStr.Length = 0 then
        res = Error MessageContentError.Empty
    elif contentStr.Length > 4000 then
        res = Error MessageContentError.TooLong
    else
        match res with
        | Ok c -> MessageContent.value c = contentStr
        | _ -> false

[<Property>]
let ``Message cannot be read before it is sent`` (readTime: DateTimeOffset) =
    let dummyContent = MessageContent.create "Hello!" |> function | Ok c -> c | _ -> failwith "Bad setup"
    let sentTime = DateTimeOffset.Now
    let msg = {
        Id = MessageId(Guid.NewGuid())
        SenderId = UserId(Guid.NewGuid())
        Target = DirectMessage (UserId(Guid.NewGuid()))
        Content = dummyContent
        Type = Text
        SentAt = sentTime
        ReadAt = None
    }

    if readTime < sentTime then
        match markRead msg readTime with
        | Error MessageError.ReadBeforeSent -> true
        | _ -> false
    else
        match markRead msg readTime with
        | Ok markedMsg -> markedMsg.ReadAt = Some readTime
        | _ -> false
