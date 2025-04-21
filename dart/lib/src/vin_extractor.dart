import 'package:shift_software_adp_vin_decode/src/vin.dart';

class VINExtractionOptions {
  /// Removes all whitespace from the input text.
  final bool removeWhiteSpace;

  /// Replaces not allowed VIN characters with their allowed counterparts.
  /// 
  /// I -> 1, O -> 0, Q -> 0
  final bool mutateSpecialCharacters;

  const VINExtractionOptions({
    this.removeWhiteSpace = false,
    this.mutateSpecialCharacters = false,
  });
}

class VINExtractor {
  /// Find VINs with default options
  static Set<VIN> findVINs(List<String> texts) {
    return findVINsWithOptions(null, texts);
  }

  static Set<VIN> findVINsFromString(String texts, {
      VINExtractionOptions? options}) {

    return findVINsWithOptions(options, [texts]);
  }

  /// Find VINs with custom options
  static Set<VIN> findVINsWithOptions(
      VINExtractionOptions? options, List<String> texts) {
    final vins = <VIN>{};

    for (final text in texts) {
      if (text.trim().isEmpty) continue;

      final normalized = _normalizeInput(text, options);

      for (int i = 0; i <= normalized.length - 17; i++) {
        final chunk = normalized.substring(i, i + 17);
        final vin = VIN.tryParse(chunk);
        if (vin != null) {
          vins.add(vin);
        }
      }
    }

    return vins;
  }

  /// Normalize input string based on options
  static String _normalizeInput(String text, VINExtractionOptions? options) {
    if (options == null) return text;

    final buffer = StringBuffer();
    
    for (final c in text.split('')) {
      if (options.removeWhiteSpace && c == ' ') continue;

      if (options.mutateSpecialCharacters) {
        if (c == 'I') {
          buffer.write('1');
        } else if (c == 'O' || c == 'Q') {
          buffer.write('0');
        } else {
          buffer.write(c);
        }
      } else {
        buffer.write(c);
      }
    }

    return buffer.toString();
  }
}
