module Persistence.Domain

open System
open System.Collections.Concurrent

[<Literal>]
let internal SECTION_NAME = "Persistence"

module FileSystem =
    type Client = IO.FileStream
    type ClientFactory = ConcurrentDictionary<string, Client>
    type Source = { FilePath: string; FileName: string }

    type internal Lock =
        | Read of Client
        | Write of Client

module InMemory =
    type Client = ConcurrentDictionary<string, string>

module Database =
    type DatabaseType =
        | SqlServer
        | Postgres
        | MongoDb
        | AzureTable

    type Client = { f: string -> string }

type Connection =
    | FileSystem of FileSystem.Source
    | InMemory
    | Database of string

type Storage =
    | FileSystem of FileSystem.Client
    | InMemory of InMemory.Client
    | Database of Database.Client

module ErrorCode =
    [<Literal>]
    let NOT_FOUND = "NotFound"

    [<Literal>]
    let ALREADY_EXISTS = "AlreadyExists"

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
