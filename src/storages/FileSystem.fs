module Persistence.Storage.FileSystem

open System.IO
open System.Text
open System
open Infrastructure.Domain.Errors

type Context = FileStream

let create path =
    try
        let context = new Context(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
        Ok context
    with ex ->
        Error <| Persistence ex.Message

let private writeLine data (stream: Context) =
    async {
        try
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

            return Error <| Persistence ex.Message
    }

let private readLines (stream: Context) =
    async {
        try
            let sb = new StringBuilder()
            let sr = new StreamReader(stream)

            while not sr.EndOfStream do
                let! line = sr.ReadLineAsync() |> Async.AwaitTask

                match String.IsNullOrWhiteSpace line with
                | false -> sb.AppendLine line |> ignore
                | true -> ()

            stream.Close()
            stream.Dispose()

            return Ok <| sb.ToString()
        with ex ->
            stream.Close()
            stream.Dispose()

            return Error <| Persistence ex.Message
    }

let add = writeLine

let get = readLines
