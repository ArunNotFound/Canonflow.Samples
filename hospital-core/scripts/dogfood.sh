#!/usr/bin/env bash
set -euo pipefail

# Spin up Postgres with Hospital Core schema
docker stop hospital_db || true
docker rm hospital_db || true
docker run -d --name hospital_db -p 5436:5432 -e POSTGRES_USER=hospital -e POSTGRES_PASSWORD=hospital -e POSTGRES_DB=hospital -v $(pwd)/db/init:/docker-entrypoint-initdb.d postgres:16-alpine
echo "Waiting for postgres..."
sleep 5

CONN="Host=localhost;Port=5436;Database=hospital;Username=hospital;Password=hospital"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) emit contracts"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts || true

echo "Cleaning up..."
docker stop hospital_db
docker rm hospital_db
