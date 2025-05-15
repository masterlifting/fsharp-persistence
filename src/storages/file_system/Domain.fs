module Persistence.Storages.Domain.FileSystem

open System
open System.Collections.Concurrent

type Client = IO.FileStream
type internal ClientFactory = ConcurrentDictionary<string, Client>
type Connection = { FilePath: string; FileName: string }

type internal Lock =
    | Read of Client
    | Write of Client

[<Literal>]
let internal LOCK_EXT = ".lock"

let internal Semaphore = new System.Threading.SemaphoreSlim(1, 1)
