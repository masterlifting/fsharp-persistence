[<RequireQualifiedAccess>]
module Persistence.Storages.Database.Provider

open Infrastructure.Domain
open Persistence.Storages
open Persistence.Storages.Domain

let init (connection: Database.Connection) =
    match connection.Database with
    | Database.Postgre connectionString ->
        let dbType = 
            match connection.Type with
            | Database.Singleton -> Postgre.Singleton
            | Database.Transient -> Postgre.Transient

        { Postgre.String = connectionString; Postgre.Type = dbType }
        |> Postgre.Provider.init
    | Database.MongoDb _ ->
        "MongoDb client" |> NotSupported |> Error
    | Database.Redis _ ->
        "Redis client" |> NotSupported |> Error
