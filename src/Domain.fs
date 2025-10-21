module Persistence.Domain

open System

type Lifetime =
    | Transient
    | Singleton

[<RequireQualifiedAccess>]
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

    type Pagination<'a> = {
        Page: int
        PageSize: int
        SortBy: SortBy<'a>
    }
