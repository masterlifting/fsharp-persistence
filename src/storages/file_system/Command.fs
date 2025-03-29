[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Command

open Infrastructure.Prelude
open Infrastructure.SerDe

module Json =
    let save<'a> (data: 'a array) client =
        data |> Json.serialize |> ResultAsync.wrap (Write.string client)

    let save'<'a> (data: 'a array) options client =
        data |> Json.serialize' options |> ResultAsync.wrap (Write.string client)
