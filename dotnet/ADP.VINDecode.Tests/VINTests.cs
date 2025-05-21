using ShiftSoftware.ADP.VINDecode;
using System.Globalization;
using System.Text;
using Xunit.Abstractions;

namespace ADP.VINDecode.Tests
{
    public class VINTests
    {
        private readonly ITestOutputHelper output;
        public VINTests(ITestOutputHelper testOutputHelper)
        {
            output = testOutputHelper;
        }


        [Fact(DisplayName = "01. Equality")]
        public void Equality()
        {
            //var text = "JTFUA8ÄP508014798";
            //string normalized = text.Normalize(NormalizationForm.FormD);
            //string cleaned = new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            //output.WriteLine(cleaned);

            //Assert.False(VIN.TryParse(text, out _));

            //Assert.True(VIN.TryParse(cleaned, out _));

            //var text2 = "JTFUAŠÄP50801478";
            //string normalized2 = text2.Normalize(NormalizationForm.FormD);
            //string cleaned2 = new string(normalized2.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            //output.WriteLine(cleaned2);

            //Assert.False(VIN.TryParse(text2, out _));

            //Assert.True(VIN.TryParse(cleaned2, out _));

            //var badRead = "JTFUABẮPSO801478";
            //var candidates = VINMutator.GenerateMutations(badRead).ToList();

            //foreach (var c in candidates)
            //{
            //    output.WriteLine($"{c.Value} | Score: {c.Score}, Mutations: {c.MutationCount}");
            //}

            //return;

            var vinString1 = "19XFB2F57EE209415";
            var vinString2 = "19xfB2F57Ee209415";
            var vinString3 = "19XFb2F57eE209415";

            var vin1IsValid = VIN.TryParse(vinString1, out VIN vin1);
            var vin2IsValid = VIN.TryParse(vinString2, out VIN vin2);
            var vin3IsValid = VIN.TryParse(vinString3, out VIN vin3);

            Assert.True(vin1IsValid);
            Assert.True(vin2IsValid);
            Assert.True(vin3IsValid);

            Assert.Equal(vin1, vin2);
            Assert.Equal(vin1, vin3);
            Assert.Equal(vin2, vin3);

            Assert.Equal(vin1.GetHashCode(), vin2.GetHashCode());
            Assert.Equal(vin1.GetHashCode(), vin3.GetHashCode());
            Assert.Equal(vin2.GetHashCode(), vin3.GetHashCode());

            //Check the Implicit string converter
            Assert.Equal(vinString1.ToUpperInvariant(), vin1);
            Assert.Equal(vinString2.ToUpperInvariant(), vin2);
            Assert.Equal(vinString3.ToUpperInvariant(), vin3);
        }

        [Fact(DisplayName = "02. Components")]
        public void Components()
        {
            VIN.TryParse("19XFB2F57EE209415", out VIN vin);

            Assert.Equal("19X", vin.WMI);
            Assert.Equal("FB2F57", vin.VDS);
            Assert.Equal("E", vin.CD);
            Assert.Equal("EE209415", vin.VIS);
        }

        [Fact(DisplayName = "03. VIN Extractor: Leading and Trailing Symbols")]
        public void VinExtractor_LeadingAndTrailingSymbols()
        {
            var vinString = "*1FUJGLDRXblaY2150*";
            var vinString2 = "* 1fuJGLDRXBLAY2150 *";

            var vinSet = VINExtractor.FindVINs(vinString);
            var vinSet2 = VINExtractor.FindVINs(vinString2);

            Assert.Single(vinSet);
            Assert.Single(vinSet2);

            var vin = vinSet.First();
            var vin2 = vinSet2.First();

            Assert.Equal("1FUJGLDRXBLAY2150", vin);
            Assert.Equal("1FUJGLDRXBLAY2150", vin2);
        }

        [Fact(DisplayName = "04. VIN Extractor: Leading and Trailing Text")]
        public void VinExtractor_LeadingAndTrailingText()
        {
            var vinString = "VIN: 1GKEK63U83J235900 (Brand Name)";

            var vinSet = VINExtractor.FindVINs(vinString);

            Assert.Single(vinSet);

            var vin = vinSet.First();

            Assert.Equal("1GKEK63U83J235900", vin);
        }

        [Fact(DisplayName = "05. VIN Extractor: Multiple VINs in Same Line")]
        public void VinExtractor_MultipleVINsInSameLine()
        {
            var vinString = "K249 2HGES25724H591789 239 JH4DC54865S056007";

            var vinSet = VINExtractor.FindVINs(vinString);

            Assert.Equal(2, vinSet.Count);

            var vin1 = vinSet.First();
            var vin2 = vinSet.Skip(1).First();

            Assert.Equal("2HGES25724H591789", vin1);
            Assert.Equal("JH4DC54865S056007", vin2);
        }

        [Fact(DisplayName = "06. VIN Extractor: Multiple VINs in Same Line Without Whitespace (Produces False Positives)")]
        public void VinExtractor_MultipleVINsInSameLineWithoutWhiteSpace()
        {
            var vinString1 = "5N1AA08A15N732965";
            var vinString2 = "1FMCU9G99FUA05110";

            var combinedVINLine = $"KKJ948{vinString1}{vinString2}113948LJH";

            var vinSet = VINExtractor.FindVINs(combinedVINLine);

            Assert.Equal(4, vinSet.Count);

            var vin1 = vinSet.ElementAt(0);
            var vin2 = vinSet.ElementAt(1);
            var vin3 = vinSet.ElementAt(2);
            var vin4 = vinSet.ElementAt(3);

            // False Positive
            // Not only passes the check digit validation but also has a WMI that seems to be valid according to WMI wikiooks
            Assert.Equal("485N1AA08A15N7329", vin1);

            Assert.Equal(vinString1, vin2);
            Assert.Equal(vinString2, vin3);

            // False Positive 2
            Assert.Equal("FUA05110113948LJH", vin4);

            //Same test with whitespace does not produce false positives
            Assert.Equal(2, VINExtractor.FindVINs($"KKJ948 {vinString1}{vinString2} 113948LJH").Count);
        }

