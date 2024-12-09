[<AutoOpen>]
module Persistence.DataAccess.Error

open Infrastructure.Domain

module private Code =
    [<Literal>]
    let DELIMITER = ","

    [<Literal>]
    let Line = nameof Line

module private Reason =

    [<Literal>]
    let Operation = nameof Operation

    [<Literal>]
    let Permission = nameof Permission

    [<Literal>]
    let NotFound = nameof NotFound

    [<Literal>]
    let NotSupported = nameof NotSupported

    [<Literal>]
    let NotImplemented = nameof NotImplemented

    [<Literal>]
    let Cancelled = nameof Canceled

type ErrorCode with
    member this.ToEntity() =
        match this with
        | Line(x, y, z) -> [ x; y; z ] |> String.concat Code.DELIMITER

type ErrorEntity() =
    member val Type = System.String.Empty with get, set
    member val Value = System.String.Empty with get, set
    member val CodeType: string option = None with get, set
    member val Code: string option = None with get, set

    member this.ToDomain() =
        match this.Type with
        | Reason.Operation ->
            Error'.Operation
            <| { Message = this.Value
                 Code = code.T
            |> Ok
        | Reason.Permission ->
            Error'.Permission
                { Message = this.Value
                  Code = this.Code }
            |> Ok
        | Reason.NotFound -> Error'.NotFound this.Value |> Ok
        | Reason.NotSupported -> Error'.NotSupported this.Value |> Ok
        | Reason.NotImplemented -> Error'.NotImplemented this.Value |> Ok
        | Reason.Cancelled -> Canceled this.Value |> Ok
        | _ -> this.Type |> Error'.NotSupported |> Result.Error

type Error' with
    member this.toEntity() =
        let result = ErrorEntity()

        match this with
        | Error'.Operation reason ->
            result.Type <- Reason.Operation
            result.Value <- reason.Message
            result.Code <- reason.Code |> Option.map _.Value
        | Error'.Permission reason ->
            result.Type <- Reason.Permission
            result.Value <- reason.Message
            result.Code <- reason.Code |> Option.map _.Value
        | Error'.NotFound src ->
            result.Type <- Reason.NotFound
            result.Value <- src
        | Error'.NotSupported src ->
            result.Type <- Reason.NotSupported
            result.Value <- src
        | Error'.NotImplemented src ->
            result.Type <- Reason.NotImplemented
            result.Value <- src
        | Error'.Canceled src ->
            result.Type <- Reason.Cancelled
            result.Value <- src

        result
