[<RequireQualifiedAccess>]
module Persistence.Storages.Configuration.Read

open Infrastructure
open Infrastructure.Domain
open Persistence.Storages.Domain.Configuration

let section<'a> (client: Client) =
    client.Configuration
    |> Configuration.getSection<'a> client.Key
    |> Option.map Ok
    |> Option.defaultValue ($"The section '%s{client.Key}' in a configuration" |> NotFound |> Error)
