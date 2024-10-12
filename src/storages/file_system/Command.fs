[<RequireQualifiedAccess>]
module Persistence.FileSystem.Command

open Infrastructure

module Json =
    let save<'a> (data: 'a array) storage =
        data |> Json.serialize |> ResultAsync.wrap (Storage.Write.string storage)
