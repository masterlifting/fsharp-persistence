[<RequireQualifiedAccess>]
module Persistence.Domain.Storage

open Microsoft.Extensions.Configuration

type Storage =
    | FileSystem of FileSystem.Client
    | InMemory of InMemory.Client
    | Database of Database.Client
    | Configuration of sectionName: string * configuration: IConfigurationRoot
