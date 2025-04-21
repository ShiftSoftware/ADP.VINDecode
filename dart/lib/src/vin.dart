import 'package:shift_software_adp_vin_decode/src/validator.dart';

class VIN {
  final String _vin;
  final String wmi;
  final String vds;
  final String cd;
  final String vis;

  VIN._(this._vin)
      : wmi = _vin.substring(0, 3),
        vds = _vin.substring(3, 9),
        cd = _vin.substring(9, 10),
        vis = _vin.substring(9, 17);

  /// Tries to parse a VIN. Returns a [VIN] instance if valid, otherwise `null`.
  static VIN? tryParse(String vin) {
    final vinUpper = vin.toUpperCase();

    if (!Validator.validateCheckDigit(vinUpper)) {
      return null;
    }

    return VIN._(vinUpper); // Using private constructor
  }

  /// Parses a VIN. Throws an [ArgumentError] if invalid.
  static VIN parse(String vin) {
    final vinUpper = vin.toUpperCase();

    if (!Validator.validateCheckDigit(vinUpper)) {
      throw ArgumentError('Invalid VIN', 'vin');
    }

    return VIN._(vinUpper); // Using private constructor
  }

  @override
  String toString() => _vin;

  @override
  bool operator == (Object other) =>
      other is VIN && _vin.toUpperCase() == other._vin.toUpperCase();

  @override
  int get hashCode => _vin.toUpperCase().hashCode;
}
