module Persistence.Storages.Domain.Postgre

open Npgsql
open Persistence.Domain

type Client = {
    Connection: NpgsqlConnection
    Lifetime: Lifetime
}
type Connection = { String: string; Lifetime: Lifetime }
type Request = { Sql: string; Params: obj option }
