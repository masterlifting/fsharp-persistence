[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Read

open Dapper
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.Postgre

let query<'a> (request: Query.Request<'a>) (client: Client) =
    async {
        try
            let! results = client.QueryAsync<'a>(request.Sql, request.Params) |> Async.AwaitTask
            return Ok(results |> Seq.toArray)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let scalar<'a> (request: Query.Request<'a>) (client: Client) =
    async {
        try
            let! result = client.ExecuteScalarAsync<'a>(request.Sql, request.Params) |> Async.AwaitTask
            match result with
            | null ->
                return
                    Error
                    <| Operation {
                        Message = "Scalar query returned null"
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
            | value -> return Ok value
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute scalar query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
