[<AutoOpen>]
module Persistence.Database.Domain

type DatabaseType =
    | SqlServer
    | Postgres
    | MongoDb
    | AzureTable

type Client = { f: string -> string }

type Connection =
    { ConnectionString: string
      Type: DatabaseType }
