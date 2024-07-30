[<RequireQualifiedAccess>]
module Persistence.Storage

open Persistence

[<RequireQualifiedAccess>]
type Type =
    | FileSystem of FileSystem.Domain.Storage
    | InMemory of InMemory.Domain.Storage
    | Database of Database.Domain.Storage

let create context =
    match context with
    | Domain.FileSystem path -> FileSystem.Storage.create path |> Result.map Type.FileSystem
    | Domain.InMemory -> InMemory.Storage.create () |> Result.map Type.InMemory
    | Domain.Database connectionString -> Database.Storage.create connectionString |> Result.map Type.Database
