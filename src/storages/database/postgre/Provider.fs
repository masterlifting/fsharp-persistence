[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Provider

open Npgsql
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Domain
open Persistence.Storages.Domain.Postgre
open System.Collections.Concurrent
open System.Data
open System

let private clients = ConcurrentDictionary<string, NpgsqlConnection>()

type private SqlDateOnlyTypeHandler() =
    inherit Dapper.SqlMapper.TypeHandler<DateOnly>()

    override _.SetValue(parameter: IDbDataParameter, date: DateOnly) =
        parameter.Value <- date.ToDateTime(TimeOnly(0, 0))

    override _.Parse(value: obj) =
        DateOnly.FromDateTime(value :?> DateTime)

type private SqlTimeOnlyTypeHandler() =
    inherit Dapper.SqlMapper.TypeHandler<TimeOnly>()

    override _.SetValue(parameter: IDbDataParameter, time: TimeOnly) = parameter.Value <- time.ToString()

    override _.Parse(value: obj) =
        TimeOnly.FromTimeSpan(value :?> TimeSpan)

Dapper.SqlMapper.AddTypeHandler(SqlDateOnlyTypeHandler())
Dapper.SqlMapper.AddTypeHandler(SqlTimeOnlyTypeHandler())

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

let clone (client: Client) =
    let connection = client.Connection.ConnectionString |> client.Connection.CloneWith
    {
        Connection = connection
        Lifetime = Transient
    }

let dispose (client: Client) =
    try
        match client.Lifetime with
        | Transient ->
            match client.Connection.State with
            | ConnectionState.Open ->
                client.Connection.Close()
                client.Connection.Dispose()
            | _ -> ()
        | Singleton -> ()
    with _ ->
        ()
