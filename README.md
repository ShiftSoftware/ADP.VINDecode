# ADP.VINDecode

VIN validation, parsing, and extraction — the same library, ported to multiple languages.

A **VIN** (Vehicle Identification Number) is a 17-character code. This library:

- validates a VIN via its **check digit** (ISO 3779 transliteration + positional weights),
- **parses** a VIN into its components — WMI, VDS, CD, VIS,
- **extracts** VINs out of free-form text (OCR output, invoices, log lines, multi-line documents).

## Ports

| Language | Folder | Package | Availability |
|---|---|---|---|
| .NET | [dotnet/](dotnet/) | `ShiftSoftware.ADP.VINDecode` | [NuGet](https://www.nuget.org/packages/ShiftSoftware.ADP.VINDecode) |
| Dart / Flutter | [dart/](dart/) | `shift_software_adp_vin_decode` | pub |
| TypeScript | [typescript/](typescript/) | `ADP.VINDecoder` | Not published — build from source |
| Kotlin / Android | [android/](android/) | `software.shift:lib` | Not published — build from source |
| iOS | [ios/](ios/) | — | Not implemented yet |

Each folder has its own README with install and usage instructions.

## Common API

Every port exposes the same three types, named idiomatically per language:

- **`Validator`** — `validateCheckDigit(vin)` → `bool`. 17 chars, no `I`/`O`/`Q`, check digit at position 9 must match.
- **`VIN`** — an immutable, validated value. Created only through `parse` (throws on invalid) or `tryParse` (returns null/false). Exposes:
  - `WMI` — World Manufacturer Identifier, characters 1–3
  - `VDS` — Vehicle Descriptor Section, characters 4–9
  - `CD` — check digit field
  - `VIS` — Vehicle Identifier Section, characters 10–17

  Equality and hashing are case-insensitive; `toString()` returns the uppercased VIN.
- **`VINExtractor`** — `findVINs(...)` scans text with a 17-character sliding window and returns every distinct slice that passes validation. Options:
  - `RemoveWhiteSpace` — strip spaces first, so a VIN broken by line-wrapping or OCR (`3FAFP 3136YR25 9819`) is still found.
  - `MutateSpecialCharacters` — map the characters a VIN can never contain onto the ones they were probably misread from: `I → 1`, `O → 0`, `Q → 0`.

### About false positives

Extraction is a brute-force scan: every 17-character window that passes the check digit is returned. When two VINs sit next to each other with no separator, a window straddling the boundary can also pass — roughly 1 in 11 random 17-char strings will. Given

```
KKJ9485N1AA08A15N7329651FMCU9G99FUA05110113948LJH
```

the extractor returns **four** VINs: the two real ones plus `485N1AA08A15N7329` and `FUA05110113948LJH`. Whitespace between the VINs removes the ambiguity and yields exactly two.

This is inherent to check-digit-only validation. Feed the extractor text that retains its original separators, and treat results as candidates if the source is untrusted.

## Repository layout

One top-level folder per language, each a standalone package with its own build and test suite. There is no shared build. The .NET implementation is the reference; the others are ports of it and share an identical numbered test suite (`01`–`10`) over the same VIN fixtures.

## License

Apache License 2.0.
