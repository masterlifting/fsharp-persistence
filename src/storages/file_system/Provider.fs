[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Provider

open System.IO
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Domain
open Persistence.Storages.Domain.FileSystem
open System.Collections.Concurrent
open System.Threading

let private clients = ConcurrentDictionary<string, FileStream>()
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

let private createClient file type' =
    try
        match type' with
        | Transient ->
            let stream =
                new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
            Ok stream
        | Singleton ->
            let stream =
                new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
            Ok stream

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
        match connection.Lifetime with
        | Transient ->
            createClient filePath Transient
            |> Result.map (fun stream -> {
                Stream = stream
                Lifetime = Transient
            })
        | Singleton ->
            match clients.TryGetValue filePath with
            | true, stream ->
                match stream.CanRead && stream.CanWrite with
                | true ->
                    Ok {
                        Stream = stream
                        Lifetime = Singleton
                    }
                | false ->
                    stream.Close()
                    createClient filePath Singleton
                    |> Result.map (fun stream -> {
                        Stream = stream
                        Lifetime = Singleton
                    })
            | false, _ ->
                createClient filePath Singleton
                |> Result.map (fun stream ->
                    clients.TryAdd(filePath, stream) |> ignore
                    {
                        Stream = stream
                        Lifetime = Singleton
                    }))

let internal acquireLock (client: Client) =
    async {
        let filePath = client.Stream.Name
        let appLock = getAppLock filePath
        do! appLock.WaitAsync() |> Async.AwaitTask
        let lockFilePath = getLockFilePath filePath

        let createLockFile () =
            try
                use _ =
                    new FileStream(lockFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                Ok()
            with ex ->

                appLock.Release() |> ignore

                Error
                <| Operation {
                    Message =
                        $"FileSystem.Provider. Failed to acquire file lock after multiple attempts: {ex |> Exception.toMessage}"
                    Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                }
            |> async.Return

        return!
            Async.retry {
                Delay = 100
                Attempts = 5u<attempts>
                Perform = createLockFile
            }
    }

let internal releaseLock (client: Client) =
    async {
        let filePath = client.Stream.Name
        let appLock = getAppLock filePath
        let lockFilePath = getLockFilePath filePath
        try
            if File.Exists lockFilePath then
                File.Delete lockFilePath
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

let dispose (client: Client) =
    try
        match client.Lifetime with
        | Transient ->
            client.Stream.Close()
            client.Stream.Dispose()
        | Singleton -> client.Stream.Close()
    with _ ->
        ()
