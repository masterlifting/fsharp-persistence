[<AutoOpen>]
module Persistence.Configuration.Domain

open Microsoft.Extensions.Configuration

type Client =
    { Configuration: IConfigurationRoot
      Key: string }

type Connection =
    { Configuration: IConfigurationRoot
      SectionName: string }
