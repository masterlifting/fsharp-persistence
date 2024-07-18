[<RequireQualifiedAccess>]
module Persistence.Storage.Database

open Persistence.Domain.Database
open Infrastructure.Domain.Errors

module Context =
    let create connectionString =
        try
            failwith "Database storage is not implemented yet."
        with ex ->
            Error <| NotSupported ex.Message
