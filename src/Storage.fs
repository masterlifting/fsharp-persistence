[<RequireQualifiedAccess>]
module Persistence.Storage

open Infrastructure
open Persistence
open Persistence.Domain

let getConnectionString persistenceType configuration =
    configuration
    |> Configuration.getSection<string> $"{SECTION_NAME}:{persistenceType}"
    |> Option.map Ok
    |> Option.defaultValue (Error <| NotFound $"Section '%s{SECTION_NAME}' in the configuration.")

let create context =
    match context with
    | Storage.Context.FileSystem filePath ->
        (filePath.Directory, filePath.FileName)
        |> FileSystem.Storage.createFilePath
        |> Result.bind FileSystem.Storage.create
        |> Result.map Storage.Type.FileSystem
    | Storage.Context.InMemory -> InMemory.Storage.create () |> Result.map Storage.Type.InMemory
    | Storage.Context.Database connectionString ->
        Database.Storage.create connectionString |> Result.map Storage.Type.Database

module Command =
    let execute storage execute =
        match storage with
        | Storage.Type.InMemory context -> context |> execute
        | _ -> async { return Error <| NotSupported $"Storage {storage}" }
