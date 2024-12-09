[<RequireQualifiedAccess>]
module Persistence.FileSystem.Command

open Infrastructure.Prelude
open Infrastructure.SerDe

module Json =
    let save<'a> (data: 'a array) client =
        data |> Json.serialize |> ResultAsync.wrap (Write.string client)
