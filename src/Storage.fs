[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Persistence.Storages
open Persistence.Storages.Domain

type Provider =
    | FileSystem of FileSystem.Client
    | InMemory of InMemory.Client
    | Database of Database.Client
    | Configuration of Configuration.Client

type Connection =
    | FileSystem of FileSystem.Connection
    | InMemory
    | Database of Database.Connection
    | Configuration of Configuration.Connection

let init connection =
    match connection with
    | Connection.InMemory -> InMemory.Client.init () |> Result.map Provider.InMemory
    | Connection.FileSystem value -> value |> FileSystem.Client.init |> Result.map Provider.FileSystem
    | Connection.Database value -> value |> Database.Client.init |> Result.map Provider.Database
    | Connection.Configuration value -> value |> Configuration.Client.init |> Result.map Provider.Configuration
