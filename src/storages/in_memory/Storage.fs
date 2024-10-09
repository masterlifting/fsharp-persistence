[<RequireQualifiedAccess>]
module Persistence.InMemory.Storage

open System
open Infrastructure
open Persistence.InMemory.Domain

let storage = Storage(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

module Query =
    let get key (storage: Storage) =
        try
            match storage.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

module Command =
    let add key value (storage: Storage) =
        try
            let _ = storage.TryAdd(key, value)
            Ok()
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

    let update key value (cache: Storage) =
        try
            let _ = cache.TryUpdate(key, value, cache[key])
            Ok()
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

    let remove key (storage: Storage) =
        try
            let _ = storage.TryRemove(key)
            Ok()
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
