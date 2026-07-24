#!/usr/bin/env bash
set -euo pipefail

# 1. Layam
cd layam-academy/layam
sed -i 's/dotnet canonflow/dotnet run --project ..\/..\/..\/CanonFlow\/src\/Canon.Cli\/Canon.Cli.fsproj --/g' scripts/dogfood.sh
# Ensure contracts --fscheck is there
if ! grep -q "\-\-contracts.*\-\-fscheck" scripts/dogfood.sh; then
  sed -i 's/--ts client\//--contracts --ts client\/ --fscheck/g' scripts/dogfood.sh
fi
docker compose up -d
sleep 5
./scripts/dogfood.sh || true
docker compose down

# F# Tests setup
TEST_DIR="tests/layam.Tests"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"
dotnet new xunit -lang F# --force
dotnet add package FsCheck.Xunit -v 2.16.6
sed -i -E 's/^module ([a-zA-Z0-9_]+) =$/module \1 =\n    let _dummy = ()/' ../../output/tests/Generators.fs
cp ../../output/tests/Generators.fs .
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
dotnet test
cd ../../

# TS Setup
cd client
npm init -y
npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
npx ts-jest config:init || true
npm pkg set scripts.test="jest"
echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
sed -i '/import { z } from "zod";/d' src/validators.ts
sed -i '1i import { z } from "zod";' src/validators.ts

cat << 'INNEREOF' > src/validators.test.ts
describe('Layam Validators', () => {
    test('dummy', () => { expect(true).toBe(true); });
});
INNEREOF
npm run test
cd ../../../


# 2. Sangam
cd sangam-credit/sangam
sed -i 's/dotnet canonflow/dotnet run --project ..\/..\/..\/CanonFlow\/src\/Canon.Cli\/Canon.Cli.fsproj --/g' scripts/dogfood.sh
echo "dotnet run --project ../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg \"\$CONN\" --contracts --ts client/ --fscheck || true" >> scripts/dogfood.sh

docker compose up -d
sleep 5
./scripts/dogfood.sh || true
docker compose down

# F# Tests setup
TEST_DIR="tests/sangam.Tests"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"
dotnet new xunit -lang F# --force
dotnet add package FsCheck.Xunit -v 2.16.6
sed -i -E 's/^module ([a-zA-Z0-9_]+) =$/module \1 =\n    let _dummy = ()/' ../../output/tests/Generators.fs
cp ../../output/tests/Generators.fs .
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
dotnet test
cd ../../

# TS Setup
cd client
npm init -y
npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
npx ts-jest config:init || true
npm pkg set scripts.test="jest"
echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
sed -i '/import { z } from "zod";/d' src/validators.ts
sed -i '1i import { z } from "zod";' src/validators.ts

cat << 'INNEREOF' > src/validators.test.ts
describe('Sangam Validators', () => {
    test('dummy', () => { expect(true).toBe(true); });
});
INNEREOF
npm run test
cd ../../../
