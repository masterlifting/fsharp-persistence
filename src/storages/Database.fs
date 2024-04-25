module Persistence.Database

type Type =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Storage = { f: string -> string }

let internal create connectionString =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error ex.Message
