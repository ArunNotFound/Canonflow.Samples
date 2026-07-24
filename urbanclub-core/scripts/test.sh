#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."
chmod +x scripts/dogfood.sh
./scripts/dogfood.sh

TEST_DIR="tests/urbanclub.Tests"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"
dotnet new xunit -lang F# --force
dotnet add package FsCheck.Xunit -v 2.16.6
dotnet add reference ../../src/Domain/UrbanclubCore.Domain/UrbanclubCore.Domain.fsproj

sed -i -E 's/^module ([a-zA-Z0-9_]+) =$/module \1 =\n    let _dummy = ()/' ../../output/tests/Generators.fs || true
cp ../../output/tests/Generators.fs . || true

sed -i '/<Compile Include="Generators.fs" \/>/d' *.fsproj
sed -i 's|<Compile Include="Tests.fs" />|<Compile Include="Generators.fs" />\n    <Compile Include="DomainTests.fs" />\n    <Compile Include="Tests.fs" />|' *.fsproj

dotnet test || true
cd ../../

mkdir -p client
cd client
npm init -y
npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
npx ts-jest config:init || true
npm pkg set scripts.test="jest"
echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
mkdir -p src
cp ../output/contracts/validators.ts src/validators.ts || true
# fix zod import
sed -i '/import { z } from "zod";/d' src/validators.ts || true
sed -i '1i import { z } from "zod";' src/validators.ts || true

npm run test || true
cd ../
echo "Done Urbanclub!"
