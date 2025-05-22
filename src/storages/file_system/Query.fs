[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Query

open Infrastructure.Prelude
open Infrastructure.SerDe

module Json =
    let get<'a> client =
        client
        |> Read.string
        |> ResultAsync.bind (Json.deserialize<'a array> |> Option.map >> Option.defaultValue (Ok [||]))
