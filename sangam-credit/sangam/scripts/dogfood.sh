#!/usr/bin/env bash
set -euo pipefail
CONN="Host=localhost;Port=5434;Database=sangam;Username=sangam;Password=sangam"
echo "1) introspect (expect 11 proofs, 6 Opaque, 0 invented — audit S1 in Domain.fs)"
dotnet canonflow introspect --pg "$CONN" --out generated/
echo "2) diagnose (expect 1 REDUNDANCY on loans.interest_pct <=24; 0 contradictions)"
dotnet canonflow diagnose --pg "$CONN"
echo "3) S1 audit: guarantees row (NULL,NULL) exists in DB — generated proof must admit it"
docker compose exec -T postgres psql -U sangam -d sangam -c "SELECT * FROM guarantees WHERE guarantor_id IS NULL;"
echo "4) apply bidirectional drift"; docker compose exec -T postgres psql -U sangam -d sangam < db/init/03-drift.sql
echo "   drift (expect 2 violations: principal WIDENED, deposit_min NARROWED-with-warning)"
dotnet canonflow drift --pg "$CONN" --expected generated/
