[<AutoOpen>]
module Persistence.InMemory.Domain

open System.Collections.Concurrent

type Client = ConcurrentDictionary<string, string>
