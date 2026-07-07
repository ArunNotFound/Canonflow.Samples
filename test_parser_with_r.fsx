#r "nuget: FParsec"
open System
open FParsec

type Bound =
    | InSet of string list
    | Opaque of string

let ws = spaces

let pIdentifier = 
    pipe2 (asciiLetter <|> pchar '_') (manyChars (asciiLetter <|> digit <|> pchar '_')) (fun c s -> string c + s)

let pCast = opt (pstring "::" >>. manyChars (asciiLetter <|> pchar ' ') .>> opt (pstring "[]")) |>> ignore

let pField, pFieldRef = createParserForwardedToRef()
pFieldRef.Value <- 
    (attempt (between (pchar '(' >>. ws) (ws .>> pchar ')') pField) <|> pIdentifier) .>> pCast

let pStringLiteral = 
    between (pstring "'") (pstring "'") (manyChars (noneOf "'"))

let pAnyArray = 
    pstring "=" >>. ws >>. pstring "ANY" >>. ws >>. 
    between (pchar '(' >>. ws) (ws .>> pchar ')') (
        between (pchar '(' >>. ws) (ws .>> pchar ')') (
            pstring "ARRAY[" >>. ws >>. sepBy (pStringLiteral .>> pCast) (ws .>> pstring "," .>> ws) .>> ws .>> pstring "]"
        ) .>> pCast
    ) |>> InSet

let pCondition = 
    pipe2 (pField .>> ws) pAnyArray (fun ident inset -> inset)

let test () =
    let input = "(direction)::text = ANY ((ARRAY['CREDIT'::character varying, 'DEBIT'::character varying])::text[])"
    match run (ws >>. pCondition .>> eof) input with
    | Success(result, _, _) -> printfn "Success: %A" result
    | Failure(err, _, _) -> printfn "Failure: %s" err

test ()
