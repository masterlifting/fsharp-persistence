module Persistence.Core

open Infrastructure.Domain.Errors
open Persistence.Storage
open Domain

let createStorage storageType =
    match storageType with
    | FileSystem path ->
        FileSystem.create path
        |> Result.mapError Persistence
        |> Result.map FileSystemContext
    | InMemory -> 
        InMemory.create () 
        |> Result.mapError Persistence 
        |> Result.map InMemoryContext
    | Database connectionString ->
        Database.create connectionString
        |> Result.mapError Persistence
        |> Result.map DatabaseContext
