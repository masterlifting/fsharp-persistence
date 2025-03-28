[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Client

open Persistence.Storages.Domain.Configuration

let init (connection: Connection) =
    {
        Configuration = connection.Provider
        Key = connection.SectionName
    }
    |> Ok
