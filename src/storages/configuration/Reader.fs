[<RequireQualifiedAccess>]
module Persistence.Configuration.Read

open Infrastructure
open Infrastructure.Domain

let section<'a> (client: Client) =
    client.Configuration
    |> Configuration.getSection<'a> client.Key
    |> Option.map Ok
    |> Option.defaultValue ($"Section '%s{client.Key}' in the configuration." |> NotFound |> Error)
