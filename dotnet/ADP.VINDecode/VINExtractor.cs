using System;
using System.Collections.Generic;

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
                    if (VIN.TryParse(span.Slice(i, 17), out VIN vin))  // Avoiding ToString()
                    {
                        vins.Add(vin);
                    }
                }
            }

            return vins;
        }

        private static ReadOnlySpan<char> NormalizeInput(string text, VINExtractionOptions options)
        {
            if (options == null) return text.AsSpan();

            char[] buffer = new char[text.Length]; // Heap allocation since stackalloc can't be returned
            int index = 0;

            foreach (char c in text)
            {
                if (options.RemoveWhiteSpace && c == ' ')
                    continue;

                if (options.MutateSpecialCharacters)
                {
                    if (c == 'I') buffer[index++] = '1';
                    else if (c == 'O' || c == 'Q') buffer[index++] = '0';
                    else buffer[index++] = c;
                }
                else
                {
                    buffer[index++] = c;
                }
            }

            return new ReadOnlySpan<char>(buffer, 0, index);
        }
    }
}
