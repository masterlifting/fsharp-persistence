[<RequireQualifiedAccess>]
module Persistence.InMemory.Storage

open System
open Infrastructure
open Persistence.InMemory.Domain

let private storage = Storage(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

module Read =
    let string key (storage: Storage) =
        try
            match storage.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

module Write =
    let string key value (storage: Storage) =
        try
            storage.AddOrUpdate(key, value, (fun _ _ -> value)) |> ignore |> Ok
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
