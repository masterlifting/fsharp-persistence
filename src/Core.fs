module Persistence.Core

open Persistence.Storage
open Domain

let createStorage ``type`` =
    match ``type`` with
    | FileSystem path -> FileSystem.create path |> Result.map FileSystemContext
    | InMemory -> InMemory.create () |> Result.map InMemoryContext
    | Database connectionString -> Database.create connectionString |> Result.map DatabaseContext
