module Persistence.Storages.Domain.FileSystem

open System

type Client = IO.FileStream
type ConnectionType =
    | Transient
    | Singleton
type Connection = {
    FilePath: string
    FileName: string
    Type: ConnectionType
}
