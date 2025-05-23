[<RequireQualifiedAccess>]
module Persistence.Storage

open Persistence.Storages
open Persistence.Storages.Domain

type Provider =
    | FileSystem of FileSystem.Client
    | InMemory of InMemory.Client
    | Database of Database.Client
    | Configuration of Configuration.Client

type Connection =
    | FileSystem of FileSystem.Connection
    | InMemory of InMemory.Connection
    | Database of Database.Connection
    | Configuration of Configuration.Connection

let init connection =
    match connection with
    | Connection.InMemory c -> c |> InMemory.Provider.init |> Result.map Provider.InMemory
    | Connection.FileSystem c -> c |> FileSystem.Provider.init |> Result.map Provider.FileSystem
    | Connection.Database c -> c |> Database.Provider.init |> Result.map Provider.Database
    | Connection.Configuration c -> c |> Configuration.Provider.init |> Result.map Provider.Configuration
