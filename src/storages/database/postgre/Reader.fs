[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Read

open System
open System.Data
open Npgsql
open Dapper
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.Postgre

let private executeReader (sql: string) (parameters: obj) (client: Client) =
    async {
        try
            let! results = 
                client.Connection.QueryAsync<obj>(sql, parameters)
                |> Async.AwaitTask
            
            return Ok (results |> Seq.toList)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let query (sql: string) (client: Client) =
    async {
        try
            let! results = 
                client.Connection.QueryAsync(sql)
                |> Async.AwaitTask
            
            return Ok (results |> Seq.toList)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let queryWithParams (sql: string) (parameters: obj) (client: Client) =
    executeReader sql parameters client

let queryTyped<'T> (sql: string) (client: Client) =
    async {
        try
            let! results = 
                client.Connection.QueryAsync<'T>(sql)
                |> Async.AwaitTask
            
            return Ok (results |> Seq.toArray)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute typed query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let queryTypedWithParams<'T> (sql: string) (parameters: obj) (client: Client) =
    async {
        try
            let! results = 
                client.Connection.QueryAsync<'T>(sql, parameters)
                |> Async.AwaitTask
            
            return Ok (results |> Seq.toArray)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute typed query with params: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let scalar (sql: string) (client: Client) =
    async {
        try
            let! result = 
                client.Connection.ExecuteScalarAsync(sql)
                |> Async.AwaitTask
            
            return Ok (if isNull result then None else Some result)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute scalar query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let scalarWithParams (sql: string) (parameters: obj) (client: Client) =
    async {
        try
            let! result = 
                client.Connection.ExecuteScalarAsync(sql, parameters)
                |> Async.AwaitTask
            
            return Ok (if isNull result then None else Some result)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute scalar query with params: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let scalarTyped<'T> (sql: string) (client: Client) =
    async {
        try
            let! result = 
                client.Connection.ExecuteScalarAsync<'T>(sql)
                |> Async.AwaitTask
            
            return Ok (if box result |> isNull then None else Some result)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute scalar typed query: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }

let scalarTypedWithParams<'T> (sql: string) (parameters: obj) (client: Client) =
    async {
        try
            let! result = 
                client.Connection.ExecuteScalarAsync<'T>(sql, parameters)
                |> Async.AwaitTask
            
            return Ok (if box result |> isNull then None else Some result)
        with ex ->
            return
                Error
                <| Operation {
                    Message = $"Failed to execute scalar typed query with params: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
