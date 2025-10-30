module Persistence.Storages.Domain.FileSystem

open System
open Persistence.Domain

type Client = {
    Stream: IO.FileStream
    Lifetime: Lifetime
}

type Connection = {
    FilePath: string
    FileName: string
    Lifetime: Lifetime
}
