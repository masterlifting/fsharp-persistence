[<RequireQualifiedAccess>]
module Persistence.InMemory.Storage

open System
open Infrastructure.Domain

let internal Storage = Client(StringComparer.OrdinalIgnoreCase)

let init () =
    try
        Ok <| Storage
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
