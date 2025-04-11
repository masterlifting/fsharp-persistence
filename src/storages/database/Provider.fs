[<RequireQualifiedAccess>]
module Persistence.Storages.Database.Provider

open Infrastructure.Domain

let init _ =
    "A database client" |> NotSupported |> Error
