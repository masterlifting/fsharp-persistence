[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Query

open Infrastructure.SerDe

module Json =
    let get<'a> client =
        client
        |> Read.string
        |> Result.bind (Json.deserialize<'a array> |> Option.map >> Option.defaultValue (Ok [||]))
