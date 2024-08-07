[<RequireQualifiedAccess>]
module Persistence.Database.Storage

open Infrastructure

let create _ =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error <| NotSupported ex.Message
