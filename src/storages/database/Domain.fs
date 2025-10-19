module Persistence.Storages.Domain.Database

type DatabaseType =
    | Postgres of connectionString: string
    | MongoDb of connectionString: string
    | Redis of connectionString: string

type Client = unit

type Connection = { Database: DatabaseType }
