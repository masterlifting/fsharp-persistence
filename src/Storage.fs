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
    | InMemory c -> c |> InMemory.Provider.init |> Result.map Provider.InMemory
    | FileSystem c -> c |> FileSystem.Provider.init |> Result.map Provider.FileSystem
    | Database c -> c |> Database.Provider.init |> Result.map Provider.Database
    | Configuration c -> c |> Configuration.Provider.init |> Result.map Provider.Configuration

let dispose provider =
    match provider with
    | Provider.InMemory client -> client |> InMemory.Provider.dispose
    | Provider.FileSystem client -> client |> FileSystem.Provider.dispose
    | Provider.Configuration client -> client |> Configuration.Provider.dispose
    | Provider.Database client -> client |> Database.Provider.dispose
