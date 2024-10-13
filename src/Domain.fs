module Persistence.Domain

open System
open System.Collections.Concurrent

[<Literal>]
let internal SectionName = "Persistence"

module FileSystem =
    [<Literal>]
    let SectionName = "FileSystem"

    type Storage = IO.FileStream
    type StorageFactory = ConcurrentDictionary<string, Storage>
    type SourcePath = { Path: string; File: string }

module InMemory =
    [<Literal>]
    let SectionName = "InMemory"

    type Storage = ConcurrentDictionary<string, string>

module Database =
    type Type =
        | SqlServer
        | Postgres
        | MongoDb
        | AzureTable

    type Storage = { f: string -> string }

[<RequireQualifiedAccess>]
module Storage =

    type Context =
        | FileSystem of FileSystem.SourcePath
        | InMemory
        | Database of string

    type Type =
        | FileSystem of FileSystem.Storage
        | InMemory of InMemory.Storage
        | Database of Database.Storage

module ErrorCodes =
    [<Literal>]
    let NotFound = "NotFound"

    [<Literal>]
    let AlreadyExists = "AlreadyExists"

module Query =

    type OrderBy<'a> =
        | Date of ('a -> DateTime)
        | String of ('a -> string)
        | Int of ('a -> int)
        | Bool of ('a -> bool)
        | Guid of ('a -> Guid)

    type SortBy<'a> =
        | Asc of OrderBy<'a>
        | Desc of OrderBy<'a>

    type Predicate<'a> = 'a -> bool

    type Pagination<'a> =
        { Page: int
          PageSize: int
          SortBy: SortBy<'a> }
