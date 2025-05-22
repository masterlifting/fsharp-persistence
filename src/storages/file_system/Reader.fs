[<RequireQualifiedAccess>]
module Persistence.Storages.FileSystem.Read

open System.Text
open Infrastructure.Domain
open Infrastructure.Prelude
open Persistence.Storages.FileSystem
open Persistence.Storages.Domain.FileSystem

let private read (stream: Client) =
    stream
    |> Provider.acquireLock
    |> ResultAsync.bindAsync (fun _ ->
        async {
            try
                stream.Position <- 0
                let data = Array.zeroCreate<byte> (int stream.Length)
                let! _ = stream.ReadAsync(data, 0, data.Length) |> Async.AwaitTask
                return
                    match data.Length with
                    | 0 -> Ok None
                    | _ -> Ok(Some data)
            with ex ->
                return
                    Error
                    <| Operation {
                        Message = "FileSystem.Provider. Failed to read from file. " + (ex |> Exception.toMessage)
                        Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
                    }
        })
    |> Async.apply (stream |> Provider.releaseLock)

let bytes (stream: Client) = stream |> read

let string (stream: Client) =
    stream |> read |> ResultAsync.map (Option.map Encoding.UTF8.GetString)
