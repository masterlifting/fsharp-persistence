[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Client

open Persistence.Storages.Domain.Configuration

let init (connection: Connection) =
    {
        ReadOnlyStorage = connection.Provider
        Section = connection.Section
    }
    |> Ok
