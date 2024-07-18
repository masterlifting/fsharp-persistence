[<RequireQualifiedAccess>]
module Persistence.Storage.InMemory

open System
open Persistence.Domain.InMemory
open Infrastructure.Domain.Errors

module Context =
    let storage = Context(StringComparer.OrdinalIgnoreCase)
    let internal create () =
        try
            Ok <| storage
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }
            
module Query =
    let get key (cache: Context) =
        try
            match cache.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }
            
module Command =
    let add key value (cache: Context) =
        try
            let _ = cache.TryAdd(key, value)
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }
    
    let update key value (cache: Context) =
        try
            let _ = cache.TryUpdate(key, value, cache.[key])
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }
            
    let remove key (cache: Context) =
        try
            let _ = cache.TryRemove(key)
            Ok()
        with ex ->
            Error <| Operation { Message = ex.Message; Code = None }