module Persistence.FileSystem.Domain

open System
open System.Collections.Concurrent

type Storage = IO.FileStream
type StorageFactory = ConcurrentDictionary<string, Storage>
