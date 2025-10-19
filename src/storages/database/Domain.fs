module Persistence.Storages.Domain.Database

open Persistence.Storages.Domain

type DatabaseType =
    | Postgre of connectionString: string
    | MongoDb of connectionString: string
    | Redis of connectionString: string

type ConnectionType =
    | Transient
    | Singleton

type Client =
    | Postgre of Postgre.Client

type Connection = { Database: DatabaseType; Type: ConnectionType }
