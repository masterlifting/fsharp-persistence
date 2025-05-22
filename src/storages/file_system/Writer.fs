[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Write

open System.Text
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.FileSystem
open Persistence.Storages.Domain.FileSystem

let private write (data: byte array) (stream: Client) =
    stream
    |> Provider.acquireLock
    |> ResultAsync.bindAsync (fun _ ->
        async {
            try
                stream.Position <- 0
                stream.SetLength 0
                do! stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
                do! stream.FlushAsync() |> Async.AwaitTask
                return Ok()
            with ex ->
                return
                    Error
                    <| Operation {
                        Message = "FileSystem.Provider. Failed to write to file. " + (ex |> Exception.toMessage)
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
        })
    |> Async.apply (stream |> Provider.releaseLock )

let bytes (stream: Client) data = stream |> write data

let string (stream: Client) (data: string) =
    stream |> write (data |> Encoding.UTF8.GetBytes)
