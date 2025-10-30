[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Provider

open Npgsql
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Domain
open Persistence.Storages.Domain.Postgre
open System.Collections.Concurrent
open System.Data

let private clients = ConcurrentDictionary<string, NpgsqlConnection>()

let init connection =
    try
        match connection.Lifetime with
        | Singleton ->
            match clients.TryGetValue connection.String with
            | true, conn ->
                match conn.State with
                | ConnectionState.Open ->
                    Ok {
                        Connection = conn
                        Lifetime = Singleton
                    }
                | _ ->
                    conn.Open()
                    Ok {
                        Connection = conn
                        Lifetime = Singleton
                    }
            | false, _ ->
                let conn = new NpgsqlConnection(connection.String)
                conn.Open()
                clients.TryAdd(connection.String, conn) |> ignore
                Ok {
                    Connection = conn
                    Lifetime = Singleton
                }
        | Transient ->
            let conn = new NpgsqlConnection(connection.String)
            conn.Open()
            Ok {
                Connection = conn
                Lifetime = Transient
            }
    with ex ->
        Error
        <| Operation {
            Message = $"Failed to initialize PostgreSQL connection: {ex |> Exception.toMessage}"
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }

let dispose (client: Client) =
    try
        match client.Lifetime with
        | Transient ->
            client.Connection.Close()
            client.Connection.Dispose()
        | Singleton -> client.Connection.Close()
    with _ ->
        ()
