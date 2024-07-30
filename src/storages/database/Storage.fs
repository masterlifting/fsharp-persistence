[<RequireQualifiedAccess>]
module Persistence.Database.Storage

open Infrastructure

module Context =
    let create _ =
        try
            failwith "Database storage is not implemented yet."
        with ex ->
            Error <| NotSupported ex.Message
