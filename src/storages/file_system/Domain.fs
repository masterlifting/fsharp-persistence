module Persistence.Storages.Domain.FileSystem

open System
open System.Collections.Concurrent

type Client = IO.FileStream
type ClientFactory = ConcurrentDictionary<string, Client>
type Connection = { FilePath: string; FileName: string }

type internal Lock =
    | Read of Client
    | Write of Client
