[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System
open System.IO
open Infrastructure
open Persistence.FileSystem.Domain

let private storages = StorageFactory()

let private create' path =
    try
        let storage =
            new Storage(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

        Ok storage
    with ex ->
        Error <| NotSupported ex.Message

let create path =
    match storages.TryGetValue path with
    | true, storage -> Ok storage
    | false, _ ->
        match create' path with
        | Ok storage ->
            storages.TryAdd(path, storage) |> ignore
            Ok storage
        | Error ex -> Error ex

module Read =

    let rec bytes (stream: Storage) =
        async {
            try
                if not stream.CanRead then
                    do! Async.Sleep 500
                    return! bytes stream
                else
                    let data = Array.zeroCreate<byte> (int stream.Length)
                    let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask

                    stream.Close()
                    stream.Dispose()

                    match data.Length with
                    | 0 -> return Ok None
                    | _ -> return Ok(Some data)

            with ex ->
                stream.Close()
                stream.Dispose()

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let rec string (stream: Storage) =
        async {
            try
                if not stream.CanRead then
                    do! Async.Sleep 500
                    return! string stream
                else
                    let sr = new StreamReader(stream)
                    let! data = sr.ReadToEndAsync() |> Async.AwaitTask

                    return
                        match data.Length with
                        | 0 -> Ok None
                        | _ -> Ok(Some data)
            with ex ->
                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

module Write =
    open System.Text

    let rec bytes data (stream: Storage) =
        async {
            try
                if not stream.CanWrite then
                    do! Async.Sleep 500
                    return! bytes data stream
                else
                    stream.Position <- stream.Length
                    do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
                    do! stream.FlushAsync() |> Async.AwaitTask

                    return Ok()
            with ex ->
                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let rec string data (stream: Storage) =
        async {
            try
                if not stream.CanWrite then
                    do! Async.Sleep 500
                    return! string data stream
                else
                    let buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine)
                    stream.Position <- stream.Length
                    do! stream.WriteAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
                    do! stream.FlushAsync() |> Async.AwaitTask

                    return Ok()
            with ex ->
                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }
