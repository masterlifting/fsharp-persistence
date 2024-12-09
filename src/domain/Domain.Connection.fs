[<RequireQualifiedAccess>]
module Persistence.Domain.Connection

open Microsoft.Extensions.Configuration

type Connection =
    | FileSystem of FileSystem.Source
    | InMemory
    | Database of string
    | Configuration of sectionName: string * configuration: IConfigurationRoot
