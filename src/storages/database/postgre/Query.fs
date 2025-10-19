[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Query

open Infrastructure.SerDe
open Persistence.Storages.Domain.Postgre

module Json =
    let get<'a> (tableName: string) (whereClause: string option) (parameters: obj option) (client: Client) =
        async {
            let sql =
                match whereClause with
                | Some where -> $"SELECT * FROM {tableName} WHERE {where}"
                | None -> $"SELECT * FROM {tableName}"
            
            let! result = 
                match parameters with
                | Some p -> Read.queryTypedWithParams<'a> sql p client
                | None -> Read.queryTyped<'a> sql client
            
            return result
        }
    
    let getOne<'a> (tableName: string) (whereClause: string) (parameters: obj) (client: Client) =
        async {
            let sql = $"SELECT * FROM {tableName} WHERE {whereClause} LIMIT 1"
            
            let! result = Read.queryTypedWithParams<'a> sql parameters client
            
            return
                result
                |> Result.map (fun rows -> rows |> Array.tryHead)
        }
    
    let count (tableName: string) (whereClause: string option) (parameters: obj option) (client: Client) =
        async {
            let sql =
                match whereClause with
                | Some where -> $"SELECT COUNT(*) FROM {tableName} WHERE {where}"
                | None -> $"SELECT COUNT(*) FROM {tableName}"
            
            let! result = 
                match parameters with
                | Some p -> Read.scalarTypedWithParams<int64> sql p client
                | None -> Read.scalarTyped<int64> sql client
            
            return
                result
                |> Result.map (fun value ->
                    match value with
                    | Some v -> v |> int
                    | None -> 0)
        }
    
    let exists (tableName: string) (whereClause: string) (parameters: obj) (client: Client) =
        async {
            let sql = $"SELECT EXISTS(SELECT 1 FROM {tableName} WHERE {whereClause})"
            
            let! result = Read.scalarTypedWithParams<bool> sql parameters client
            
            return
                result
                |> Result.map (fun value ->
                    match value with
                    | Some v -> v
                    | None -> false)
        }

    let executeQuery<'a> (sql: string) (parameters: obj option) (client: Client) =
        async {
            let! result = 
                match parameters with
                | Some p -> Read.queryTypedWithParams<'a> sql p client
                | None -> Read.queryTyped<'a> sql client
            
            return result
        }
