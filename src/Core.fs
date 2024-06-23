module Persistence.Core

open Infrastructure.Domain.Errors
open Persistence.Storage
open Domain

let createStorage ``type`` =
    match ``type`` with
    | FileSystem path ->
        FileSystem.create path
        |> Result.map FileSystemContext
        |> Result.mapError PersistenceError
    | InMemory -> 
        InMemory.create () 
        |> Result.map InMemoryContext
        |> Result.mapError PersistenceError 
    | Database connectionString ->
        Database.create connectionString
        |> Result.map DatabaseContext
        |> Result.mapError PersistenceError
