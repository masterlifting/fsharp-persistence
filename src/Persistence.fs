module Persistence

open System.IO
open System

let private _inMemoryStorage: Map<string, string> = Map.empty

type Type =
    | FileStorage of string
    | InMemoryStorage
    | DatabaseStorage of string

module Scope =
    type Type =
        | FileStorageScope of FileStream
        | InMemoryStorageScope of Map<string, string>
        | DatabaseStorageScope

    let create persistenceType =
        match persistenceType with
        | FileStorage path ->
            try
                let stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
                Ok <| FileStorageScope stream
            with ex ->
                Error ex.Message
        | InMemoryStorage -> Ok <| InMemoryStorageScope _inMemoryStorage
        | DatabaseStorage connectionString -> Error "Database storage is not supported"

    let remove scope =
        match scope with
        | FileStorageScope stream -> stream.Dispose()
        | InMemoryStorageScope _ -> ignore ()
        | DatabaseStorageScope -> ignore ()
