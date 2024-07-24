[<RequireQualifiedAccess>]
module Persistence.Storage.Database

open Infrastructure

module Context =
    let create _ =
        try
            failwith "Database storage is not implemented yet."
        with ex ->
            Error <| NotSupported ex.Message
