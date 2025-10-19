[<RequireQualifiedAccess>]
module Persistence.Storages.Postgre.Command

open Infrastructure.SerDe
open Persistence.Storages.Domain.Postgre

module Json =
    let insert<'a> (tableName: string) (data: 'a array) (client: Client) =
        async {
            let commands =
                data
                |> Array.map (fun item -> 
                    let sql = $"INSERT INTO {tableName} VALUES (@item)"
                    (sql, box {| item = item |}))
                |> Array.toList
            
            return! Write.transaction commands client
        }
    
    let update<'a> (tableName: string) (data: 'a array) (whereClause: 'a -> string) (client: Client) =
        async {
            let commands =
                data
                |> Array.map (fun item ->
                    let where = whereClause item
                    let sql = $"UPDATE {tableName} SET data = @item WHERE {where}"
                    (sql, box {| item = item |}))
                |> Array.toList
            
            return! Write.transaction commands client
        }
    
    let delete (tableName: string) (whereClause: string) (parameters: obj) (client: Client) =
        async {
            let sql = $"DELETE FROM {tableName} WHERE {whereClause}"
            return! Write.executeWithParams sql parameters client
        }
    
    let save<'a> (tableName: string) (data: 'a array) (client: Client) =
        insert tableName data client

    let bulkInsert<'a> (tableName: string) (data: 'a seq) (client: Client) =
        async {
            let sql = $"INSERT INTO {tableName} VALUES (@data)"
            return! Write.executeWithParams sql (box {| data = data |}) client
        }

    let bulkUpdate<'a> (tableName: string) (data: 'a seq) (keyColumn: string) (client: Client) =
        async {
            // Using temporary table approach for bulk updates
            let guidStr = System.Guid.NewGuid().ToString("N")
            let tempTable = $"{tableName}_temp_{guidStr}"
            let commands = [
                ($"CREATE TEMP TABLE {tempTable} AS SELECT * FROM {tableName} LIMIT 0", box null)
                ($"INSERT INTO {tempTable} SELECT * FROM UNNEST(@data)", box {| data = data |})
                ($"UPDATE {tableName} t SET data = tmp.data FROM {tempTable} tmp WHERE t.{keyColumn} = tmp.{keyColumn}", box null)
                ($"DROP TABLE {tempTable}", box null)
            ]
            
            return! Write.transaction commands client
        }
