using System;
using System.Collections.Generic;

namespace ADP.VINDecode
{
    public class Extractor
    {
        public static HashSet<VIN> ExtractVINsFromText(params string[] inputTexts)
        {
            var vinSet = new HashSet<VIN>();

            foreach (var text in inputTexts)
            {
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                ReadOnlySpan<char> textSpan = text.AsSpan();

                for (int i = 0; i <= textSpan.Length - 17; i++)
                {
                    var candidateVIN = textSpan.Slice(i, 17).ToString();

                    if (VIN.TryParse(candidateVIN, out VIN vin))
                    {
                        vinSet.Add(vin);
                    }
                }
            }

            return vinSet;
        }
    }
}