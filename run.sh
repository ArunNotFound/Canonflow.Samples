#!/bin/bash
set -e

echo "Starting Postgres container for Fintech Wallet..."
docker run --name canon-fintech-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=fintech_db -p 5433:5432 -d postgres:15-alpine > /dev/null

echo "Waiting for Postgres to be ready..."
sleep 3
until docker exec canon-fintech-db pg_isready -U postgres > /dev/null 2>&1; do
  sleep 1
done

echo "Applying schema.sql..."
docker exec -i canon-fintech-db psql -U postgres -d fintech_db < schema.sql

echo "Running CanonFlow CLI against the database..."
cd ../CanonFlow
dotnet run --project src/Canon.Cli/Canon.Cli.fsproj -- --pg "Host=localhost;Port=5433;Database=fintech_db;Username=postgres;Password=postgres" --contracts

echo "Copying generated artifacts to Canonflow.Samples..."
cp -r output/* ../Canonflow.Samples/

echo "Cleaning up..."
docker stop canon-fintech-db > /dev/null
docker rm canon-fintech-db > /dev/null
echo "Done!"
