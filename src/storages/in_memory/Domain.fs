module Persistence.InMemory.Domain

open System.Collections.Concurrent

type Storage = ConcurrentDictionary<string, string>
