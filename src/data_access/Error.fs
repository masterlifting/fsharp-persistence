[<AutoOpen>]
module Persistence.DataAccess.Error

open Infrastructure.Domain

module private Code =
    [<Literal>]
    let DELIMITER = ","

    [<Literal>]
    let LINE = nameof Line

    [<Literal>]
    let HTTP = nameof Http

    [<Literal>]
    let CUSTOM = nameof Custom

module private Reason =

    [<Literal>]
    let OPERATION = nameof Operation

    [<Literal>]
    let PERMISSION = nameof Permission

    [<Literal>]
    let ALREADY_EXISTS = nameof AlreadyExists

    [<Literal>]
    let NOT_FOUND = nameof NotFound

    [<Literal>]
    let NOT_SUPPORTED = nameof NotSupported

    [<Literal>]
    let NOT_IMPLEMENTED = nameof NotImplemented

    [<Literal>]
    let CANCELED = nameof Canceled

type private ErrorCode with
    member this.ToValue() =
        match this with
        | Line(x, y, z) -> [ (nameof Line); x; y; z ] |> String.concat Code.DELIMITER
        | Http code -> [ nameof Http; code |> System.Enum.GetName ] |> String.concat Code.DELIMITER
        | Custom value -> [ nameof Custom; value ] |> String.concat Code.DELIMITER

type ErrorEntity() =
    member val Type = System.String.Empty with get, set
    member val Value = System.String.Empty with get, set
    member val Code: string option = None with get, set

    member private this.ToErrorCode() =
        match this.Code with
        | Some code ->
            let args = code.Split Code.DELIMITER

            match args.Length with
            | 4 ->
                match args[0] with
                | Code.LINE -> ErrorCode.Line(args[1], args[2], args[3]) |> Some |> Ok
                | Code.HTTP ->
                    args[1]
                    |> System.Enum.Parse<System.Net.HttpStatusCode>
                    |> ErrorCode.Http
                    |> Some
                    |> Ok
                | Code.CUSTOM -> args[1] |> ErrorCode.Custom |> Some |> Ok
                | _ -> code |> NotSupported |> Result.Error
            | _ -> code |> NotSupported |> Result.Error
        | None -> None |> Ok

    member this.ToDomain() =
        this.ToErrorCode()
        |> Result.bind (fun code ->
            match this.Type with
            | Reason.OPERATION -> Error'.Operation { Message = this.Value; Code = code } |> Ok
            | Reason.PERMISSION -> Error'.Permission { Message = this.Value; Code = code } |> Ok
            | Reason.ALREADY_EXISTS -> Error'.AlreadyExists this.Value |> Ok
            | Reason.NOT_FOUND -> Error'.NotFound this.Value |> Ok
            | Reason.NOT_SUPPORTED -> Error'.NotSupported this.Value |> Ok
            | Reason.NOT_IMPLEMENTED -> Error'.NotImplemented this.Value |> Ok
            | Reason.CANCELED -> Error'.Canceled this.Value |> Ok
            | _ -> this.Type |> Error'.NotSupported |> Result.Error)

type Error' with
    member this.ToEntity() =
        let result = ErrorEntity()

        match this with
        | Error'.Operation reason ->
            result.Type <- Reason.OPERATION
            result.Value <- reason.Message
            result.Code <- reason.Code |> Option.map _.ToValue()
        | Error'.Permission reason ->
            result.Type <- Reason.PERMISSION
            result.Value <- reason.Message
            result.Code <- reason.Code |> Option.map _.ToValue()
        | Error'.AlreadyExists src ->
            result.Type <- Reason.ALREADY_EXISTS
            result.Value <- src
        | Error'.NotFound src ->
            result.Type <- Reason.NOT_FOUND
            result.Value <- src
        | Error'.NotSupported src ->
            result.Type <- Reason.NOT_SUPPORTED
            result.Value <- src
        | Error'.NotImplemented src ->
            result.Type <- Reason.NOT_IMPLEMENTED
            result.Value <- src
        | Error'.Canceled src ->
            result.Type <- Reason.CANCELED
            result.Value <- src

        result
