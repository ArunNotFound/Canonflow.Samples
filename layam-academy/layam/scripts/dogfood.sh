#!/usr/bin/env bash
set -euo pipefail
CONN="Host=localhost;Port=5433;Database=layam;Username=layam;Password=layam"
echo "1) introspect"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"
echo "2) diagnose"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true
echo "3) contracts"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true
echo "4) apply drift"
docker compose exec -T postgres psql -U layam -d layam < db/init/03-drift.sql || true
echo "   drift diagnosis"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true
