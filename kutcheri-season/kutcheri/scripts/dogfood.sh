#!/usr/bin/env bash
set -euo pipefail

# Spin up Postgres with Kutcheri schema
docker stop kutcheri_db || true
docker rm kutcheri_db || true
docker run -d --name kutcheri_db -p 5434:5432 -e POSTGRES_USER=kutcheri -e POSTGRES_PASSWORD=kutcheri -e POSTGRES_DB=kutcheri -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5434;Database=kutcheri;Username=kutcheri;Password=kutcheri"

echo "1) introspect"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts || true

echo "Cleaning up..."
docker stop kutcheri_db
docker rm kutcheri_db
