#!/usr/bin/env bash
set -euo pipefail

# Spin up DB V1
docker stop cf_v1 || true
docker rm cf_v1 || true
docker run -d --name cf_v1 -p 5441:5432 -e POSTGRES_USER=app -e POSTGRES_PASSWORD=app -e POSTGRES_DB=app -v $(pwd)/db/v1.sql:/docker-entrypoint-initdb.d/v1.sql postgres:16-alpine

# Spin up DB V2
docker stop cf_v2 || true
docker rm cf_v2 || true
docker run -d --name cf_v2 -p 5442:5432 -e POSTGRES_USER=app -e POSTGRES_PASSWORD=app -e POSTGRES_DB=app -v $(pwd)/db/v2.sql:/docker-entrypoint-initdb.d/v2.sql postgres:16-alpine

echo "Waiting for postgres instances to initialize..."
sleep 6

CONN_V1="Host=localhost;Port=5441;Database=app;Username=app;Password=app"
CONN_V2="Host=localhost;Port=5442;Database=app;Username=app;Password=app"

echo "Running CanonFlow Migration Engine (V1 -> V2)..."
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN_V1" --migrateto "$CONN_V2"

echo "Cleaning up..."
docker stop cf_v1 cf_v2
docker rm cf_v1 cf_v2
