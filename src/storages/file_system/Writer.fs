[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Write

open System.Text
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.FileSystem
open Persistence.Storages.Domain.FileSystem

let private write (data: byte array) (client: Client) =
    client
    |> Provider.acquireLock
    |> ResultAsync.bindAsync (fun _ ->
        async {
            try
                client.Stream.Position <- 0
                client.Stream.SetLength 0
                do! client.Stream.WriteAsync(data, 0, data.Length) |> Async.AwaitTask
                do! client.Stream.FlushAsync() |> Async.AwaitTask
                return Ok()
            with ex ->
                return
                    Error
                    <| Operation {
                        Message = "FileSystem.Provider. Failed to write to file. " + (ex |> Exception.toMessage)
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
        })
    |> ResultAsync.apply (client |> Provider.releaseLock)

let bytes (client: Client) data = client |> write data

let string (client: Client) (data: string) =
    client |> write (data |> Encoding.UTF8.GetBytes)
