module Persistence.MemoryStorage

open System
open System.Collections.Concurrent

let storage =
    ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase)

type Context = ConcurrentDictionary<string, string>

let create () =
    try
        Ok <| storage
    with ex ->
        Error ex.Message

let add key value (storage: Context) =
    try
        let _ = storage.TryAdd(key, value)
        Ok()
    with ex ->
        Error ex.Message

let remove key (storage: Context) =
    try
        let _ = storage.TryRemove(key)
        Ok()
    with ex ->
        Error ex.Message

let find key (storage: Context) =
    try
        match storage.TryGetValue(key) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error ex.Message