        [Fact(DisplayName = "07. VIN Extractor: Whitespace inside VIN")]
        public void VinExtractor_WhiteSpaceInsideVIN()
        {
            var vinString = "3FAFP 3136YR25 9819";

            var vin = VINExtractor.FindVINs(
                new VINExtractionOptions { RemoveWhiteSpace = true },
                vinString
            );

            Assert.Single(vin);

            Assert.Equal("3FAFP3136YR259819", vin.First());
        }

        [Fact(DisplayName = "08. VIN Extractor: Multiple Lines")]
        public void VinExtractor_MultipleLines()
        {
            var line1 = "Engine Size: 4.0 L";
            var line2 = "VIN: 1FDKF37GXVEB34368 - 2025-03-26";
            var line3 = "Brand: Some Brand";
            var line4 = "Color: White";

            var vinSet = VINExtractor.FindVINs(
                line1, line2, line3, line4
            );

            Assert.Single(vinSet);

            Assert.Equal("1FDKF37GXVEB34368", vinSet.First());

            var multiLineString =
            """
            Engine Size: 4.0 L
            VIN: 1FDKF37GXVEB34368 - 2025-03-26
            Brand: Some Brand
            Color: White
            """;

            var multiLineVinSet = VINExtractor.FindVINs(multiLineString);

            Assert.Single(multiLineVinSet);

            Assert.Equal("1FDKF37GXVEB34368", multiLineVinSet.First());
        }

        [Fact(DisplayName = "09. VIN Extractor: Multiple VINs in Multiple lines")]
        public void VinExtractor_MultipleVINsInMultipleLinesWithoutWhiteSpace()
        {
            var vinString =
                """
                   VIN 1 & VIN2:
                   2C4RDGBG6ER101765
                   3GNDA13D48S626939

                   VIN 3:
                   1GNES16S932111256 - 2C4RDGCG2ER226597
                """;

            var vinSet = VINExtractor.FindVINs(vinString);

            Assert.Equal(4, vinSet.Count);
            Assert.Equal(4, VINExtractor.FindVINs(new VINExtractionOptions { RemoveWhiteSpace = true }, vinString).Count);

            var vin1 = vinSet.ElementAt(0);
            var vin2 = vinSet.ElementAt(1);
            var vin3 = vinSet.ElementAt(2);
            var vin4 = vinSet.ElementAt(3);

            Assert.Equal("2C4RDGBG6ER101765", vin1);
            Assert.Equal("3GNDA13D48S626939", vin2);
            Assert.Equal("1GNES16S932111256", vin3);
            Assert.Equal("2C4RDGCG2ER226597", vin4);
        }

        [Fact(DisplayName = "10. VIN Extractor: Character Replacement")]
        public void VinExtractor_CharacterReplacement()
        {
            var vinString = "IGCHK236O8F181Q70";

            //Without Character Replacement
            Assert.Empty(VINExtractor.FindVINs(vinString));

            var vinSet = VINExtractor.FindVINs(
                new VINExtractionOptions { MutationConfidence = MutationConfidence.High },
                vinString
            );

            Assert.Single(vinSet);

            var vin = vinSet.First();

            Assert.Equal("1GCHK23608F181070", vin);
        }

        [Fact(DisplayName = "11. VIN Mutator")]
        public void VinExtractor_Mutator()
        {
            var vins = new List<string>
            {
                "JTFUASAP508014798",
                //"JTFUABẮP508014798",
                //"JTFUAŠÄP508014798",
                //"JTFUA&AP508014798",
            };

            foreach (var vinReading in vins)
            {
                var vinSet = VINExtractor.FindVINs(
                    new VINExtractionOptions { MutationConfidence = MutationConfidence.Low },
                    vinReading
                );

                if (vinSet.Count == 0)
                {
                    this.output.WriteLine($"No VIN found for {vinReading}");
                }

                Assert.Single(vinSet);

                var vin = vinSet.First();
                           //"JTFUASAP508014798"
                           //"JTFUASAP50B014798"
                Assert.Equal("JTFUA8AP508014798", vin);
            }
        }

        [Fact(DisplayName = "12. VIN Mutator: Multiple Candidates")]
        public void VinExtractor_Mutator_MultipleCandidates()
        {
            var lines = System.IO.File.ReadAllLines(@"C:\Web\New Text Document.txt");

            var uniqueLines = lines.Distinct();

            var parsed = uniqueLines.Select(x => new
            {
                From = x[0],
                To = x[2],
            });

            var cleaned = parsed
                .Where(x => x.To != 'I' && x.To != 'Q' && x.To != 'O')
                .Where(x => x.From != 'I' && x.From != 'Q' && x.From != 'O')
                .ToList();

            //this.output.WriteLine($"Total: {cleaned.Count}");

            //this.output.WriteLine(string.Join("\r\n", cleaned.Select(x=> $"{x.From},{x.To}")));

            foreach (var item in cleaned)
            {
                this.output.WriteLine($"new CharacterMutation {{ From = '{item.From}', To = '{item.To}', Confidence = MutationConfidence.Low }},");
            }
        }
    }
}