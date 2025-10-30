[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Provider

open Npgsql
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Domain
open Persistence.Storages.Domain.Postgre
open System.Collections.Concurrent
open System.Data

let private clients = ConcurrentDictionary<string, Client>()

let init connection =
    try
        match connection.Lifetime with
        | Singleton ->
            match clients.TryGetValue connection.String with
            | true, client ->
                match client.State with
                | ConnectionState.Open -> Ok client
                | _ ->
                    client.Close()
                    client.Open()
                    Ok client
            | false, _ ->
                let client = new NpgsqlConnection(connection.String)
                client.Open()
                clients.TryAdd(connection.String, client) |> ignore
                Ok client
        | Transient ->
            let client = new NpgsqlConnection(connection.String)
            client.Open()
            Ok client
    with ex ->
        Error
        <| Operation {
            Message = $"Failed to initialize PostgreSQL connection: {ex |> Exception.toMessage}"
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }

let dispose (client: Client) =
    try
        client.Close()
        client.Dispose()
        clients.TryRemove client.ConnectionString |> ignore
    with _ ->
        ()
