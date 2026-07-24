namespace WecarCore.Domain

open System
open System.Text.RegularExpressions

// FsAssay Pattern: Primitive Obsession Prevention via Smart Constructors

module ValueObjects =

    type UsernameError = 
        | TooShort
        | TooLong
        | InvalidFormat

    type Username = private Username of string
    module Username =
        let value (Username u) = u
        let create (u: string) =
            let regex = Regex(@"^[a-zA-Z0-9_]+$")
            if isNull u then Error TooShort
            elif u.Length < 3 then Error TooShort
            elif u.Length > 50 then Error TooLong
            elif not (regex.IsMatch(u)) then Error InvalidFormat
            else Ok (Username u)

    type PhoneError = 
        | TooShort
        | TooLong
        
    type PhoneNumber = private PhoneNumber of string
    module PhoneNumber =
        let value (PhoneNumber p) = p
        let create (p: string) =
            if isNull p then Error TooShort
            elif p.Length < 10 then Error TooShort
            elif p.Length > 15 then Error TooLong
            else Ok (PhoneNumber p)

    type MessageContentError =
        | Empty
        | TooLong

    type MessageContent = private MessageContent of string
    module MessageContent =
        let value (MessageContent c) = c
        let create (c: string) =
            if isNull c || c.Length = 0 then Error Empty
            elif c.Length > 4000 then Error TooLong
            else Ok (MessageContent c)

module DomainModel =
    open ValueObjects

    type UserId = UserId of Guid
    type GroupId = GroupId of Guid
    type MessageId = MessageId of Guid

    type UserStatus =
        | Active
        | Inactive
        | Banned
        | Deleted

    type User = {
        Id: UserId
        Username: Username
        DisplayName: string
        PhoneNumber: PhoneNumber
        Status: UserStatus
    }

    type MessageType =
        | Text
        | Image
        | Video
        | Audio
        | File

    type MessageTarget =
        | DirectMessage of UserId
        | GroupMessage of GroupId

    type Message = {
        Id: MessageId
        SenderId: UserId
        Target: MessageTarget
        Content: MessageContent
        Type: MessageType
        SentAt: DateTimeOffset
        ReadAt: DateTimeOffset option
    }

    type MessageError =
        | ReadBeforeSent

module ChatBehavior =
    open DomainModel

    let markRead (msg: Message) (readTime: DateTimeOffset) =
        if readTime < msg.SentAt then
            Error MessageError.ReadBeforeSent
        else
            Ok { msg with ReadAt = Some readTime }
