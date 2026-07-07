#!/usr/bin/env bash
set -euo pipefail

docker stop trade_db || true
docker rm trade_db || true
docker run -d --name trade_db -p 5438:5432 -e POSTGRES_USER=trade -e POSTGRES_PASSWORD=trade -e POSTGRES_DB=trade -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5438;Database=trade;Username=trade;Password=trade"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts || true

echo "Cleaning up..."
docker stop trade_db
docker rm trade_db
