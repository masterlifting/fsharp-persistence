module Persistence.DatabaseStorage

open System

type Provider = DbContext // DbContext is a placeholder for a real database context type

let create connectionString =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error ex.Message
