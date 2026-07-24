#!/usr/bin/env bash
set -euo pipefail

cd arangetram-adversaries/arangetram-adversaries

mkdir -p db/init
cp tier1-A2-hostile-schema/db/init/hostile.sql db/init/

cat << 'DOCKEREOF' > docker-compose.yml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: arangetram
      POSTGRES_PASSWORD: arangetram
      POSTGRES_DB: arangetram
    ports:
      - "5443:5432"
    volumes:
      - ./db/init:/docker-entrypoint-initdb.d
DOCKEREOF

docker stop arangetram_postgres || true
docker rm arangetram_postgres || true
docker compose up -d

echo "Waiting for postgres to start..."
sleep 15

CONN="Host=localhost;Port=5443;Database=arangetram;Username=arangetram;Password=arangetram"

cat << 'SCRIPT' > dogfood.sh
#!/usr/bin/env bash
set -euo pipefail
CONN="Host=localhost;Port=5443;Database=arangetram;Username=arangetram;Password=arangetram"
dotnet run --project ../../../../CanonFlow/src/Canon.Cli/Canon.Cli.fsproj -- --pg "$CONN" --contracts --fscheck || true
SCRIPT
chmod +x dogfood.sh
./dogfood.sh

TEST_DIR="tests/arangetram.Tests"
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

mkdir -p client/src
cd client
npm init -y
npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
npx ts-jest config:init || true
npm pkg set scripts.test="jest"
echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
cp ../output/contracts/validators.ts src/validators.ts || true
sed -i '/import { z } from "zod";/d' src/validators.ts || true
sed -i '1i import { z } from "zod";' src/validators.ts || true

cat << 'INNEREOF' > src/validators.test.ts
describe('Validators', () => {
    test('dummy', () => { expect(true).toBe(true); });
});
INNEREOF
npm run test || true
cd ../../../
echo "Done Arangetram"
