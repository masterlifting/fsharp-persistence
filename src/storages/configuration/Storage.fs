[<RequireQualifiedAccess>]
module Persistence.Configuration.Storage

let init (connection: Connection) =
    { Configuration = connection.Configuration
      Key = connection.SectionName }
    |> Ok
