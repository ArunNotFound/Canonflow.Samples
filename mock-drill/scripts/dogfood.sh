#!/usr/bin/env bash
set -euo pipefail

docker stop drill_db || true
docker rm drill_db || true
docker run -d --name drill_db -p 5439:5432 -e POSTGRES_USER=drill -e POSTGRES_PASSWORD=drill -e POSTGRES_DB=drill -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5439;Database=drill;Username=drill;Password=drill"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts || true

echo "Cleaning up..."
docker stop drill_db
docker rm drill_db
