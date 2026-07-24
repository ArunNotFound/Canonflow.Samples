#!/usr/bin/env bash
set -euo pipefail
CONN="Host=localhost;Port=5434;Database=sangam;Username=sangam;Password=sangam"
echo "1) introspect"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN"
echo "2) diagnose"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true
echo "3) S1 audit"
docker compose exec -T postgres psql -U sangam -d sangam -c "SELECT * FROM guarantees WHERE guarantor_id IS NULL;" || true
echo "4) contracts"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true
echo "5) apply bidirectional drift"
docker compose exec -T postgres psql -U sangam -d sangam < db/init/03-drift.sql || true
echo "   drift diagnosis"
dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --diagnose || true
