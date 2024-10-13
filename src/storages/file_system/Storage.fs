[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System.Text
open System.IO
open Infrastructure
open Persistence.Domain.FileSystem

[<Literal>]
let private lock = ".lock"

let private semaphore = new System.Threading.SemaphoreSlim(1, 1)

let private storages = StorageFactory()

let private createLock filePath =
    let lockFile = filePath + lock

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                failwith $"FileSystem.Storage.createLock: {lockFile} could not be created."

            if lockFile |> File.Exists then
                do! Async.Sleep delay
                return! innerLoop (attempts - 1) (delay * 2)
            else
                try
                    do! semaphore.WaitAsync() |> Async.AwaitTask
                    File.Create(lockFile).Dispose()
                    semaphore.Release() |> ignore
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100

let private releaseLock filePath =
    let lockFile = filePath + lock

    if File.Exists(lockFile) then
        semaphore.Wait() |> ignore
        File.Delete(lockFile)
        semaphore.Release() |> ignore

let internal createFilePath (path, file) =
    try
        Path.Combine(path, file) |> Ok
    with ex ->
        Error <| NotSupported ex.Message

let private create' filePath =
    try
        let storage =
            new Storage(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

        Ok storage
    with ex ->
        Error <| NotSupported ex.Message

let create filePath =
    match storages.TryGetValue filePath with
    | true, storage -> Ok storage
    | false, _ ->
        match create' filePath with
        | Ok storage ->
            storages.TryAdd(filePath, storage) |> ignore
            Ok storage
        | Error ex -> Error ex

module Read =

    let bytes (stream: Storage) =
        async {
            try
                do! stream.Name |> createLock

                stream.Position <- 0

                let data = Array.zeroCreate<byte> (int stream.Length)
                let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask

                stream.Name |> releaseLock

                return
                    match data.Length with
                    | 0 -> Ok None
                    | _ -> Ok(Some data)

            with ex ->
                stream.Name |> releaseLock

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let string (stream: Storage) =
        async {
            try
                do! stream.Name |> createLock

                stream.Position <- 0

                let buffer = Array.zeroCreate<byte> (int stream.Length)
                let! _ = stream.ReadAsync(buffer, 0, buffer.Length) |> Async.AwaitTask

                stream.Name |> releaseLock

                let data = buffer |> Encoding.UTF8.GetString

                return
                    match data.Length with
                    | 0 -> Ok None
                    | _ -> Ok(Some data)

            with ex ->
                stream.Name |> releaseLock

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

module Write =

    let bytes (stream: Storage) data =
        async {
            try
                do! stream.Name |> createLock

                stream.Position <- 0
                stream.SetLength 0

                do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
                do! stream.FlushAsync() |> Async.AwaitTask

                stream.Name |> releaseLock

                return Ok()
            with ex ->
                stream.Name |> releaseLock

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let string (stream: Storage) (data: string) =
        async {
            try
                do! stream.Name |> createLock

                let buffer = data |> Encoding.UTF8.GetBytes
                stream.Position <- 0
                stream.SetLength 0

                do! stream.WriteAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
                do! stream.FlushAsync() |> Async.AwaitTask

                stream.Name |> releaseLock

                return Ok()
            with ex ->
                stream.Name |> releaseLock

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }
