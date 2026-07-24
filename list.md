# CanonFlow Samples Implementation Checklist

This document tracks the tasks required to fully verify each sample using CanonFlow's contract emissions.

## Rules / Requirements for each sample:
1. **Dogfood Update**: Update the sample's `dogfood.sh` script to include the `--fscheck` flag in the `Canon.Cli` emission step.
2. **FSA (FsCheck Arbitraries)**: Ensure `output/tests/Generators.fs` is successfully emitted by the CanonFlow pipeline.
3. **F# Test Coverage**: Create an F# xUnit test project (`tests/<Sample>.Tests`), reference `FsCheck.Xunit`, and write property-based tests proving the Arbitraries adhere to PostgreSQL constraints.
4. **TypeScript Test Coverage**: Initialize a Jest test suite in `client/` and write unit tests for the Zod validators (`validators.test.ts`).
5. **TS Import Fix**: Ensure the duplicate `import { z } from "zod";` lines are removed from `client/src/validators.ts`.

## Checklist

### 1. Mock Drill (`mock-drill`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 2. Kutcheri Season (`kutcheri-season`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 3. Banking Core (`banking-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 4. Hospital Core (`hospital-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 5. Airline Core (`airline-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 6. Trading Core (`trading-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 7. Migration Engine Demo (`migration-demo`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 8. Arangetram Adversaries (`arangetram-adversaries`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 9. Layam Academy (`layam-academy`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 10. Sangam Credit (`sangam-credit`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 11. Gatepass App (`gatepass-core`)
- [x] Generated `--fscheck` (FSA) in dogfood.sh
- [x] F# Tests (FsCheck property tests added for Resident/Visitor flows)
- [x] TypeScript / Jest tests added
- [x] Duplicate Zod imports removed

### 12. Wecar App (`wecar-core`)
- [x] SOTA Hybrid Approach (DB-First + FsAssay DDD)
- [x] Complex Chat DB Schema (Users, Groups, Messages)
- [x] CanonFlow FSA Extraction
- [x] FsAssay Property Tests for strict types

### 13. UrbanClub App (`urbanclub-core`)
- [x] SOTA Hybrid Approach (DB-First + FsAssay DDD)
- [x] Home Services DB Schema (Users, Services, Bookings)
- [x] CanonFlow FSA Extraction
- [x] FsAssay Property Tests for strict types
