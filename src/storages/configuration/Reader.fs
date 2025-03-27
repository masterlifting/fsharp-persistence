[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Read

open Infrastructure
open Infrastructure.Domain
open Persistence.Storages.Domain.Configuration

let section<'a> (client: Client) =
    client.Configuration
    |> Configuration.getSection<'a> client.Key
    |> Option.map Ok
    |> Option.defaultValue ($"Section '%s{client.Key}' in the configuration." |> NotFound |> Error)
