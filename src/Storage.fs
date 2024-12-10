[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Infrastructure.Domain
open Microsoft.Extensions.Configuration

type Client =
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
    | Connection.FileSystem src ->
        (src.FilePath, src.FileName)
        |> FileSystem.Storage.createSource
        |> Result.bind FileSystem.Storage.init
        |> Result.map Client.FileSystem
    | Connection.InMemory -> InMemory.Storage.init () |> Result.map Client.InMemory
    | Connection.Database connectionString -> Database.Storage.init connectionString |> Result.map Client.Database
    | Connection.Configuration connection -> connection |> Client.Configuration |> Ok
