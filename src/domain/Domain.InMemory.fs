module Persistence.Domain.InMemory

open System.Collections.Concurrent

type Client = ConcurrentDictionary<string, string>
