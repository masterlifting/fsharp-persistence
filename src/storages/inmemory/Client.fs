[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Client

open System
open Infrastructure.Domain
open Persistence.Storages.Domain.InMemory

let internal Storage = Client(StringComparer.OrdinalIgnoreCase)

let init () =
    try
        Ok <| Storage
    with ex ->
        Error
        <| Operation {
            Message = ex.Message
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }
