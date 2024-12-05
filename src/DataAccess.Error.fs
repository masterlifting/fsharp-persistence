[<AutoOpen>]
module Persistence.DataAccess.Error

open Infrastructure

[<Literal>]
let private Operation = nameof Operation

[<Literal>]
let private Permission = nameof Permission

[<Literal>]
let private NotFound = nameof NotFound

[<Literal>]
let private NotSupported = nameof NotSupported

[<Literal>]
let private NotImplemented = nameof NotImplemented

[<Literal>]
let private Cancelled = nameof Canceled

type Error() =
    member val Type = System.String.Empty with get, set
    member val Value = System.String.Empty with get, set
    member val Code: string option = None with get, set

    member this.ToDomain() =
        match this.Type with
        | Operation ->
            Errors.Operation
                { Message = this.Value
                  Code = this.Code }
            |> Ok
        | Permission ->
            Errors.Permission
                { Message = this.Value
                  Code = this.Code }
            |> Ok
        | NotFound -> Errors.NotFound this.Value |> Ok
        | NotSupported -> Errors.NotSupported this.Value |> Ok
        | NotImplemented -> Errors.NotImplemented this.Value |> Ok
        | Cancelled -> Canceled this.Value |> Ok
        | _ -> this.Type |> Errors.NotSupported |> Result.Error

type Error' with
    member this.toEntity() =
        let result = Error()

        match this with
        | Errors.Operation reason ->
            result.Type <- Operation
            result.Value <- reason.Message
            result.Code <- reason.Code
        | Errors.Permission reason ->
            result.Type <- Permission
            result.Value <- reason.Message
            result.Code <- reason.Code
        | Errors.NotFound src ->
            result.Type <- NotFound
            result.Value <- src
        | Errors.NotSupported src ->
            result.Type <- NotSupported
            result.Value <- src
        | Errors.NotImplemented src ->
            result.Type <- NotImplemented
            result.Value <- src
        | Canceled src ->
            result.Type <- Cancelled
            result.Value <- src

        result
