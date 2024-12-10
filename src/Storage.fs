[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Infrastructure.Domain
open Microsoft.Extensions.Configuration

type Storage =
    | FileSystem of FileSystem.Domain.Client
    | InMemory of InMemory.Domain.Client
    | Database of Database.Domain.Client
    | Configuration of sectionName: string * configuration: IConfigurationRoot

type Connection =
    | FileSystem of FileSystem.Domain.Source
    | InMemory
    | Database of string
    | Configuration of sectionName: string * configuration: IConfigurationRoot

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
        |> Result.map Storage.FileSystem
    | Connection.InMemory -> InMemory.Storage.init () |> Result.map Storage.InMemory
    | Connection.Database connectionString -> Database.Storage.init connectionString |> Result.map Storage.Database
    | Connection.Configuration(section, config) ->
        Configuration.Storage.init section config |> Storage.Configuration |> Ok
