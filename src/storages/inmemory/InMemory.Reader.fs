[<RequireQualifiedAccess>]
module Persistence.InMemory.Read

open Infrastructure.Domain
open Persistence.Domain.InMemory

let string key (storage: Client) =
    try
        match storage.TryGetValue(key) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
