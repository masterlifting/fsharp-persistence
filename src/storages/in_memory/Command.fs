[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Command

open Infrastructure

module Json =
    let save<'a> (data: 'a array) client =
        data
        |> Json.serialize
        |> Result.bind (fun value -> client |> Write.string value)
