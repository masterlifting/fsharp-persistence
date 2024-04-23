module Persistence.InMemoryStorage
open System.Collections.Concurrent

let add key value (storage: ConcurrentDictionary<string, string>) =
    try
        let _ = storage.TryAdd(key, value)
        Ok ()
    with ex -> Error ex.Message
let remove key (storage: ConcurrentDictionary<string, string>) =
    try
        let _ = storage.TryRemove(key)
        Ok ()
    with ex -> Error ex.Message

let tryFind key (storage: ConcurrentDictionary<string, string>) =
    try
        match storage.TryGetValue(key) with
        | true, value -> Some value
        | _ -> None
    with _ -> None
