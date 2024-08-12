[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System
open System.IO
open Infrastructure
open Persistence.FileSystem.Domain

let create path =
    try
        let storage =
            new Storage(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

        Ok storage
    with ex ->
        Error <| NotSupported ex.Message

module Read =
    open System.Text

    let rec bytes (stream: Storage) =
        async {
            try
                if not stream.CanRead then
                    do! Async.Sleep 500
                    return! bytes stream
                else
                    let buffer = Array.zeroCreate<byte> (int stream.Length)
                    let! _ = stream.ReadAsync(buffer, 0, buffer.Length) |> Async.AwaitTask

                    stream.Close()
                    stream.Dispose()

                    return Ok buffer
            with ex ->
                stream.Close()
                stream.Dispose()

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLine (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }

    let rec string (stream: Storage) =
        async {
            try
                if not stream.CanRead then
                    do! Async.Sleep 500
                    return! string stream
                else
                    let sb = StringBuilder()
                    let sr = new StreamReader(stream)

                    while not sr.EndOfStream do
                        let! line = sr.ReadLineAsync() |> Async.AwaitTask

                        match String.IsNullOrWhiteSpace line with
                        | false -> sb.AppendLine line |> ignore
                        | true -> ()

                    stream.Close()
                    stream.Dispose()

                    return
                        match sb.Length with
                        | 0 -> Error <| NotFound stream.Name
                        | _ -> Ok <| sb.ToString()
            with ex ->
                stream.Close()
                stream.Dispose()

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLine (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
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

                    stream.Close()
                    stream.Dispose()

                    return Ok()
            with ex ->
                stream.Close()
                stream.Dispose()

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLine (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
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

                    stream.Close()
                    stream.Dispose()

                    return Ok()
            with ex ->
                stream.Close()
                stream.Dispose()

                return
                    Error
                    <| Operation
                        { Message = ex.Message
                          Code = ErrorReason.buildLine (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) }
        }
