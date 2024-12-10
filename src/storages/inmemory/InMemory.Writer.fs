[<RequireQualifiedAccess>]
module Persistence.InMemory.Write

open Infrastructure.Domain

let string key value (storage: Client) =
    try
        storage.AddOrUpdate(key, value, (fun _ _ -> value)) |> ignore |> Ok
    with ex ->
        Error
        <| Operation
            { Message = ex.Message
              Code = (__SOURCE_DIRECTORY__, __SOURCE_FILE__, __LINE__) |> Line |> Some }
