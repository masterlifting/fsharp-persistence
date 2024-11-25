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

let create connection =
    match connection with
    | Connection.FileSystem src ->
        (src.FIlePath, src.FileName)
        |> FileSystem.Storage.createSource
        |> Result.bind FileSystem.Storage.create
        |> Result.map Storage.FileSystem
    | Connection.InMemory -> InMemory.Storage.create () |> Result.map Storage.InMemory
    | Connection.Database connectionString -> Database.Storage.create connectionString |> Result.map Storage.Database

module Command =
    let execute storage execute =
        match storage with
        | Storage.InMemory context -> context |> execute
        | _ -> async { return Error <| NotSupported $"Storage {storage}" }
