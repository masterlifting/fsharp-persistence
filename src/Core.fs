module Persistence.Storage.Core

open Persistence.Domain
open Persistence.Storage

type StorageType =
    | FileSystemStorage of FileSystem.Storage
    | InMemoryStorage of InMemory.Storage
    | DatabaseStorage of Database.Storage

let createStorage ``type`` =
    match ``type`` with
    | Core.FileSystem path -> FileSystem.Context.create path |> Result.map FileSystemStorage
    | Core.InMemory -> InMemory.Context.create () |> Result.map InMemoryStorage
    | Core.Database connectionString -> Database.Context.create connectionString |> Result.map DatabaseStorage
