module Persistence.InMemoryStorage

open System
open System.Collections.Concurrent

let cache =
    new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase)

type Context() =
    member _.add key value =
        try
            let _ = cache.TryAdd(key, value)
            Ok()
        with ex ->
            Error ex.Message

    member _.remove key =
        try
            let _ = cache.TryRemove(key)
            Ok()
        with ex ->
            Error ex.Message

    member _.find key =
        try
            match cache.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error ex.Message

    member _.clear() =
        try
            cache.Clear()
            Ok()
        with ex ->
            Error ex.Message

let create () =
    try
        Ok <| Context()
    with ex ->
        Error ex.Message
