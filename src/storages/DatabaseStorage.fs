module Persistence.DatabaseStorage

type Provider = 
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

let create connectionString =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error ex.Message
