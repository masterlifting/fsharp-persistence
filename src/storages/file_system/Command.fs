[<RequireQualifiedAccess>]
module Persistence.FileSystem.Command

open Infrastructure

module Json =
    let save<'a> (data: 'a array) storage =
        if data.Length = 1 then
            data
            |> Json.serialize
            |> ResultAsync.wrap (fun value -> storage |> Storage.Write.string value)
        else
            data
            |> Json.serialize
            |> ResultAsync.wrap (fun value -> storage |> Storage.Write.string value)
