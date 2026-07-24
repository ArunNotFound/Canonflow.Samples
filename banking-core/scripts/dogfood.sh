#!/usr/bin/env bash
set -euo pipefail

# Spin up Postgres with Banking Core schema
docker stop banking_db || true
docker rm banking_db || true
docker run -d --name banking_db -p 5435:5432 -e POSTGRES_USER=banking -e POSTGRES_PASSWORD=banking -e POSTGRES_DB=banking -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5435;Database=banking;Username=banking;Password=banking"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true

echo "Cleaning up..."
docker stop banking_db
docker rm banking_db
