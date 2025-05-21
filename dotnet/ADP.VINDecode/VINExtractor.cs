using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ShiftSoftware.ADP.VINDecode
{
    public class VINExtractor
    {
        public static HashSet<VIN> FindVINs(params string[] texts) => FindVINs(null, texts);

        public static HashSet<VIN> FindVINs(VINExtractionOptions options, params string[] texts)
        {
            var vins = new HashSet<VIN>();

            foreach (var text in texts)
            {
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                ReadOnlySpan<char> span = NormalizeInput(text, options);

                for (int i = 0; i <= span.Length - 17; i++)
                {
                    var sliced = span.Slice(i, 17);

                    if (VIN.TryParse(sliced, out VIN vin))  // Avoiding ToString()
                    {
                        vins.Add(vin);
                        continue;
                    }

                    // Try mutations
                    var mutations = VINMutator.GenerateMutations(options, sliced, 1000);

                    var validMutations = mutations
                        .Select(m =>
                        {
                            if (VIN.TryParse(m.Value, out var parsedVin))
                                return (vin: parsedVin, score: m.Score, mutations: m.MutationCount);

                            return (vin: (VIN)null, score: m.Score, mutations: m.MutationCount);
                        })
                        .Where(t => t.vin != null)
                        .OrderByDescending(t => t.score)
                        .ToList();

                    throw new Exception(string.Join("\r\n", validMutations));

                    if (validMutations.Any())
                    {
                        vins.Add(validMutations.First().vin);
                    }
                }
            }

            return vins;
        }

        private static ReadOnlySpan<char> NormalizeInput(string text, VINExtractionOptions options)
        {
            if (options == null) return text.AsSpan();

            string cleaned = text.Normalize(NormalizationForm.FormD);
            char[] buffer = new char[text.Length]; // Heap allocation since stackalloc can't be returned
            int index = 0;

            foreach (char c in cleaned)
            {
                if (options.RemoveWhiteSpace && char.IsWhiteSpace(c))
                    continue;

                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                    continue;

                buffer[index++] = c;
            }

            return new ReadOnlySpan<char>(buffer, 0, index);
        }
    }
}
