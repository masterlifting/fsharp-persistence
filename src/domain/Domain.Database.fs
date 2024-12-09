module Persistence.Domain.Database

type DatabaseType =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Client = { f: string -> string }
