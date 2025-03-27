module Persistence.Storages.Domain.Configuration

open Microsoft.Extensions.Configuration

type Client =
    { Configuration: IConfigurationRoot
      Key: string }

type Connection =
    { Provider: IConfigurationRoot
      SectionName: string }
