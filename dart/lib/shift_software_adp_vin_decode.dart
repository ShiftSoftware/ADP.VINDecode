/// VIN validation, parsing, and extraction.
///
/// ```dart
/// import 'package:shift_software_adp_vin_decode/shift_software_adp_vin_decode.dart';
///
/// final vin = VIN.tryParse('19XFB2F57EE209415');
/// final found = VINExtractor.findVINsFromString('VIN: 1GKEK63U83J235900');
/// ```
library shift_software_adp_vin_decode;

export 'src/validator.dart';
export 'src/vin.dart';
export 'src/vin_extractor.dart';
