module Persistence.InMemory

open System
open System.Collections.Concurrent
open Infrastructure.Domain.Errors

type Storage = ConcurrentDictionary<string, string>
let storage = Storage(StringComparer.OrdinalIgnoreCase)

let internal create () =
    try
        Ok <| storage
    with ex ->
        Error ex.Message

let add (storage: Storage) =
    fun key value ->
        try
            let _ = storage.TryAdd(key, value)
            Ok()
        with ex ->
            Error <| Persistence ex.Message

let remove (storage: Storage) =
    fun key ->
        try
            let _ = storage.TryRemove(key)
            Ok()
        with ex ->
            Error <| Persistence ex.Message

let update (storage: Storage) =
    fun key value ->
        try
            let _ = storage.TryUpdate(key, value, storage.[key])
            Ok()
        with ex ->
            Error <| Persistence ex.Message

let get (storage: Storage) =
    fun key ->
        try
            match storage.TryGetValue(key) with
            | true, value -> Ok <| Some value
            | _ -> Ok <| None
        with ex ->
            Error <| Persistence ex.Message
