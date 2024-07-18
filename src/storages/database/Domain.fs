module Persistence.Domain.Database

type Type =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Context = { f: string -> string }