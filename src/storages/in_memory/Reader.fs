[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Read

open Infrastructure.Domain
open Persistence.Storages.Domain.InMemory

let string (client: Client) =
    try
        match client.Storage.TryGetValue(client.TableName) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error
        <| Operation {
            Message = ex.Message
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }
