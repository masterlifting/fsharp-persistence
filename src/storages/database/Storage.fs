[<RequireQualifiedAccess>]
module Persistence.Database.Storage

open Infrastructure.Domain

let init _ =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error <| NotSupported ex.Message
