[<RequireQualifiedAccess>]
module Persistence.Configuration.Query

open Infrastructure
open Infrastructure.Domain
open Microsoft.Extensions.Configuration

let get<'a> key (client: IConfigurationRoot) =
    client
    |> Configuration.getSection<'a> key
    |> Option.map Ok
    |> Option.defaultValue ($"Section '%s{key}' in the configuration." |> NotFound |> Error)
