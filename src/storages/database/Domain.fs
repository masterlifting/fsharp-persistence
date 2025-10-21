module Persistence.Storages.Domain.Database

open Persistence.Storages.Domain
open Persistence.Domain

type DatabaseType =
    | Postgre of connectionString: string
    | MongoDb of connectionString: string
    | Redis of connectionString: string

type Client = Postgre of Postgre.Client

type Connection = {
    Database: DatabaseType
    Lifetime: Lifetime
}
