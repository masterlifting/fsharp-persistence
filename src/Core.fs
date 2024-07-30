[<RequireQualifiedAccess>]
module Persistence.Storage

open Persistence

type StorageType =
    | FileSystem of FileSystem.Domain.Storage
    | InMemory of InMemory.Domain.Storage
    | Database of Database.Domain.Storage

let create ``type`` =
    match ``type`` with
    | Domain.FileSystem path -> FileSystem.Storage.Context.create path |> Result.map FileSystem
    | Domain.InMemory -> InMemory.Storage.Context.create () |> Result.map InMemory
    | Domain.Database connectionString -> Database.Storage.Context.create connectionString |> Result.map Database
