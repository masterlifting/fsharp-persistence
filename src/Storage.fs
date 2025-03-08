[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure

type Provider =
    | FileSystem of FileSystem.Domain.Client
    | InMemory of InMemory.Domain.Client
    | Database of Database.Domain.Client
    | Configuration of Configuration.Domain.Client

type Connection =
    | FileSystem of FileSystem.Domain.Connection
    | InMemory
    | Database of Database.Domain.Connection
    | Configuration of Configuration.Domain.Connection

let init connection =
    match connection with
    | Connection.InMemory -> InMemory.Storage.init () |> Result.map Provider.InMemory
    | Connection.FileSystem value -> value |> FileSystem.Storage.init |> Result.map Provider.FileSystem
    | Connection.Database value -> value |> Database.Storage.init |> Result.map Provider.Database
    | Connection.Configuration value -> value |> Configuration.Storage.init |> Result.map Provider.Configuration
