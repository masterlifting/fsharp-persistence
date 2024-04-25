module Persistence.Core

type Type =
    | FileStorage of string
    | InMemoryStorage
    | DatabaseStorage of string

module Scope =
    type Type =
        | FileStorageScope of FileStorage.Provider
        | InMemoryStorageScope of InMemoryStorage.Context
        | DatabaseStorageScope of DatabaseStorage.Provider

    let create persistenceType =
        match persistenceType with
        | FileStorage path ->
            match FileStorage.create path with
            | Ok storage -> FileStorageScope storage |> Ok
            | Error message -> Error message
        | InMemoryStorage ->
            match InMemoryStorage.create () with
            | Ok storage -> InMemoryStorageScope storage |> Ok
            | Error message -> Error message
        | DatabaseStorage connectionString ->
            match DatabaseStorage.create connectionString with
            | Ok storage -> DatabaseStorageScope storage |> Ok
            | Error message -> Error message

    let clear scope =
        match scope with
        | FileStorageScope storage -> storage.Dispose()
        | InMemoryStorageScope storage -> ignore ()
        | DatabaseStorageScope _ -> ignore ()
