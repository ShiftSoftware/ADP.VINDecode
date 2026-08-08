# ADP.VINDecode (TypeScript)

VIN validation, parsing, and extraction for TypeScript and JavaScript. Ships ESM and CJS builds plus type declarations.

## Status

**Not published to npm yet.** `package.json` currently has no `version` field, and the name `ADP.VINDecoder` is not a legal npm package name (npm rejects uppercase letters and dots). Consume it by building from source, or vendor `dist/` into your project.

```bash
cd typescript
npm install
npm run build      # → dist/
```

Then point your project at the built output, e.g. via a file dependency:

```json
{
  "dependencies": {
    "adp-vindecode": "file:../ADP.VINDecode/typescript"
  }
}
```

## Usage

```ts
import { VIN, Validator, findVINs } from "adp-vindecode"
```

Subpath entry points are also built, if you want to pull in only what you use:

```ts
import VIN from "adp-vindecode/VIN"
import Validator from "adp-vindecode/Validator"
import VINExtractor from "adp-vindecode/VINExtractor"
```

### Validate

```ts
Validator.validateCheckDigit("19XFB2F57EE209415") // true
```

Input must already be uppercase — `Validator` does not normalize.

### Parse

`tryParse` returns a tuple `[valid, vin]`; `parse` throws on an invalid VIN. Both accept mixed case.

```ts
const [valid, vin] = VIN.tryParse("19xfB2F57Ee209415")

if (valid && vin) {
  vin.WMI // "19X"
  vin.VDS // "FB2F57"
  vin.CD  // "E"
  vin.VIS // "EE209415"
}

const parsed = VIN.parse("19XFB2F57EE209415") // throws Error("Invalid VIN")
```

`VIN` has a private constructor — an instance always holds a VIN that passed validation.

### Equality and string coercion

```ts
const a = VIN.parse("19XFB2F57EE209415")
const b = VIN.parse("19xfb2f57ee209415")

a.equals(b)      // true
a.getHashCode()  // stable, case-insensitive

String(a)        // "19XFB2F57EE209415"
`VIN: ${a}`      // "VIN: 19XFB2F57EE209415"
```

`Symbol.toPrimitive` is implemented, so a `VIN` coerces to its string form in template literals and string concatenation. `getHashCode()` is provided for parity with the other ports; in JS, key a `Map` on `vin.toString()` instead.

### Extract from text

`findVINs` takes any number of strings and returns a deduplicated `VIN[]`.

```ts
findVINs("VIN: 1GKEK63U83J235900 (Brand Name)")
// [ VIN(1GKEK63U83J235900) ]

findVINs(
  "Engine Size: 4.0 L",
  "VIN: 1FDKF37GXVEB34368 - 2025-03-26",
  "Brand: Some Brand"
)
// [ VIN(1FDKF37GXVEB34368) ]
```

### Extraction options

Pass an options object as the **last** argument — `findVINs` detects a trailing object and treats it as options.

```ts
// VIN broken up by whitespace (OCR, line wrapping)
findVINs("3FAFP 3136YR25 9819", { RemoveWhiteSpace: true })
// [ VIN(3FAFP3136YR259819) ]

// Characters a VIN can never contain, mapped back: I -> 1, O -> 0, Q -> 0
findVINs("IGCHK236O8F181Q70", { MutateSpecialCharacters: true })
// [ VIN(1GCHK23608F181070) ]   (returns [] without the option)
```

| Option | Effect |
|---|---|
| `RemoveWhiteSpace` | Strips all whitespace before scanning. |
| `MutateSpecialCharacters` | Rewrites `I → 1`, `O → 0`, `Q → 0` before scanning. |

`normalizeInput(text, options)` is exported separately if you want to apply the same normalization yourself.

### False positives

Extraction validates the check digit and nothing else, so a window straddling two adjacent VINs can pass:

```ts
findVINs("KKJ9485N1AA08A15N7329651FMCU9G99FUA05110113948LJH").length   // 4
findVINs("KKJ948 5N1AA08A15N7329651FMCU9G99FUA05110 113948LJH").length // 2
```

Preserve the separators in your source text where you can.

## Development

```bash
npm install
npm test                                             # vitest, watch mode
npx vitest run                                        # single pass
npx vitest run tests/VIN.test.ts -t "02. Components"  # one test
npm run build                                         # rollup
```

The build emits four entry points — `index`, `VIN`, `Validator`, `VINExtractor` — each as `.mjs`, `.cjs`, and a rolled-up `.d.ts`.

> **Note:** the source file is tracked in git as `src/validator.ts` (lowercase) while imports reference `./Validator`. This resolves on Windows and macOS but fails on a case-sensitive filesystem such as Linux CI. `git mv` it if you need to build there.

## License

Apache License 2.0.
