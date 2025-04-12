[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Read

open Infrastructure
open Infrastructure.Domain
open Persistence.Storages.Domain.Configuration

let section<'a> (client: Client) =
    client.ReadOnlyStorage
    |> Configuration.Client.tryGetSection<'a> client.Section
    |> Option.map Ok
    |> Option.defaultValue ($"The section '%s{client.Section}' in a configuration" |> NotFound |> Error)
