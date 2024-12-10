[<RequireQualifiedAccess>]
module Persistence.Configuration.Storage

open Microsoft.Extensions.Configuration

let internal init section (configuration: IConfigurationRoot) = section, configuration
