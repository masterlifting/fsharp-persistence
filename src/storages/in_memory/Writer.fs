[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Write

open Infrastructure.Domain
open Persistence.Storages.Domain.InMemory

let string value (client: Client) =
    try
        client.Storage.AddOrUpdate(client.TableName, value, (fun _ _ -> value)) |> ignore |> Ok
    with ex ->
        Error
        <| Operation {
            Message = ex.Message
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }
