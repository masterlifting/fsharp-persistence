module Persistence.InMemoryStorage

open System
open System.Collections.Concurrent

type Provider = ConcurrentDictionary<string, string>

let create () =
    try
        Ok <| ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    with ex ->
        Error ex.Message

let add key value (storage: Provider) =
    try
        let _ = storage.TryAdd(key, value)
        Ok()
    with ex ->
        Error ex.Message

let remove key (storage: Provider) =
    try
        let _ = storage.TryRemove(key)
        Ok()
    with ex ->
        Error ex.Message

let find key (storage: Provider) =
    try
        match storage.TryGetValue(key) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error ex.Message
