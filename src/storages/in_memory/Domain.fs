module Persistence.Domain.InMemory

open System.Collections.Concurrent

type Context = ConcurrentDictionary<string, string>