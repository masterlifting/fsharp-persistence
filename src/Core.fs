module Persistence.Core

open Infrastructure.Domain.Errors

type Type =
    | FileSystem of string
    | InMemory
    | Database of string

type Storage =
    | FileStorage of FileSystem.Storage
    | MemoryStorage of InMemory.Storage
    | DatabaseStorage of Database.Storage

let createStorage persistenceType =
    match persistenceType with
    | FileSystem path ->
        FileSystem.create path
        |> Result.mapError Persistence
        |> Result.map (fun storage -> FileStorage storage)
    | InMemory ->
        InMemory.create ()
        |> Result.mapError Persistence
        |> Result.map (fun storage -> MemoryStorage storage)
    | Database connectionString ->
        Database.create connectionString
        |> Result.mapError Persistence
        |> Result.map (fun storage -> DatabaseStorage storage)
