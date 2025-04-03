module Persistence.Storages.Domain.InMemory

open System.Collections.Concurrent

type Client = {
    TableName: string
    Storage: ConcurrentDictionary<string, string>
}
type Connection = { TableName: string }
