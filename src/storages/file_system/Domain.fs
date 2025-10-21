module Persistence.Storages.Domain.FileSystem

open System
open Persistence.Domain

type Client = IO.FileStream

type Connection = {
    FilePath: string
    FileName: string
    Lifetime: Lifetime
}
