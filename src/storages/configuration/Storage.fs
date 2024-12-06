[<RequireQualifiedAccess>]
module Persistence.Configuration.Storage

open Microsoft.Extensions.Configuration

let internal create section (configuration: IConfigurationRoot) = section, configuration
