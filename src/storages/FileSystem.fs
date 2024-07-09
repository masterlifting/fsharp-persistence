module Persistence.Storage.FileSystem

open System.IO
open System.Text
open System
open Infrastructure.Domain.Errors

type Context = FileStream

let create path =
    try
        let context = new Context(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
        Ok context
    with ex ->
        Error <| NotSupported ex.Message

let rec private writeLine data (stream: Context) =
    async {
        try
            if not stream.CanWrite then
                do! Async.Sleep 500
                return! writeLine data stream
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

            return Error <| Operation { Message = ex.Message; Code = None }
    }

let rec private readLines (stream: Context) =
    async {
        try
            if not stream.CanRead then
                do! Async.Sleep 500
                return! readLines stream
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

            return Error <| Operation { Message = ex.Message; Code = None }
    }

let add = writeLine

let get = readLines
