#!/usr/bin/env bash
set -euo pipefail

docker stop airline_db || true
docker rm airline_db || true
docker run -d --name airline_db -p 5437:5432 -e POSTGRES_USER=airline -e POSTGRES_PASSWORD=airline -e POSTGRES_DB=airline -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5437;Database=airline;Username=airline;Password=airline"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts || true

echo "Cleaning up..."
docker stop airline_db
docker rm airline_db
