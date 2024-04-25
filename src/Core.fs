module Persistence.Core

type Type =
    | FileStorage of string
    | MemoryStorage
    | DatabaseStorage of string

module Scope =
    type Type =
        | FileStorageScope of FileStorage.Context
        | MemoryStorageScope of MemoryStorage.Context
        | DatabaseStorageScope of DatabaseStorage.Context

    let create persistenceType =
        match persistenceType with
        | FileStorage path ->
            match FileStorage.create path with
            | Ok storage -> FileStorageScope storage |> Ok
            | Error message -> Error message
        | MemoryStorage ->
            match MemoryStorage.create () with
            | Ok storage -> MemoryStorageScope storage |> Ok
            | Error message -> Error message
        | DatabaseStorage connectionString ->
            match DatabaseStorage.create connectionString with
            | Ok storage -> DatabaseStorageScope storage |> Ok
            | Error message -> Error message

    let clear scope =
        match scope with
        | FileStorageScope storage -> storage.Dispose()
        | MemoryStorageScope storage -> storage.Clear()
        | DatabaseStorageScope _ -> ignore ()
