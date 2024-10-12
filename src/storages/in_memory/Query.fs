[<RequireQualifiedAccess>]
module Persistence.InMemory.Query

open Infrastructure
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
    let get<'a> key storage =
        storage
        |> Storage.Read.string key
        |> Result.bind (Json.deserialize<'a array> |> Option.map >> Option.defaultValue (Ok [||]))
