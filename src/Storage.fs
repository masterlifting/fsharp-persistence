[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Infrastructure.Domain

type Type =
    | FileSystem of FileSystem.Domain.Client
    | InMemory of InMemory.Domain.Client
    | Database of Database.Domain.Client
    | Configuration of Configuration.Domain.Client

type Connection =
    | FileSystem of FileSystem.Domain.Source
    | InMemory
    | Database of string
    | Configuration of Configuration.Domain.Client

/// <summary>
/// Gets the connection value from the configuration.
/// </summary>
/// <param name="sectionName">
/// The name of the section in the 'Persistence' section.
/// </param>
/// <param name="configuration">
/// IConfigurationRoot instance.
/// </param>
let getConnectionString sectionName configuration =
    configuration
    |> Configuration.getSection<string> $"Persistence:%s{sectionName}"
    |> Option.map Ok
    |> Option.defaultValue (Error <| NotFound $"Section 'Persistence' in the configuration.")

let init connection =
    match connection with
    | Connection.InMemory -> InMemory.Storage.init () |> Result.map Type.InMemory
    | Connection.FileSystem src -> src |> FileSystem.Storage.init |> Result.map Type.FileSystem
    | Connection.Database connectionString -> connectionString |> Database.Storage.init |> Result.map Type.Database
    | Connection.Configuration connection -> connection |> Type.Configuration |> Ok
