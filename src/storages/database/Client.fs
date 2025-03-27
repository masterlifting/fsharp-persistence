[<RequireQualifiedAccess>]
module Persistence.Storages.Database.Client

open Infrastructure.Domain

let init _ =
    try
        failwith "Database storage is not implemented yet."
    with ex ->
        Error <| NotSupported ex.Message
