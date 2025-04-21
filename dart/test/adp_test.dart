import 'package:shift_software_adp_vin_decode/src/vin.dart';
import 'package:shift_software_adp_vin_decode/src/vin_extractor.dart';
import 'package:shift_software_adp_vin_decode/src/validator.dart';
import 'package:test/test.dart';

void main() {
  test('01. Equality: VIN equality and hash code checks', () {
    final vinString1 = '19XFB2F57EE209415';
    final vinString2 = '19xfB2F57Ee209415';
    final vinString3 = '19XFb2F57eE209415';

    final vin1 = VIN.tryParse(vinString1);
    final vin2 = VIN.tryParse(vinString2);
    final vin3 = VIN.tryParse(vinString3);

    expect(vin1, isNotNull);
    expect(vin2, isNotNull);
    expect(vin3, isNotNull);

    print(vin1);
    expect(vin1, equals(vin2));
    print(vin2);
    expect(vin1, equals(vin3));
    print(vin3);
    expect(vin2, equals(vin3));

    expect(vin1.hashCode, equals(vin2.hashCode));
    expect(vin1.hashCode, equals(vin3.hashCode));
    expect(vin2.hashCode, equals(vin3.hashCode));

    expect(vin1.toString(), equals(vinString1.toUpperCase()));
    expect(vin2.toString(), equals(vinString2.toUpperCase()));
    expect(vin3.toString(), equals(vinString3.toUpperCase()));
  });

  test('02. Components: Extract VIN components', () {
    final vin = VIN.tryParse('19XFB2F57EE209415');

    expect(vin, isNotNull);

    print(vin!.wmi);
    expect(vin!.wmi, equals('19X'));

    print(vin.vds);
    expect(vin.vds, equals('FB2F57'));

    print(vin.cd);
    expect(vin.cd, equals('E'));

    print(vin.vis);
    expect(vin.vis, equals('EE209415'));
  });

  test('03. VIN Extractor: Leading and Trailing Symbols', () {
    final List<String> vinString = ['*1FUJGLDRXblaY2150*'];
    final vinString2 = '* 1fuJGLDRXBLAY2150 *';

    final vinSet = VINExtractor.findVINs(vinString);
    final vinSet2 = VINExtractor.findVINsFromString(vinString2);

    print('VinSet Length: ' + vinSet.length.toString());
    print('VinSet2 Length: ' + vinSet2.length.toString());
    expect(vinSet.length, equals(1));
    expect(vinSet2.length, equals(1));

    final vin = vinSet.first;
    final vin2 = vinSet2.first;

    print('VinSet: $vinSet');
    print('VinSet2: $vinSet2');
    expect(vin.toString(), equals('1FUJGLDRXBLAY2150'));
    expect(vin2.toString(), equals('1FUJGLDRXBLAY2150'));
  });

  test('04. VIN Extractor: Leading and Trailing Text', () {
    final vinString = 'VIN: 1GKEK63U83J235900 (Brand Name)';

    final vinSet = VINExtractor.findVINsFromString(vinString);

    print('VinSet Length: ' + vinSet.length.toString());
    expect(vinSet.length, equals(1));

    final vin = vinSet.first;

    print('VinSet: $vinSet');
    expect(vin.toString(), equals('1GKEK63U83J235900'));
  });

  test('05. VIN Extractor: Multiple VINs in Same Line', () {
    final vinString = 'K249 2HGES25724H591789 239 JH4DC54865S056007';

    final vinSet = VINExtractor.findVINsFromString(vinString).toList();

    expect(vinSet.length, equals(2));

    final vin1 = vinSet[0];
    final vin2 = vinSet[1];

    print(vin1);
    print(vin2);

    expect(vin1.toString(), equals('2HGES25724H591789'));
    expect(vin2.toString(), equals('JH4DC54865S056007'));
  });

  test(
      '06. VIN Extractor: Multiple VINs in Same Line Without Whitespace (Produces False Positives)',
      () {
    final vinString1 = "5N1AA08A15N732965";
    final vinString2 = "1FMCU9G99FUA05110";

    final combinedVINLine = "KKJ948${vinString1}${vinString2}113948LJH";

    final vinSet = VINExtractor.findVINsFromString(combinedVINLine).toList();

    expect(vinSet.length, equals(4));

    final vin1 = vinSet[0];
    final vin2 = vinSet[1];
    final vin3 = vinSet[2];
    final vin4 = vinSet[3];

    // False Positive
    expect(vin1.toString(), equals("485N1AA08A15N7329"));
    expect(vin2.toString(), equals(vinString1));
    expect(vin3.toString(), equals(vinString2));
    expect(vin4.toString(), equals("FUA05110113948LJH"));

    // Same test with whitespace does not produce false positives
    final withWhitespace = VINExtractor.findVINsFromString(
        "KKJ948 ${vinString1}${vinString2} 113948LJH");
    expect(withWhitespace.length, equals(2));
  });

  test('07. VIN Extractor: Whitespace inside VIN', () {
    final vinString = "3FAFP 3136YR25 9819";

    final vinSet = VINExtractor.findVINsWithOptions(
      VINExtractionOptions(removeWhiteSpace: true),
      [vinString],
    );

    expect(vinSet.length, equals(1));
    expect(vinSet.first.toString(), equals("3FAFP3136YR259819"));
    print('vinString: ' + vinString);
    print('vinSet: ' + vinSet.first.toString());
  });

  test('08. VIN Extractor: Multiple Lines', () {
    final line1 = "Engine Size: 4.0 L";
    final line2 = "VIN: 1FDKF37GXVEB34368 - 2025-03-26";
    final line3 = "Brand: Some Brand";
    final line4 = "Color: White";

    final vinSet = VINExtractor.findVINs([line1, line2, line3, line4]);

    expect(vinSet.length, equals(1));
    expect(vinSet.first.toString(), equals("1FDKF37GXVEB34368"));

    final multiLineString = '''
    Engine Size: 4.0 L
    VIN: 1FDKF37GXVEB34368 - 2025-03-26
    Brand: Some Brand
    Color: White
    ''';

    final multiLineVinSet = VINExtractor.findVINs([multiLineString]);

    expect(multiLineVinSet.length, equals(1));
    expect(multiLineVinSet.first.toString(), equals("1FDKF37GXVEB34368"));
  });

  test('09. VIN Extractor: Multiple VINs in Multiple lines', () {
    final vinString = '''
    VIN 1 & VIN2:
    2C4RDGBG6ER101765
    3GNDA13D48S626939

    VIN 3:
    1GNES16S932111256 - 2C4RDGCG2ER226597
    ''';

    final vinSet = VINExtractor.findVINsFromString(vinString);
    expect(vinSet.length, equals(4));

    final vinSetWithOptions = VINExtractor.findVINsWithOptions(
      VINExtractionOptions(removeWhiteSpace: true),
      [vinString],
    );
    expect(vinSetWithOptions.length, equals(4));

    final vinList = vinSet.toList();

    expect(vinList[0].toString(), equals("2C4RDGBG6ER101765"));
    expect(vinList[1].toString(), equals("3GNDA13D48S626939"));
    expect(vinList[2].toString(), equals("1GNES16S932111256"));
    expect(vinList[3].toString(), equals("2C4RDGCG2ER226597"));
  });

  test('10. VIN Extractor: Character Replacement', () {
    final vinString = "IGCHK236O8F181Q70";
    print(vinString);
    // Without Character Replacement
    final noReplacementSet = VINExtractor.findVINsFromString(vinString);

    expect(noReplacementSet.length, 0);

    // With Character Replacement
    final vinSet = VINExtractor.findVINsFromString(
      vinString,
      options: VINExtractionOptions(mutateSpecialCharacters: true),
    );

  
    expect(vinSet.length, 1);
    final vin = vinSet.first;

    expect(vin.toString(), equals("1GCHK23608F181070"));
    print(vin);
  });
}
