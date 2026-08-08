# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A polyglot mono-repo holding **the same small VIN library ported to five targets**, one top-level folder per language. There is no shared build, no code generation, and no cross-folder dependency — each folder is a standalone package.

| Folder | Language | Package identity | Distribution |
|---|---|---|---|
| `dotnet/` | C# (netstandard2.0) | `ShiftSoftware.ADP.VINDecode` | NuGet (CI-published) |
| `typescript/` | TypeScript | `ADP.VINDecoder` (package.json) | Not publishable as-is — see below |
| `dart/` | Dart | `shift_software_adp_vin_decode` | pub |
| `android/` | Kotlin/JVM (Gradle) | `software.shift:lib` | Not published to any registry |
| `ios/` | — | — | Empty placeholder (`.gitkeep` only) |

**.NET is the reference implementation.** The other ports were written from it and must keep the same behavior. When changing library semantics, change every port — the test suites are the contract that binds them.

## The three types (present in every port)

- **`Validator`** — check-digit validation only. Holds the VIN transliteration table (char → int; `I`, `O`, `Q` deliberately absent) and the positional weight table `[8,7,6,5,4,3,2,10,0,9,8,7,6,5,4,3,2]`. Rejects anything not exactly 17 chars or containing an untransliterable char. Expects **uppercase** input — callers upper-case before calling.
- **`VIN`** — immutable value type, private constructor, only reachable via `parse` (throws) / `tryParse` (returns null / false+out). Exposes `WMI` (0..2), `VDS` (3..8), `CD` (9..9), `VIS` (9..16). Equality and hash code are case-insensitive on the whole VIN; `toString()` returns the stored uppercase VIN. .NET and TS additionally provide implicit string coercion (`implicit operator string` / `[Symbol.toPrimitive]`).
- **`VINExtractor`** + **`VINExtractionOptions`** — finds VINs inside free text. Normalizes the input per options, then brute-forces a **17-char sliding window** over the result, calling `tryParse` on every offset and collecting hits into a set (dedup by VIN value). Options: `RemoveWhiteSpace` (strip spaces) and `MutateSpecialCharacters` (`I→1`, `O→0`, `Q→0`).

The sliding window **is expected to produce false positives** when VINs are concatenated without separators — a 17-char window straddling two adjacent VINs can pass the check digit. Test `06` in every suite asserts those exact false positives on purpose. Do not "fix" it without changing all four suites.

## Test suites are the cross-language contract

Every port has the same numbered tests `01`–`10` (`01. Equality`, `02. Components`, `03. VIN Extractor: Leading and Trailing Symbols`, … `10. VIN Extractor: Character Replacement`) using the same VIN fixtures. TypeScript splits them across two files (`tests/VIN.test.ts` holds 01–02, `tests/VINExtractor.test.ts` holds 03–10).

When adding behavior: add the numbered test to **all** ports that have a test suite (.NET, TS, Dart, Kotlin), keeping the number and title aligned.

## Naming divergence between ports

Public API names follow each language's idiom, so grep across folders won't match:

| Concept | .NET | TypeScript | Dart | Kotlin |
|---|---|---|---|---|
| parse | `VIN.Parse` | `VIN.parse` | `VIN.parse` | `VIN.parseVIN` |
| try parse | `VIN.TryParse(s, out v)` | `VIN.tryParse(s) → [bool, VIN\|null]` | `VIN.tryParse(s) → VIN?` | `VIN.tryParseVIN(s) → VIN?` |
| extract | `VINExtractor.FindVINs(opts?, params string[])` | `findVINs(...texts, opts?)` (options sniffed as trailing object arg) | `findVINs(List)` / `findVINsFromString(String, {options})` / `findVINsWithOptions(opts, List)` | `VINExtractor.findVINs(opts?, vararg texts)` |
| components | `WMI/VDS/CD/VIS` | `WMI/VDS/CD/VIS` | `wmi/vds/cd/vis` | `wmi/vds/cd/vis` |
| result type | `HashSet<VIN>` | `VIN[]` (deduped via internal `Set`) | `Set<VIN>` | `Set<VIN>` |

