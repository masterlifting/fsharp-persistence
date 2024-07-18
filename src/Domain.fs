module Persistence.Core.Domain

open Persistence.Domain

type Type =
    | FileSystem of string
    | InMemory
    | Database of string

type Storage =
    | FileSystemContext of FileSystem.Context
    | InMemoryContext of InMemory.Context
    | DatabaseContext of Database.Context