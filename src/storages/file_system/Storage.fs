[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System.Threading
open System.Text
open System.IO
open Infrastructure
open Persistence.FileSystem.Domain

let private storages = StorageFactory()
let private semaphor = new SemaphoreSlim(1, 1)

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

    let bytes (stream: Storage) =
        async {
            try
                do! semaphor.WaitAsync() |> Async.AwaitTask

                stream.Position <- 0

                let data = Array.zeroCreate<byte> (int stream.Length)
                let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask

                semaphor.Release() |> ignore

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

    let string (stream: Storage) =
        async {
            try
                do! semaphor.WaitAsync() |> Async.AwaitTask

                stream.Position <- 0

                let buffer = Array.zeroCreate<byte> (int stream.Length)
                let! _ = stream.ReadAsync(buffer, 0, buffer.Length) |> Async.AwaitTask

                semaphor.Release() |> ignore

                let data = buffer |> Encoding.UTF8.GetString

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

    let bytes (stream: Storage) data =
        async {
            try
                do! semaphor.WaitAsync() |> Async.AwaitTask

                stream.Position <- 0
                stream.SetLength 0

                do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
                do! stream.FlushAsync() |> Async.AwaitTask

                semaphor.Release() |> ignore

                return Ok()
            with ex ->
                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let string (stream: Storage) (data: string) =
        async {
            try
                let buffer = data |> Encoding.UTF8.GetBytes

                do! semaphor.WaitAsync() |> Async.AwaitTask

                stream.Position <- 0
                stream.SetLength 0

                do! stream.WriteAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
                do! stream.FlushAsync() |> Async.AwaitTask

                semaphor.Release() |> ignore

                return Ok()
            with ex ->
                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLineOpt (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }
