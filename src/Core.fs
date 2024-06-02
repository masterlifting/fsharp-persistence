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

let getStorage persistenceType =
    match persistenceType with
    | FileSystem path ->
        match FileSystem.create path with
        | Ok storage -> Ok <| FileStorage storage
        | Error message -> Error <| Persistence message
    | InMemory ->
        match InMemory.create () with
        | Ok storage -> Ok <| MemoryStorage storage
        | Error message -> Error <| Persistence message
    | Database connectionString ->
        match Database.create connectionString with
        | Ok storage -> Ok <| DatabaseStorage storage
        | Error message -> Error <| Persistence message
