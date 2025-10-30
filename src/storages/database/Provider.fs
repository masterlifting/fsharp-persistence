[<RequireQualifiedAccess>]
module Persistence.Storages.Database.Provider

open Infrastructure.Domain
open Persistence.Storages
open Persistence.Storages.Domain

let init (connection: Database.Connection) =
    match connection.Database with
    | Database.Postgre connectionString ->
        ({
            String = connectionString
            Lifetime = connection.Lifetime
        }
        : Postgre.Connection)
        |> Postgre.Provider.init
        |> Result.map Database.Client.Postgre
    | Database.MongoDb _ -> "MongoDb client" |> NotSupported |> Error
    | Database.Redis _ -> "Redis client" |> NotSupported |> Error

let dispose (client: Database.Client) =
    match client with
    | Database.Client.Postgre client -> client |> Postgre.Provider.dispose
