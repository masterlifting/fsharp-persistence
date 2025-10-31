[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Query

open Dapper
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.Postgre

let get<'a> (request: Request) (client: Client) =
    async {
        try
            let! results =
                client.Connection.QueryAsync<'a>(request.Sql, request.Params |> Option.toObj)
                |> Async.AwaitTask
            return Ok(results |> Seq.toArray)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
