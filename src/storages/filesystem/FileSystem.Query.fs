[<RequireQualifiedAccess>]
module Persistence.FileSystem.Query

open Infrastructure
open Infrastructure.Prelude
open Persistence.Domain

let paginate<'a> (pagination: Query.Pagination<'a>) (data: 'a list) =
    data
    |> match pagination.SortBy with
       | Query.Asc sortBy ->
           match sortBy with
           | Query.Date getValue -> List.sortBy <| getValue
           | Query.String getValue -> List.sortBy <| getValue
           | Query.Int getValue -> List.sortBy <| getValue
           | Query.Bool getValue -> List.sortBy <| getValue
           | Query.Guid getValue -> List.sortBy <| getValue
       | Query.Desc sortBy ->
           match sortBy with
           | Query.Date getValue -> List.sortByDescending <| getValue
           | Query.String getValue -> List.sortByDescending <| getValue
           | Query.Int getValue -> List.sortByDescending <| getValue
           | Query.Bool getValue -> List.sortByDescending <| getValue
           | Query.Guid getValue -> List.sortByDescending <| getValue
    |> List.skip (pagination.PageSize * (pagination.Page - 1))
    |> List.truncate pagination.PageSize

module Json =
    let get<'a> client =
        client
        |> Read.string
        |> ResultAsync.bind (Json.deserialize<'a array> |> Option.map >> Option.defaultValue (Ok [||]))
