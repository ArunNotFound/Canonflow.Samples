#!/usr/bin/env bash
set -euo pipefail
CONN="Host=localhost;Port=5443;Database=arangetram;Username=arangetram;Password=arangetram"
dotnet run --project ../../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true
