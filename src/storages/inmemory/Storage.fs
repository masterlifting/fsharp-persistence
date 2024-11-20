[<RequireQualifiedAccess>]
module Persistence.InMemory.Storage

open System
open Infrastructure
open Persistence.Domain.InMemory

let private storage = Client(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }

module Read =
    let string key (storage: Client) =
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
    let string key value (storage: Client) =
        try
            storage.AddOrUpdate(key, value, (fun _ _ -> value)) |> ignore |> Ok
        with ex ->
            Error
            <| Operation
                { Message = ex.Message
                  Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
