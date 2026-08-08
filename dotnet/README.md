# ShiftSoftware.ADP.VINDecode (.NET)

VIN validation, parsing, and extraction for .NET. Targets `netstandard2.0`, so it runs on .NET Framework 4.6.1+, .NET Core, .NET 5+, Xamarin, and Unity.

## Install

```bash
dotnet add package ShiftSoftware.ADP.VINDecode
```

```xml
<PackageReference Include="ShiftSoftware.ADP.VINDecode" Version="0.0.4" />
```

## Usage

```csharp
using ShiftSoftware.ADP.VINDecode;
```

### Validate

```csharp
bool ok = Validator.ValidateCheckDigit("19XFB2F57EE209415"); // true
```

Input must be uppercase — `Validator` does not normalize. There is also a `ReadOnlySpan<char>` overload for allocation-free validation.

### Parse

`TryParse` returns `false` instead of throwing; `Parse` throws `ArgumentException` on an invalid VIN. Both accept mixed case.

```csharp
if (VIN.TryParse("19xfB2F57Ee209415", out VIN vin))
{
    Console.WriteLine(vin.WMI); // 19X
    Console.WriteLine(vin.VDS); // FB2F57
    Console.WriteLine(vin.CD);  // E
    Console.WriteLine(vin.VIS); // EE209415
}

VIN parsed = VIN.Parse("19XFB2F57EE209415"); // throws if invalid
```

`VIN` is immutable and has no public constructor — an instance always holds a VIN that passed validation.

There is a `ReadOnlySpan<char>` overload of `TryParse` that validates without allocating; it is what `VINExtractor` uses internally.

### Equality and string conversion

Equality and `GetHashCode` are case-insensitive, so parsed VINs work as dictionary and `HashSet` keys. An implicit conversion to `string` lets a `VIN` be used wherever a string is expected.

```csharp
var a = VIN.Parse("19XFB2F57EE209415");
var b = VIN.Parse("19xfb2f57ee209415");

a.Equals(b);                  // true
string s = a;                 // "19XFB2F57EE209415"
Console.WriteLine($"VIN: {a}");
```

### Extract from text

`FindVINs` scans each input string with a 17-character sliding window and returns a `HashSet<VIN>` of everything that validates. It accepts any number of strings.

```csharp
var vins = VINExtractor.FindVINs("VIN: 1GKEK63U83J235900 (Brand Name)");
// { 1GKEK63U83J235900 }

var fromLines = VINExtractor.FindVINs(
    "Engine Size: 4.0 L",
    "VIN: 1FDKF37GXVEB34368 - 2025-03-26",
    "Brand: Some Brand"
);
// { 1FDKF37GXVEB34368 }
```

Multi-line strings work as a single argument too — newlines act as separators like any other non-VIN character.

### Extraction options

Pass `VINExtractionOptions` as the first argument.

```csharp
// VIN broken up by whitespace (OCR, line wrapping)
VINExtractor.FindVINs(
    new VINExtractionOptions { RemoveWhiteSpace = true },
    "3FAFP 3136YR25 9819"
);
// { 3FAFP3136YR259819 }

// Characters a VIN can never contain, mapped back: I -> 1, O -> 0, Q -> 0
VINExtractor.FindVINs(
    new VINExtractionOptions { MutateSpecialCharacters = true },
    "IGCHK236O8F181Q70"
);
// { 1GCHK23608F181070 }   (returns nothing without the option)
```

| Option | Effect |
|---|---|
| `RemoveWhiteSpace` | Strips spaces before scanning. |
| `MutateSpecialCharacters` | Rewrites `I → 1`, `O → 0`, `Q → 0` before scanning. |

### False positives

Extraction validates the check digit and nothing else, so a window straddling two adjacent VINs can pass:

```csharp
VINExtractor.FindVINs("KKJ9485N1AA08A15N7329651FMCU9G99FUA05110113948LJH").Count; // 4
VINExtractor.FindVINs("KKJ948 5N1AA08A15N7329651FMCU9G99FUA05110 113948LJH").Count; // 2
```

Preserve the separators in your source text where you can.

## Building and testing

```bash
dotnet build dotnet --configuration Release
dotnet test dotnet/ADP.VINDecode.Tests
dotnet test dotnet/ADP.VINDecode.Tests --filter "DisplayName~Character Replacement"
```

Tests are xUnit on `net8.0`.

## Releasing

`azure-pipeline.yml` triggers on tags matching `release*` and publishes to NuGet when the tag is `release-all-*` or `release-dotnet-*`. To cut a release: bump `<Version>` in `ADP.VINDecode.csproj`, commit, then tag and push — e.g. `release-dotnet-0-0-5`.

## License

Apache License 2.0.
