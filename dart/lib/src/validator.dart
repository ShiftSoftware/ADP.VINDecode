library shift_software_adp_vin_decode;

class Validator {
   // Transliteration Table
    static final Map<String, int> transliterationTable = {
      '0': 0,
      '1': 1,
      '2': 2,
      '3': 3,
      '4': 4,
      '5': 5,
      '6': 6,
      '7': 7,
      '8': 8,
      '9': 9,
      'A': 1,
      'B': 2,
      'C': 3,
      'D': 4,
      'E': 5,
      'F': 6,
      'G': 7,
      'H': 8,
      'J': 1,
      'K': 2,
      'L': 3,
      'M': 4,
      'N': 5,
      'P': 7,
      'R': 9,
      'S': 2,
      'T': 3,
      'U': 4,
      'V': 5,
      'W': 6,
      'X': 7,
      'Y': 8,
      'Z': 9
    };

    // Weight Table
    static final List<int> weightTable = [
      8,
      7,
      6,
      5,
      4,
      3,
      2,
      10,
      0,
      9,
      8,
      7,
      6,
      5,
      4,
      3,
      2
    ];

  /// Validates a VIN (Vehicle Identification Number) using the check digit rule.
  ///
  /// The VIN must be exactly 17 characters long. This function uses
  /// a transliteration and weight table to compute the expected check digit,
  /// and compares it to the 9th character of the VIN.
  ///
  /// Returns `true` if the VIN is valid, otherwise `false`.
  static bool validateCheckDigit(String vin) {
    // Check if VIN is not empty and exactly 17 characters
    if (vin.isEmpty || vin.length != 17) {
      return false;
    }

    int sum = 0;

    for (int i = 0; i < vin.length; i++) {
      final char = vin[i];

      // Check if the character is valid (in the transliteration table)
      if (!transliterationTable.containsKey(char)) {
        return false;
      }

      final int value = transliterationTable[char]!;
      final int weight = weightTable[i];
      final int product = value * weight;

      sum = sum + product;
    }

    // Calculate checksum
    final int remainder = sum % 11;
    final String checksum = remainder == 10 ? 'X' : remainder.toString();

    // Validate checksum against 9th character
    return vin[8] == checksum;
  }


  // static bool isVinValid(String vin) {
  //   // Generate all possible 17-character chunks
  //   for (int i = 0; i <= vin.length - 17; i++) {
  //     final chunk = vin.substring(i, i + 17);
  //     if (_validateVin(chunk)) {
  //       return true; // Return the first valid VIN chunk
  //     }
  //   }
  //   return false; // No valid VIN found
  // }
}
