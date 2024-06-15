module Persistence.Core

open Infrastructure.Domain.Errors
open Persistence.Storage
open Domain

let createStorage storageType =
    match storageType with
    | FileSystem path ->
        FileSystem.create path
        |> Result.map FileSystemContext
        |> Result.mapError Persistence
    | InMemory -> 
        InMemory.create () 
        |> Result.map InMemoryContext
        |> Result.mapError Persistence 
    | Database connectionString ->
        Database.create connectionString
        |> Result.map DatabaseContext
        |> Result.mapError Persistence
