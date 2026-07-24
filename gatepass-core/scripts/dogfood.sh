#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

# Start Postgres
docker stop gatepass_postgres || true
docker rm gatepass_postgres || true
docker compose up -d

echo "Waiting 10s for Postgres..."
sleep 10

CONN="Host=localhost;Port=5444;Database=gatepass;Username=gatepass;Password=gatepass"

echo "1) introspect"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"

echo "2) diagnose"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true

echo "3) contracts (FSA emission)"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true
