[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Command

open Dapper
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.Postgre

let execute (request: Request) (client: Client) =
    async {
        try
            let! rowsAffected =
                client.Connection.ExecuteAsync(request.Sql, request.Params |> Option.toObj)
                |> Async.AwaitTask
            return Ok rowsAffected
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute command: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
