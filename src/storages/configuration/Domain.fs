[<AutoOpen>]
module Persistence.Configuration.Domain

open Microsoft.Extensions.Configuration

type Client =
    { SectionName: string
      Configuration: IConfigurationRoot }
