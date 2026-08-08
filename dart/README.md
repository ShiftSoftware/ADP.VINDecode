# shift_software_adp_vin_decode

VIN validation, parsing, and extraction for Dart and Flutter. Pure Dart — no platform channels, no plugins, works everywhere Dart runs.

## Install

```yaml
dependencies:
  shift_software_adp_vin_decode: ^0.0.1
```

Or straight from the repo:

```yaml
dependencies:
  shift_software_adp_vin_decode:
    git:
      url: https://github.com/ShiftSoftware/ADP.VINDecode
      path: dart
```

```bash
dart pub get
```

## Usage

The package has no barrel file — import the sources you need directly:

```dart
import 'package:shift_software_adp_vin_decode/src/vin.dart';
import 'package:shift_software_adp_vin_decode/src/validator.dart';
import 'package:shift_software_adp_vin_decode/src/vin_extractor.dart';
```

### Validate

```dart
Validator.validateCheckDigit('19XFB2F57EE209415'); // true
```

Input must already be uppercase — `Validator` does not normalize.

### Parse

`tryParse` returns `null` on an invalid VIN; `parse` throws `ArgumentError`. Both accept mixed case.

```dart
final vin = VIN.tryParse('19xfB2F57Ee209415');

if (vin != null) {
  print(vin.wmi); // 19X
  print(vin.vds); // FB2F57
  print(vin.cd);  // E
  print(vin.vis); // EE209415
}

final parsed = VIN.parse('19XFB2F57EE209415'); // throws ArgumentError if invalid
```

`VIN` has a private constructor — an instance always holds a VIN that passed validation.

### Equality

`==` and `hashCode` are case-insensitive, so `VIN` works as a `Map` key or in a `Set`. `toString()` returns the uppercased VIN.

```dart
final a = VIN.parse('19XFB2F57EE209415');
final b = VIN.parse('19xfb2f57ee209415');

a == b;                  // true
print('VIN: $a');        // VIN: 19XFB2F57EE209415
```

### Extract from text

Three entry points, all returning a `Set<VIN>`:

```dart
// Single string
VINExtractor.findVINsFromString('VIN: 1GKEK63U83J235900 (Brand Name)');
// {1GKEK63U83J235900}

// List of strings — e.g. lines from a document
VINExtractor.findVINs([
  'Engine Size: 4.0 L',
  'VIN: 1FDKF37GXVEB34368 - 2025-03-26',
  'Brand: Some Brand',
]);
// {1FDKF37GXVEB34368}
```

A multi-line string passed as one item works the same way — newlines act as separators like any other non-VIN character.

### Extraction options

```dart
// Single string
VINExtractor.findVINsFromString(
  '3FAFP 3136YR25 9819',
  options: VINExtractionOptions(removeWhiteSpace: true),
);
// {3FAFP3136YR259819}

// List of strings
VINExtractor.findVINsWithOptions(
  VINExtractionOptions(mutateSpecialCharacters: true),
  ['IGCHK236O8F181Q70'],
);
// {1GCHK23608F181070}   (empty without the option)
```

| Option | Effect |
|---|---|
| `removeWhiteSpace` | Strips spaces before scanning — recovers VINs split by line wrapping or OCR. |
| `mutateSpecialCharacters` | Rewrites `I → 1`, `O → 0`, `Q → 0` — characters a VIN can never legally contain. |

### False positives

Extraction validates the check digit and nothing else, so a 17-character window straddling two adjacent VINs can pass:

```dart
VINExtractor.findVINsFromString(
  'KKJ9485N1AA08A15N7329651FMCU9G99FUA05110113948LJH').length;   // 4
VINExtractor.findVINsFromString(
  'KKJ948 5N1AA08A15N7329651FMCU9G99FUA05110 113948LJH').length; // 2
```

Preserve the separators in your source text where you can.

## Development

```bash
cd dart
dart pub get
dart test
dart test --plain-name "02. Components"
dart analyze
dart format .
```

SDK constraint: `>=2.15.0 <4.0.0`. Lints: `package:lints/recommended.yaml`.

## License

Apache License 2.0.
