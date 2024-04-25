module Persistence.Core

type Type =
    | FileSystem of string
    | InMemory
    | Database of string

module Storage =
    type Type =
        | FileStorage of FileSystem.Storage
        | MemoryStorage of InMemory.Storage
        | DatabaseStorage of Database.Storage

    let create persistenceType =
        match persistenceType with
        | FileSystem path ->
            match FileSystem.create path with
            | Ok storage -> Ok <| FileStorage storage
            | Error message -> Error message
        | InMemory ->
            match InMemory.create () with
            | Ok storage -> Ok <| MemoryStorage storage
            | Error message -> Error message
        | Database connectionString ->
            match Database.create connectionString with
            | Ok storage -> Ok <| DatabaseStorage storage
            | Error message -> Error message
