[<RequireQualifiedAccess>]
module Persistence.Core.Storage

open Persistence
open Persistence.Core.Domain

let create ``type`` =
    match ``type`` with
    | FileSystem path -> Storage.FileSystem.Context.create path |> Result.map FileSystemContext
    | InMemory -> Storage.InMemory.Context.create () |> Result.map InMemoryContext
    | Database connectionString -> Storage.Database.Context.create connectionString |> Result.map DatabaseContext
