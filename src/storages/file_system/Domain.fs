module Persistence.Storages.Domain.FileSystem

open System
open System.Collections.Concurrent

type Client = IO.FileStream
type internal ClientFactory = ConcurrentDictionary<string, Client>
type Connection = { FilePath: string; FileName: string }
