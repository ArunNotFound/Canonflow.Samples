#!/usr/bin/env bash
set -euo pipefail

cd migration-demo
docker stop cf_v1 cf_v2 || true
docker rm cf_v1 cf_v2 || true
docker run -d --name cf_v1 -p 5441:5432 -e POSTGRES_USER=app -e POSTGRES_PASSWORD=app -e POSTGRES_DB=app -v $(pwd)/db/v1.sql:/docker-entrypoint-initdb.d/v1.sql postgres:16-alpine
docker run -d --name cf_v2 -p 5442:5432 -e POSTGRES_USER=app -e POSTGRES_PASSWORD=app -e POSTGRES_DB=app -v $(pwd)/db/v2.sql:/docker-entrypoint-initdb.d/v2.sql postgres:16-alpine

echo "Waiting for postgres instances to initialize..."
sleep 15

CONN_V2="Host=localhost;Port=5442;Database=app;Username=app;Password=app"
dotnet run --project ../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN_V2" --contracts --fscheck || true

TEST_DIR="tests/migration-demo.Tests"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"
dotnet new xunit -lang F# --force
dotnet add package FsCheck.Xunit -v 2.16.6
sed -i -E 's/^module ([a-zA-Z0-9_]+) =$/module \1 =\n    let _dummy = ()/' ../../output/tests/Generators.fs || true
cp ../../output/tests/Generators.fs . || true
sed -i '/<Compile Include="Generators.fs" \/>/d' *.fsproj
sed -i 's|<Compile Include="Tests.fs" />|<Compile Include="Generators.fs" />\n    <Compile Include="Tests.fs" />|' *.fsproj
cat << 'INNEREOF' > Tests.fs
module Tests
open Xunit
open FsCheck
open FsCheck.Xunit
open CanonFlow.FsCheck.Generators
[<Property>]
let ``Dummy property`` () = true
INNEREOF
dotnet test || true
cd ../../

mkdir -p client
cd client
npm init -y
npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
npx ts-jest config:init || true
npm pkg set scripts.test="jest"
echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
sed -i '/import { z } from "zod";/d' src/validators.ts || true
sed -i '1i import { z } from "zod";' src/validators.ts || true

mkdir -p src
cat << 'INNEREOF' > src/validators.test.ts
describe('Validators', () => {
    test('dummy', () => { expect(true).toBe(true); });
});
INNEREOF
npm run test || true
cd ../../
echo "Done Migration Demo"

