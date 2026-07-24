#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

echo "Starting Postgres..."
docker compose up -d
sleep 5

# Get the directory name of the sample (e.g. wecar-core)
SAMPLE_DIR=$(basename $(pwd))
# Use connection string from docker-compose mapping 5436
CONN="Host=localhost;Port=5436;Database=wecar;Username=postgres;Password=password"

echo "Running Canon.Cli on $SAMPLE_DIR..."
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true

echo "CanonFlow execution completed!"
docker compose down -v
