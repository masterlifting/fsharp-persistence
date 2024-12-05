[<RequireQualifiedAccess>]
module Persistence.Configuration.Storage

open Microsoft.Extensions.Configuration

let internal create (configuration: IConfigurationRoot) = configuration
