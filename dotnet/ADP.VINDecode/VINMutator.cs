using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ShiftSoftware.ADP.VINDecode
{
    public enum MutationConfidence
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Absolute = int.MaxValue,
    }

    public class CharacterMutation
    {
        public char From { get; set; }
        public char To { get; set; }
        public MutationConfidence Confidence { get; set; }
    }

    public static class VINCharacterMutations
    {
        public static readonly List<CharacterMutation> Mutations = new List<CharacterMutation>()
            {
                // High-confidence (officially invalid VIN characters)
                new CharacterMutation { From = 'O', To = '0', Confidence = MutationConfidence.High },
                new CharacterMutation { From = 'O', To = 'C', Confidence = MutationConfidence.High },
                new CharacterMutation { From = 'Q', To = '0', Confidence = MutationConfidence.High },
                new CharacterMutation { From = 'Q', To = 'C', Confidence = MutationConfidence.High },
                new CharacterMutation { From = 'Q', To = 'G', Confidence = MutationConfidence.High },
                new CharacterMutation { From = 'I', To = '1', Confidence = MutationConfidence.High },

                // Low-confidence (ambiguous glyphs)
                new CharacterMutation { From = '8', To = 'B', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '8', To = '3', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '&', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '∞', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'S', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '3', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'B', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'S', To = '5', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '5', To = 'S', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '5', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '8', To = 'S', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'G', To = '6', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'G', To = 'C', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '6', To = 'G', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'C', To = 'G', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'Z', To = '2', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '2', To = 'Z', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'D', To = '0', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '0', To = 'D', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'U', To = 'V', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'V', To = 'U', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '£', To = 'E', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '€', To = 'C', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '1', To = 'L', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'L', To = '1', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '7', To = 'T', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'T', To = '7', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'P', To = 'R', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'R', To = 'P', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '4', To = 'A', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'A', To = '4', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'K', To = 'X', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'X', To = 'K', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'M', To = 'N', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'N', To = 'M', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'J', To = '1', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '1', To = 'J', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'Y', To = 'V', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'V', To = 'Y', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'W', To = 'M', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'M', To = 'W', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'U', To = 'Y', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'Y', To = 'U', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'X', To = 'Y', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'Y', To = 'X', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'Z', To = 'S', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'S', To = 'Z', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '5', To = 'G', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'G', To = '5', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '2', To = 'S', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'S', To = '2', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '3', To = 'E', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'E', To = '3', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '6', To = 'B', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'B', To = '6', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '9', To = 'G', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'G', To = '9', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '8', To = 'A', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = 'A', To = '8', Confidence = MutationConfidence.Low },
                new CharacterMutation { From = '1', To = 'T', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'T', To = '1', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'P', To = 'B', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'B', To = 'P', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'R', To = 'B', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'B', To = 'R', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'K', To = 'H', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'H', To = 'K', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'H', To = 'N', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'N', To = 'H', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'F', To = 'P', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'P', To = 'F', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'A', To = 'R', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'R', To = 'A', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = '7', To = 'Y', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'Y', To = '7', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'D', To = 'B', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'B', To = 'D', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = 'Z', To = '7', Confidence = MutationConfidence.Low },
                //new CharacterMutation { From = '7', To = 'Z', Confidence = MutationConfidence.Low },
            };

        public static Dictionary<char, List<CharacterMutation>> Lookup = Mutations
            .GroupBy(m => m.From)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public class MutatedVINCandidate
    {
        public string Value { get; set; }
        public int MutationCount { get; set; }
        public int HighConfidence { get; set; }
        public int MediumConfidence { get; set; }
        public int LowConfidence { get; set; }

        public double Score { get; set; }
    }

    public static class VINMutator
    {
        public static IEnumerable<MutatedVINCandidate> GenerateMutations(VINExtractionOptions options, ReadOnlySpan<char> rawInput, int maxCandidates = 10)
        {
            ReadOnlySpan<char> baseSpan = rawInput;

            string baseString = null;

            var minimumConfidence = options?.MutationConfidence ?? MutationConfidence.Absolute;

            // Step 1: Apply high-confidence mutations if enabled
            if (minimumConfidence <= MutationConfidence.High)
            {
                // ApplyHighConfidenceMutations accepts span now
                Span<char> mutatedBuffer = stackalloc char[rawInput.Length];
                int written = ApplyHighConfidenceMutations(rawInput, mutatedBuffer);
                baseString = mutatedBuffer.Slice(0, written).ToString();
            }
            else
            {
                baseString = baseSpan.ToString();
            }

            if (minimumConfidence <= MutationConfidence.Medium)
            {
                // Step 3: Generate mutations
                return Generate(baseString, maxCandidates, minimumConfidence);
            }

            else
                return new List<MutatedVINCandidate> { new MutatedVINCandidate { Value = baseString } };
        }

        //private static string RemoveDiacritics(string input)
        //{
        //    var chars = input
        //        .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        //        .ToArray();
        //    return new string(chars);
        //}

        private static int ApplyHighConfidenceMutations(ReadOnlySpan<char> input, Span<char> output)
        {
            int written = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (VINCharacterMutations.Lookup.TryGetValue(c, out var list))
                {
                    var highConfidence = list.FirstOrDefault(m => m.Confidence == MutationConfidence.High);
                    output[written++] = highConfidence?.To ?? c;
                }
                else
                {
                    output[written++] = c;
                }
            }

            return written; // number of characters written to output span
        }

        public static class VINCandidateScorer
        {
            public static double Score(MutatedVINCandidate candidate)
            {
                if (candidate == null) return 0.0;

                double score = 1.0;

                // Mutation penalties
                score -= candidate.HighConfidence * 0.05;   // small penalty
                score -= candidate.MediumConfidence * 0.1;  // bigger penalty
                score -= candidate.LowConfidence * 0.2;     // heavy penalty

                // General penalty for number of mutations (extra penalty)
                score -= 0.02 * candidate.MutationCount;

                // (Optional) If parsed VIN is valid, boost score a bit
                if (VIN.TryParse(candidate.Value, out _))
                {
                    score += 0.1;
                }

                // Clamp between 0 and 1
                return Math.Max(0.0, Math.Min(1.0, score));
            }
        }

        private static IEnumerable<MutatedVINCandidate> Generate(string baseString, int maxCandidates, MutationConfidence minimumConfidence)
        {
            var results = new List<MutatedVINCandidate>();
            var initial = new MutatedVINCandidate { Value = baseString };
            var queue = new Queue<(char[] chars, int index, MutatedVINCandidate candidate)>();
            queue.Enqueue((baseString.ToCharArray(), 0, initial));

            while (queue.Count > 0 /*&& results.Count < maxCandidates*/)
            {
                var (chars, index, candidate) = queue.Dequeue();

                if (index >= chars.Length)
                {
                    candidate.Value = new string(chars);
                    candidate.Score = VINCandidateScorer.Score(candidate); // Recalculate score after full mutation
                    results.Add(candidate);
                    continue;
                }

                var c = chars[index];

                // Expand mutations
                if (VINCharacterMutations.Lookup.TryGetValue(c, out var mutations))
                {
                    foreach (var m in mutations.Where(m => m.Confidence >= minimumConfidence))
                    {
                        var mutatedChars = (char[])chars.Clone();
                        mutatedChars[index] = m.To;

                        var newCandidate = new MutatedVINCandidate
                        {
                            MutationCount = candidate.MutationCount + 1,
                            HighConfidence = candidate.HighConfidence + (m.Confidence == MutationConfidence.High ? 1 : 0),
                            MediumConfidence = candidate.MediumConfidence + (m.Confidence == MutationConfidence.Medium ? 1 : 0),
                            LowConfidence = candidate.LowConfidence + (m.Confidence == MutationConfidence.Low ? 1 : 0),
                        };

                        queue.Enqueue((mutatedChars, index + 1, newCandidate));
                    }
                }

                // Also keep the original path without mutation
                queue.Enqueue((chars, index + 1, candidate));
            }

            return results
                .OrderByDescending(c => c.Score)
                .ThenBy(c => c.MutationCount)
                .Take(maxCandidates);
        }
    }
}