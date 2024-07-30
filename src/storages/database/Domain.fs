module Persistence.Database.Domain

type Type =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Storage = { f: string -> string }
