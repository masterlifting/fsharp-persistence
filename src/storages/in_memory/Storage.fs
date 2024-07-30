[<RequireQualifiedAccess>]
module Persistence.InMemory.Storage

open System
open Infrastructure
open Persistence.InMemory.Domain

module Context =
    let storage = Storage(StringComparer.OrdinalIgnoreCase)

    let internal create () =
        try
            Ok <| storage
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }

module Query =
    let get key (cache: Storage) =
        try
            match cache.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }

module Command =
    let add key value (cache: Storage) =
        try
            let _ = cache.TryAdd(key, value)
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }

    let update key value (cache: Storage) =
        try
            let _ = cache.TryUpdate(key, value, cache[key])
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }

    let remove key (cache: Storage) =
        try
            let _ = cache.TryRemove(key)
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }
