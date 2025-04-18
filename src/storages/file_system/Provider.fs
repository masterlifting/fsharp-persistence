﻿[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Provider

open System.IO
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.Domain.FileSystem

let private storages = ClientFactory()

let private createFile connection =
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

let init connection =
    createFile connection
    |> Result.bind (fun file ->
        match storages.TryGetValue file with
        | true, storage -> Ok storage
        | false, _ ->
            match createClient file with
            | Ok storage ->
                storages.TryAdd(file, storage) |> ignore
                Ok storage
            | Error ex -> Error ex)

let internal createLock (stream: Client) =

    let lockFile = stream.Name + LOCK_EXT

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation {
                        Message = "Failed to create lock in file system. No more attempts."
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
            else
                try
                    if lockFile |> File.Exists then
                        do! Async.Sleep delay
                        return! innerLoop (attempts - 1) (delay * 2)
                    else
                        return File.Create(lockFile).Dispose() |> Ok
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100

let internal releaseLock (stream: Client) =
    let lockFile = stream.Name + LOCK_EXT

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation {
                        Message = "Failed to release lock in file system. No more attempts."
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
            else
                try
                    if lockFile |> File.Exists then
                        stream.Flush()
                        return File.Delete(lockFile) |> Ok
                    else
                        return Ok()
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100
