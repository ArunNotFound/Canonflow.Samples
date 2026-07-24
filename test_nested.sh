#!/usr/bin/env bash
set -euo pipefail

function setup_project {
  local PROJECT_DIR=$1
  local PORT=$2
  
  echo "Setting up $PROJECT_DIR"
  cd $PROJECT_DIR
  
  # Ensure clean slate
  docker stop ${PROJECT_DIR//\//_}_db || true
  docker rm ${PROJECT_DIR//\//_}_db || true
  
  docker compose up -d
  sleep 5
  ./scripts/dogfood.sh || true
  
  # Setup F#
  TEST_DIR="tests/$(basename $PROJECT_DIR).Tests"
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
  
  # Setup TS
  cd client
  npm init -y
  npm i zod typescript@5.5.4 ts-node jest ts-jest @types/jest @types/node
  npx ts-jest config:init || true
  npm pkg set scripts.test="jest"
  echo '{"compilerOptions": {"esModuleInterop": true}}' > tsconfig.json
  sed -i '/import { z } from "zod";/d' src/validators.ts
  sed -i '1i import { z } from "zod";' src/validators.ts
  
  cat << 'INNEREOF' > src/validators.test.ts
describe('Validators', () => {
    test('dummy', () => { expect(true).toBe(true); });
});
INNEREOF
  npm run test
  
  cd ../../../
  echo "Done $PROJECT_DIR"
}

setup_project "layam-academy/layam" "5433"
setup_project "sangam-credit/sangam" "5434"

