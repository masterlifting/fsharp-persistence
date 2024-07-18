module Persistence.Domain.Database

type Type =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Storage = { f: string -> string }