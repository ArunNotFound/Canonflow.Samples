module Tests
open Xunit
open FsCheck
open FsCheck.Xunit
open CanonFlow.FsCheck.Generators
[<Property>]
let ``Dummy property`` () = true
