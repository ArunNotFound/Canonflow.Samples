#r "nuget: Npgsql, 8.0.3"

open Npgsql
open System

let connStr = "Host=localhost;Database=fintech;Username=postgres;Password=password"
use conn = new NpgsqlConnection(connStr)
conn.Open()

let colQuery = @"
    SELECT 
        c.table_schema, c.table_name, c.column_name, c.data_type, c.is_nullable, c.domain_name,
        (SELECT string_agg(def, '|||') FROM (
            SELECT pg_get_constraintdef(con.oid) as def
            FROM pg_constraint con
            INNER JOIN pg_attribute a ON a.attnum = ANY(con.conkey) AND a.attrelid = con.conrelid
            WHERE con.conrelid = (c.table_schema || '.' || c.table_name)::regclass
              AND a.attname = c.column_name AND con.contype = 'c'
            UNION ALL
            SELECT pg_get_constraintdef(con.oid) as def
            FROM pg_constraint con
            JOIN pg_type t ON t.oid = con.contypid
            JOIN pg_namespace n ON n.oid = t.typnamespace
            WHERE t.typname = c.domain_name AND n.nspname = c.domain_schema AND c.domain_name IS NOT NULL
        ) sub) as check_constraints
    FROM information_schema.columns c
    WHERE c.table_schema = 'public' AND c.table_name = 'members'
    ORDER BY c.table_schema, c.table_name, c.ordinal_position;
"
use cmd = new NpgsqlCommand(colQuery, conn)
use reader = cmd.ExecuteReader()
while reader.Read() do
    let cname = reader.GetString(2)
    let checks = if reader.IsDBNull(6) then "" else reader.GetString(6)
    printfn "%s -> %s" cname checks
