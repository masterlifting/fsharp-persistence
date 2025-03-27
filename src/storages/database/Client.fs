[<RequireQualifiedAccess>]
module Persistence.Storages.Database.Client

open Infrastructure.Domain

let init _ =
    "A database client" |> NotSupported |> Error
