module Persistence.Domain.InMemory

open System.Collections.Concurrent

type Storage = ConcurrentDictionary<string, string>