[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Write

open System
open Npgsql
open Dapper
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.Postgre

let execute (sql: string) (client: Client) =
    async {
        try
            let! rowsAffected = 
                client.Connection.ExecuteAsync(sql)
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

let executeWithParams (sql: string) (parameters: obj) (client: Client) =
    async {
        try
            let! rowsAffected = 
                client.Connection.ExecuteAsync(sql, parameters)
                |> Async.AwaitTask
            
            return Ok rowsAffected
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute command with params: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let transaction (commands: (string * obj) list) (client: Client) =
    async {
        use transaction = client.Connection.BeginTransaction()
        try
            let mutable totalRowsAffected = 0
            
            for (sql, parameters) in commands do
                let! rowsAffected = 
                    client.Connection.ExecuteAsync(sql, parameters, transaction)
                    |> Async.AwaitTask
                
                totalRowsAffected <- totalRowsAffected + rowsAffected
            
            do! transaction.CommitAsync() |> Async.AwaitTask
            return Ok totalRowsAffected
        with ex ->
            do! transaction.RollbackAsync() |> Async.AwaitTask
            return
                Error
                <| Operation {
                    Message = $"Failed to execute transaction: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
