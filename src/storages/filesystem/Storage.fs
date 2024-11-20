[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System.Text
open System.IO
open Infrastructure
open Persistence.Domain.FileSystem

[<Literal>]
let private lockExt = ".lock"

let private semaphore = new System.Threading.SemaphoreSlim(1, 1)

let private storages = ClientFactory()

let private createLock (stream: Client) =

    let lockFile = stream.Name + lockExt

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation
                        { Message = $"Failed to acquire lock after {attempts} attempts."
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
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

let private releaseLock (stream: Client) =
    let lockFile = stream.Name + lockExt

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation
                        { Message = $"Failed to release lock after {attempts} attempts."
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
            else
                try
                    if lockFile |> File.Exists then
                        return File.Delete(lockFile) |> Ok
                    else
                        return Ok()
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100

let internal createSource (filePath, fileName) =
    try
        Path.Combine(filePath, fileName) |> Ok
    with ex ->
        Error <| NotSupported(ex |> Exception.toMessage)

let create file =
    let initialize () =
        try
            let client =
                new Client(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

            Ok client
        with ex ->
            Error <| NotSupported(ex |> Exception.toMessage)

    match storages.TryGetValue file with
    | true, storage -> Ok storage
    | false, _ ->
        match initialize () with
        | Ok storage ->
            storages.TryAdd(file, storage) |> ignore
            Ok storage
        | Error ex -> Error ex

module Read =

    let private read (stream: Client) =
        async {
            do! semaphore.WaitAsync() |> Async.AwaitTask

            stream.Position <- 0
            let data = Array.zeroCreate<byte> (int stream.Length)
            let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask

            semaphore.Release() |> ignore

            return data
        }

    let bytes (stream: Client) =
        stream
        |> createLock
        |> ResultAsync.bindAsync (fun _ ->
            async {
                try
                    let! data = stream |> read

                    return
                        match data.Length with
                        | 0 -> Ok None
                        | _ -> Ok(Some data)
                with ex ->
                    return
                        Error
                        <| Operation
                            { Message = ex |> Exception.toMessage
                              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
            })
        |> Async.bind (fun result -> stream |> releaseLock |> ResultAsync.bind (fun _ -> result))

    let string (stream: Client) =
        stream
        |> createLock
        |> ResultAsync.bindAsync (fun _ ->
            async {
                try
                    let! data = stream |> read |> Async.map Encoding.UTF8.GetString

                    return
                        match data.Length with
                        | 0 -> Ok None
                        | _ -> Ok(Some data)
                with ex ->
                    return
                        Error
                        <| Operation
                            { Message = ex |> Exception.toMessage
                              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
            })
        |> Async.bind (fun result -> stream |> releaseLock |> ResultAsync.bind (fun _ -> result))

module Write =

    let private write (data: byte array) (stream: Client) =
        async {
            do! semaphore.WaitAsync() |> Async.AwaitTask

            stream.Position <- 0
            stream.SetLength 0

            do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
            do! stream.FlushAsync() |> Async.AwaitTask

            semaphore.Release() |> ignore
        }

    let bytes (stream: Client) data =
        stream
        |> createLock
        |> ResultAsync.bindAsync (fun _ ->
            async {
                try
                    do! stream |> write data
                    return Ok()
                with ex ->
                    return
                        Error
                        <| Operation
                            { Message = ex |> Exception.toMessage
                              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
            })
        |> Async.bind (fun result -> stream |> releaseLock |> ResultAsync.bind (fun _ -> result))

    let string (stream: Client) (data: string) =
        stream
        |> createLock
        |> ResultAsync.bindAsync (fun _ ->
            async {
                try
                    do! stream |> write (data |> Encoding.UTF8.GetBytes)
                    return Ok()
                with ex ->
                    return
                        Error
                        <| Operation
                            { Message = ex |> Exception.toMessage
                              Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
            })
        |> Async.bind (fun result -> stream |> releaseLock |> ResultAsync.bind (fun _ -> result))
