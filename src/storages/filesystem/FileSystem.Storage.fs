[<RequireQualifiedAccess>]
module Persistence.FileSystem.Storage

open System.IO
open Infrastructure.Domain
open Infrastructure.Prelude

let private storages = ClientFactory()

let internal createSource (filePath, fileName) =
    try
        Path.Combine(filePath, fileName) |> Ok
    with ex ->
        Error <| NotSupported(ex |> Exception.toMessage)

let init file =
    let initialize () =
        try
            let client =
                new Client(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)

            Ok client
        with ex ->
            ex |> Exception.toMessage |> NotSupported |> Error

    match storages.TryGetValue file with
    | true, storage -> Ok storage
    | false, _ ->
        match initialize () with
        | Ok storage ->
            storages.TryAdd(file, storage) |> ignore
            Ok storage
        | Error ex -> Error ex

[<Literal>]
let private lockExt = ".lock"

let internal Semaphore = new System.Threading.SemaphoreSlim(1, 1)

let internal createLock (stream: Client) =

    let lockFile = stream.Name + lockExt

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation
                        { Message = $"Failed to acquire lock after {attempts} attempts."
                          Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
            else
                try
                    if lockFile |> File.Exists then
                        do! Async.Sleep delay
                        return! innerLoop (attempts - 1) (delay * 2)
                    else
                        return File.Create(lockFile).Dispose() |> Ok
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100

let internal releaseLock (stream: Client) =
    let lockFile = stream.Name + lockExt

    let rec innerLoop attempts (delay: int) =
        async {

            if attempts <= 0 then
                return
                    Error
                    <| Operation
                        { Message = $"Failed to release lock after {attempts} attempts."
                          Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
            else
                try
                    if lockFile |> File.Exists then
                        return File.Delete(lockFile) |> Ok
                    else
                        return Ok()
                with _ ->
                    do! Async.Sleep delay
                    return! innerLoop (attempts - 1) (delay * 2)
        }

    innerLoop 10 100
