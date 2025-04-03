module Persistence.Storages.Domain.Configuration

open Microsoft.Extensions.Configuration

type Client = {
    ReadOnlyStorage: IConfigurationRoot
    Section: string
}

type Connection = {
    Provider: IConfigurationRoot
    Section: string
}
