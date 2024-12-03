[<RequireQualifiedAccess>]
module Persistence.FileSystem.Command

open Infrastructure

module Json =
    let save<'a> (data: 'a array) client =
        data |> Json.serialize |> ResultAsync.wrap (Storage.Write.string client)
