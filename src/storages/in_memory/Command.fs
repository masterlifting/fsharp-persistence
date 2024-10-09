[<RequireQualifiedAccess>]
module Persistence.InMemory.Command

open Infrastructure

module Json =
    let save<'a> key (data: 'a array) storage =
        if data.Length = 1 then
            data
            |> Json.serialize
            |> Result.bind (fun value -> storage |> Storage.Command.add key value)
        else
            data
            |> Json.serialize
            |> Result.bind (fun value -> storage |> Storage.Command.update key value)
