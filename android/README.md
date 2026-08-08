# ADP.VINDecode (Kotlin / Android)

VIN validation, parsing, and extraction for Kotlin. Plain Kotlin/JVM with no Android framework dependencies — usable from an Android app, a Kotlin server, or any JVM project.

## Status

**Not published to any registry.** There is no Maven Central or JitPack artifact and no `maven-publish` configuration in the build. Consume it from source using one of the options below.

### Option A — composite build (recommended for local development)

In your consuming project's `settings.gradle.kts`:

```kotlin
includeBuild("../ADP.VINDecode/android") {
    dependencySubstitution {
        substitute(module("software.shift:lib")).using(project(":lib"))
    }
}
```

```kotlin
// build.gradle.kts
dependencies {
    implementation("software.shift:lib")
}
```

### Option B — build a JAR and drop it in

```bash
cd android
./gradlew :lib:jar          # → lib/build/libs/lib.jar
```

```kotlin
dependencies {
    implementation(files("libs/lib.jar"))
}
```

### Option C — copy the sources

The library is three files with no third-party dependencies: `Validator.kt`, `VIN.kt`, `VINExtractor.kt` under `lib/src/main/kotlin/software/shift/adp/vindecode/`.

## Usage

```kotlin
import software.shift.adp.vindecode.VIN
import software.shift.adp.vindecode.Validator
import software.shift.adp.vindecode.VINExtractor
import software.shift.adp.vindecode.VINExtractionOptions
```

### Validate

```kotlin
Validator.validateCheckDigit("19XFB2F57EE209415") // true
```

Input must already be uppercase — `Validator` does not normalize. A `CharArray` overload exists for allocation-sensitive paths.

### Parse

`tryParseVIN` returns `null` on an invalid VIN; `parseVIN` throws `IllegalArgumentException`. Both accept mixed case.

```kotlin
val vin = VIN.tryParseVIN("19xfB2F57Ee209415")

if (vin != null) {
    println(vin.wmi) // 19X
    println(vin.vds) // FB2F57
    println(vin.cd)  // E
    println(vin.vis) // EE209415
}

val parsed = VIN.parseVIN("19XFB2F57EE209415") // throws if invalid
```

`VIN`'s constructor is private — an instance always holds a VIN that passed validation.

### Equality

`equals` and `hashCode` are case-insensitive, so `VIN` works as a `Map` key or in a `Set`. `toString()` returns the uppercased VIN.

```kotlin
val a = VIN.parseVIN("19XFB2F57EE209415")
val b = VIN.parseVIN("19xfb2f57ee209415")

a == b            // true
println("VIN: $a") // VIN: 19XFB2F57EE209415
```

### Extract from text

`findVINs` is a vararg over strings and returns a `Set<VIN>`.

```kotlin
VINExtractor.findVINs("VIN: 1GKEK63U83J235900 (Brand Name)")
// [1GKEK63U83J235900]

VINExtractor.findVINs(
    "Engine Size: 4.0 L",
    "VIN: 1FDKF37GXVEB34368 - 2025-03-26",
    "Brand: Some Brand"
)
// [1FDKF37GXVEB34368]
```

Multi-line strings work as a single argument too — newlines act as separators like any other non-VIN character.

### Extraction options

Pass `VINExtractionOptions` as the first argument.

```kotlin
// VIN broken up by whitespace (OCR, line wrapping)
VINExtractor.findVINs(
    VINExtractionOptions(removeWhiteSpace = true),
    "3FAFP 3136YR25 9819"
)
// [3FAFP3136YR259819]

// Characters a VIN can never contain, mapped back: I -> 1, O -> 0, Q -> 0
VINExtractor.findVINs(
    VINExtractionOptions(mutateSpecialCharacters = true),
    "IGCHK236O8F181Q70"
)
// [1GCHK23608F181070]   (empty without the option)
```

| Option | Effect |
|---|---|
| `removeWhiteSpace` | Strips spaces before scanning. |
| `mutateSpecialCharacters` | Rewrites `I → 1`, `O → 0`, `Q → 0` before scanning. |

### False positives

Extraction validates the check digit and nothing else, so a 17-character window straddling two adjacent VINs can pass:

```kotlin
VINExtractor.findVINs("KKJ9485N1AA08A15N7329651FMCU9G99FUA05110113948LJH").size   // 4
VINExtractor.findVINs("KKJ948 5N1AA08A15N7329651FMCU9G99FUA05110 113948LJH").size // 2
```

Preserve the separators in your source text where you can.

## Development

```bash
cd android
./gradlew build
./gradlew test
./gradlew test --tests "software.shift.adp.vindecode.VINTest"
```

Gradle 8.14 (wrapper included), Kotlin JVM 2.1.20, Java toolchain 17, JUnit 5.

`lib/build.gradle.kts` still declares `guava` and `commons-math3` from the `gradle init` template; the library code uses neither and they can be dropped.

## License

Apache License 2.0.
