[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Write

open System.Text
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.FileSystem
open Persistence.Storages.Domain.FileSystem

let private write (data: byte array) (stream: Client) =
    async {
        do! Provider.Semaphore.WaitAsync() |> Async.AwaitTask

        stream.Position <- 0
        stream.SetLength 0

        do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
        do! stream.FlushAsync() |> Async.AwaitTask

        Provider.Semaphore.Release() |> ignore
    }

let bytes (stream: Client) data =
    stream
    |> Provider.createLock
    |> ResultAsync.bindAsync (fun _ ->
        async {
            try
                do! stream |> write data
                return Ok()
            with ex ->
                return
                    Error
                    <| Operation {
                        Message = ex |> Exception.toMessage
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
        })
    |> Async.bind (fun result -> stream |> Provider.releaseLock |> ResultAsync.bind (fun _ -> result))

let string (stream: Client) (data: string) =
    stream
    |> Provider.createLock
    |> ResultAsync.bindAsync (fun _ ->
        async {
            try
                do! stream |> write (data |> Encoding.UTF8.GetBytes)
                return Ok()
            with ex ->
                return
                    Error
                    <| Operation {
                        Message = ex |> Exception.toMessage
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
        })
    |> Async.bind (fun result -> stream |> Provider.releaseLock |> ResultAsync.bind (fun _ -> result))
