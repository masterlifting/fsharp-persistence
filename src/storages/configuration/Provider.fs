[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Provider

open Persistence.Storages.Domain.Configuration

let init (connection: Connection) =
    {
        ReadOnlyStorage = connection.Provider
        Section = connection.Section
    }
    |> Ok

let dispose _ =
    // Configuration provider is typically managed by DI container
    // IConfigurationRoot disposal is handled by the container lifecycle
    ()
