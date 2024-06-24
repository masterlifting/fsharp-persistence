module Persistence.Storage.FileSystem

open System.IO
open System.Text
open System
open Infrastructure.Domain.Errors

type Context = FileStream

let internal create path =
    try
        use context = new Context(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
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
            return Ok()
        with ex ->
            return Error <| Persistence ex.Message
    }

let private readLines number (stream: Context) =
    async {
        try
            let sr = new StreamReader(stream)
            let mutable lines = []
            let mutable i = 0

            while i < number do
                let! line = sr.ReadLineAsync() |> Async.AwaitTask

                match String.IsNullOrWhiteSpace line with
                | false ->
                    lines <- line :: lines
                    i <- i + 1
                | true -> i <- number

            return Ok lines

        with ex ->
            return Error <| Persistence ex.Message
    }


let add = writeLine
