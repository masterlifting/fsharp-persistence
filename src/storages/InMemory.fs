module Persistence.InMemory

open System
open System.Collections.Concurrent

type Storage = ConcurrentDictionary<string, string>
let storage = Storage(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error ex.Message

let add key value (storage: Storage) =
    try
        let _ = storage.TryAdd(key, value)
        Ok()
    with ex ->
        Error ex.Message

let remove key (storage: Storage) =
    try
        let _ = storage.TryRemove(key)
        Ok()
    with ex ->
        Error ex.Message

let update key value (storage: Storage) =
    try
        let _ = storage.TryUpdate(key, value, storage.[key])
        Ok()
    with ex ->
        Error ex.Message

let find key (storage: Storage) =
    try
        match storage.TryGetValue(key) with
        | true, value -> Ok <| Some value
        | _ -> Ok <| None
    with ex ->
        Error ex.Message
