module Persistence.Storages.Domain.Database

type DatabaseType =
    | Postgres of connectionString: string
    | MongoDb of connectionString: string

type Client = { f: string -> string }

type Connection = { Database: DatabaseType }
