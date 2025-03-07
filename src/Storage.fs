[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure

type Type =
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
    | Connection.InMemory -> InMemory.Storage.init () |> Result.map Type.InMemory
    | Connection.FileSystem value -> value |> FileSystem.Storage.init |> Result.map Type.FileSystem
    | Connection.Database value -> value |> Database.Storage.init |> Result.map Type.Database
    | Connection.Configuration value -> value |> Configuration.Storage.init |> Result.map Type.Configuration
