[<RequireQualifiedAccess>]
module Persistence.InMemory.Command

open Infrastructure

module Json =
    let save<'a> key (data: 'a array) storage =
        data
        |> Json.serialize
        |> Result.bind (fun value -> storage |> Storage.Write.string key value)
