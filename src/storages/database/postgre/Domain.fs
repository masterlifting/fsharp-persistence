module Persistence.Storages.Domain.Postgre

open Npgsql

type Client = NpgsqlConnection
type ConnectionType =
    | Transient
    | Singleton
type Connection = {
    String: string
    Type: ConnectionType
}
