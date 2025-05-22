[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Provider

open System.IO
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.FileSystem
open System.Collections.Concurrent
open System.Threading

let private clients = ClientFactory()
let private appLocks = ConcurrentDictionary<string, SemaphoreSlim>()

let private createFilePath connection =
    try
        Path.Combine(connection.FilePath, connection.FileName) |> Ok
    with ex ->
        Error
        <| Operation {
            Message =
                $"Failed to create file path {connection.FilePath}. "
                + (ex |> Exception.toMessage)
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }

let private createClient file =
    try
        let client =
            new Client(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

        Ok client
    with ex ->
        Error
        <| Operation {
            Message = $"Failed to create client for file {file}. " + (ex |> Exception.toMessage)
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }

let private getAppLock (filePath: string) =
    appLocks.GetOrAdd(filePath, fun _ -> new SemaphoreSlim(1, 1))

let private getLockFilePath (filePath: string) = filePath + ".lock"

let init connection =
    createFilePath connection
    |> Result.bind (fun filePath ->
        match clients.TryGetValue filePath with
        | true, storage -> Ok storage
        | false, _ ->
            createClient filePath
            |> Result.map (fun client ->
                clients.TryAdd(filePath, client) |> ignore
                client))

let internal acquireLock (stream: Client) =
    async {
        let filePath = stream.Name
        let appLock = getAppLock filePath
        do! appLock.WaitAsync() |> Async.AwaitTask
        let lockFilePath = getLockFilePath filePath
        let rec tryCreateLockFile attempts (delay: int) =
            async {
                try
                    use _ =
                        new FileStream(lockFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                    return Ok()
                with ex ->
                    if attempts <= 0 then
                        appLock.Release() |> ignore
                        return
                            Error
                            <| Operation {
                                Message =
                                    $"FileSystem.Provider. Failed to acquire file lock after multiple attempts: {ex |> Exception.toMessage}"
                                Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                            }
                    else
                        do! Async.Sleep delay
                        return! tryCreateLockFile (attempts - 1) (delay * 2)
            }
        return! tryCreateLockFile 5 100
    }

let internal releaseLock (stream: Client) =
    async {
        let filePath = stream.Name
        let appLock = getAppLock filePath
        let lockFilePath = getLockFilePath filePath
        try
            if File.Exists(lockFilePath) then
                File.Delete(lockFilePath)
            appLock.Release() |> ignore
            return Ok()
        with ex ->
            appLock.Release() |> ignore
            return
                Error
                <| Operation {
                    Message = $"FileSystem.Provider. Failed to release locks: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
    }
