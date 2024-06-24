module Persistence.Storage.InMemory

open System
open System.Collections.Concurrent
open Infrastructure.Domain.Errors

type Context = ConcurrentDictionary<string, string>
let storage = Context(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error <| Persistence ex.Message

let add (cache: Context) key value =
    try
        let _ = cache.TryAdd(key, value)
        Ok()
    with ex ->
        Error <| Persistence ex.Message

let remove (cache: Context) key =
    try
        let _ = cache.TryRemove(key)
        Ok()
    with ex ->
        Error <| Persistence ex.Message

let update (cache: Context) key value =
    try
        let _ = cache.TryUpdate(key, value, cache.[key])
        Ok()
    with ex ->
        Error <| Persistence ex.Message

let get (cache: Context) key =
    try
        match cache.TryGetValue(key) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error <| Persistence ex.Message
