[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Read

open System.Text
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.FileSystem
open Persistence.Storages.Domain.FileSystem

let private read (stream: Client) =
    async {
        do! Client.Semaphore.WaitAsync() |> Async.AwaitTask

        stream.Position <- 0
        let data = Array.zeroCreate<byte> (int stream.Length)
        let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask

        Client.Semaphore.Release() |> ignore

        return data
    }

let bytes (stream: Client) =
    stream
    |> Client.createLock
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
                          Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
        })
    |> Async.bind (fun result -> stream |> Client.releaseLock |> ResultAsync.bind (fun _ -> result))

let string (stream: Client) =
    stream
    |> Client.createLock
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
                          Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
        })
    |> Async.bind (fun result -> stream |> Client.releaseLock |> ResultAsync.bind (fun _ -> result))