Perf-oriented span/array overloads exist only in .NET (`ReadOnlySpan<char>`, `stackalloc`) and Kotlin (`CharArray`). Dart and TS use plain substring slicing.

## Known quirks — do not "clean up" without a decision

- **`CD` is off by one, consistently, in all four ports.** The check digit is VIN index 8 (`Validator` checks `vin[8]`), but the `CD` property returns index 9. Every `02. Components` test asserts the index-9 value (`"E"` for `19XFB2F57EE209415`). Changing it is a breaking API change across all ports and all four test suites.
- **TypeScript filename casing.** Git tracks `typescript/src/validator.ts` lowercase, but `src/VIN.ts` imports `"./Validator"` and `rollup.config.ts` lists `src/Validator.ts`. This resolves on Windows/macOS and **breaks on a case-sensitive filesystem** (Linux CI). Fix with `git mv` if the TS build ever moves to Linux.
- **TypeScript is not currently npm-publishable.** `package.json` has no `version` field, and the name `ADP.VINDecoder` is invalid for npm (uppercase + dot). Consume it from source or a build artifact.
- **Dart has no barrel file.** There is no `lib/shift_software_adp_vin_decode.dart`; consumers and tests import through `package:shift_software_adp_vin_decode/src/…` directly.
- **Android Gradle cruft.** `android/lib/build.gradle.kts` still carries `guava` and `commons-math3` from the `gradle init` template; the library code uses neither. There is no `maven-publish` configuration.
- **iOS is unimplemented** — `ios/` contains only `.gitkeep`.

## Commands

### .NET (`dotnet/`)

```bash
dotnet build dotnet --configuration Release
dotnet test dotnet/ADP.VINDecode.Tests
dotnet test dotnet/ADP.VINDecode.Tests --filter "DisplayName~Character Replacement"   # single test
dotnet pack dotnet/ADP.VINDecode --configuration Release --output <dir>
```

Library targets `netstandard2.0`; the test project targets `net8.0` with xUnit. Test names are set via `[Fact(DisplayName = "…")]`, so `--filter DisplayName~…` is the way to run one.

### TypeScript (`typescript/`)

```bash
npm install
npm test                      # vitest (watch by default)
npx vitest run                # single non-watch pass
npx vitest run tests/VIN.test.ts -t "02. Components"   # single test
npm run build                 # rollup → dist/{index,VIN,Validator,VINExtractor}.{mjs,cjs,d.ts}
```

Rollup emits four separate entry points (mirrored by the `exports` map in `package.json`) in both ESM and CJS, plus rolled-up `.d.ts` via `rollup-plugin-dts`.

### Dart (`dart/`)

```bash
cd dart
dart pub get
dart test
dart test --plain-name "02. Components"   # single test
dart analyze
dart format .
```

Lints come from `package:lints/recommended.yaml`. SDK constraint is `>=2.15.0 <4.0.0`.

### Android / Kotlin (`android/`)

```bash
cd android
./gradlew build
./gradlew test
./gradlew test --tests "software.shift.adp.vindecode.VINTest"
```

Gradle 8.14 wrapper, Kotlin JVM 2.1.20, Java toolchain 17, JUnit 5 via `kotlin-test-junit5`. `local.properties` is machine-local.

## CI / release

Only `.NET` has a pipeline: `dotnet/ADP.VINDecode/azure-pipeline.yml`, on `ubuntu-22.04`, triggered by **git tags** matching `release*`. It builds everything under `dotnet/`, then gates test + pack + `nuget push` on the tag being `release-all` or `release-dotnet`. Releasing .NET therefore means: bump `<Version>` in `ADP.VINDecode.csproj`, commit, tag `release-dotnet-x-y-z` (or `release-all-…`), push the tag.

Existing tags use dashes, not dots: `release-dotnet-0-0-4`, `release-dotnet-0-0-4-2`.

No other language has automated publishing.
