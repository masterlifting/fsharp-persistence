module Persistence.Storages.Domain.Postgre

open Npgsql
open Persistence.Domain

type Client = NpgsqlConnection
type Connection = { String: string; Lifetime: Lifetime }

module Query =
    type Request<'a> = { Sql: string; Params: obj option }
module Command =
    type Request = { Sql: string; Params: obj option }
