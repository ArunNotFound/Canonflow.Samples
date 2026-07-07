#!/usr/bin/env bash
# Layam dogfood run — each step's expectation is in README pass criteria.
set -euo pipefail
CONN="Host=localhost;Port=5433;Database=layam;Username=layam;Password=layam"
echo "1) introspect";  dotnet canonflow introspect --pg "$CONN" --out generated/
echo "2) diagnose (expect 1 contradiction: scholarships)"; dotnet canonflow diagnose --pg "$CONN"
echo "3) contracts";   dotnet canonflow contracts --ts client/
echo "4) agreement spot-probes"; node scripts/agreement-spot.js
echo "5) apply drift"; docker compose exec -T postgres psql -U layam -d layam < db/init/03-drift.sql
echo "   drift (expect 1 violation: batches.fee_monthly)"; dotnet canonflow drift --pg "$CONN" --expected generated/
echo "6) opensearch fidelity"; dotnet canonflow emit --target opensearch --pg "$CONN"
