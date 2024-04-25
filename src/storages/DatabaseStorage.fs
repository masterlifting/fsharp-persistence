module Persistence.DatabaseStorage

type DatabaseStorageType =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Context = { f: string -> string }

let create connectionString =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error ex.Message
