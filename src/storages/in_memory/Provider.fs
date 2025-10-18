[<RequireQualifiedAccess>]
module Persistence.Storages.InMemory.Provider

open System
open System.Collections.Concurrent
open Infrastructure.Domain
open Persistence.Storages.Domain.InMemory

let private storage =
    ConcurrentDictionary<string, string> StringComparer.OrdinalIgnoreCase

let init connection =
    try
        Ok
        <| {
               TableName = connection.TableName
               Storage = storage
           }
    with ex ->
        Error
        <| Operation {
            Message = ex.Message
            Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some
        }
