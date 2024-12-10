[<AutoOpen>]
module Persistence.FileSystem.Domain

open System
open System.Collections.Concurrent

type Client = IO.FileStream
type ClientFactory = ConcurrentDictionary<string, Client>
type Source = { FilePath: string; FileName: string }

type internal Lock =
    | Read of Client
    | Write of Client
