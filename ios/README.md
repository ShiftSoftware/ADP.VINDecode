# ADP.VINDecode (iOS)

**Not implemented yet.** This folder is a placeholder for a Swift port of the library.

## In the meantime

If you need VIN decoding on iOS today:

- **Flutter apps** — use the Dart port, [`dart/`](../dart/). It is pure Dart and runs on iOS unchanged.
- **.NET MAUI / Xamarin apps** — use the .NET port, [`dotnet/`](../dotnet/). It targets `netstandard2.0`.
- **Native Swift** — port it by hand. The library is small and dependency-free; the Kotlin port in [`android/`](../android/) is the closest structural match.

## What a Swift port needs

Mirror the three types the other ports share:

- **`Validator`** — `validateCheckDigit(_ vin: String) -> Bool`. Transliteration table (`I`, `O`, `Q` excluded) and weight table `[8,7,6,5,4,3,2,10,0,9,8,7,6,5,4,3,2]`. Reject anything not exactly 17 characters or containing an untransliterable character; compare the computed digit against position 9 (`X` when the remainder is 10).
- **`VIN`** — an immutable value with a private initializer, reachable only through `parse` (throws) and `tryParse` (returns optional). Exposes `wmi`, `vds`, `cd`, `vis`. Case-insensitive `Equatable`/`Hashable`; `description` returns the uppercased VIN.
- **`VINExtractor`** + **`VINExtractionOptions`** — normalize per options (`removeWhiteSpace`, `mutateSpecialCharacters`: `I → 1`, `O → 0`, `Q → 0`), then slide a 17-character window over the result and collect every slice that validates into a `Set<VIN>`.

Port the numbered test suite (`01`–`10`) from [`../dotnet/ADP.VINDecode.Tests/VINTests.cs`](../dotnet/ADP.VINDecode.Tests/VINTests.cs) as-is — it is the cross-language contract, including the deliberate false-positive assertions in test `06`.

See the [root README](../README.md) for the full API description.
