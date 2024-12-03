[<RequireQualifiedAccess>]
module Persistence.InMemory.Command

open Infrastructure

module Json =
    let save<'a> key (data: 'a array) client =
        data
        |> Json.serialize
        |> Result.bind (fun value -> client |> Storage.Write.string key value)
