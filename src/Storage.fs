[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Persistence
open Persistence.Domain

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
    |> Configuration.getSection<string> $"{SECTION_NAME}:%s{sectionName}"
    |> Option.map Ok
    |> Option.defaultValue (Error <| NotFound $"Section '%s{SECTION_NAME}' in the configuration.")

let create connection =
    match connection with
    | Connection.FileSystem src ->
        (src.FilePath, src.FileName)
        |> FileSystem.Storage.createSource
        |> Result.bind FileSystem.Storage.create
        |> Result.map Storage.FileSystem
    | Connection.InMemory -> InMemory.Storage.create () |> Result.map Storage.InMemory
    | Connection.Database connectionString -> Database.Storage.create connectionString |> Result.map Storage.Database
    | Connection.Configuration config -> Configuration.Storage.create config |> Storage.Configuration |> Ok

module Command =
    let execute storage execute =
        match storage with
        | Storage.InMemory context -> context |> execute
        | _ -> async { return Error <| NotSupported $"Storage {storage}" }
