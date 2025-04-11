[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Provider

open Persistence.Storages.Domain.Configuration

let init (connection: Connection) =
    {
        ReadOnlyStorage = connection.Provider
        Section = connection.Section
    }
    |> Ok
