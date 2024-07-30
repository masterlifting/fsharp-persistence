module Persistence.Domain

type Context =
    | FileSystem of string
    | InMemory
    | Database of string

module ErrorCodes =
    [<Literal>]
    let NotFound = "NotFound"

    [<Literal>]
    let AlreadyExists = "AlreadyExists"
