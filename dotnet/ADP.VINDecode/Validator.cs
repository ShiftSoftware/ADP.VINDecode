using System;
using System.Collections.Generic;

namespace ShiftSoftware.ADP.VINDecode
{
    public class Validator
    {
        private static readonly Dictionary<char, int> TransliterationTable = new Dictionary<char, int>
        {
            {'0', 0},
            {'1', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},

            {'A', 1},
            {'B', 2},
            {'C', 3},
            {'D', 4},
            {'E', 5},
            {'F', 6},
            {'G', 7},
            {'H', 8},

            {'J', 1},
            {'K', 2},
            {'L', 3},
            {'M', 4},
            {'N', 5},

            {'P', 7},

            {'R', 9},

            {'S', 2},
            {'T', 3},
            {'U', 4},
            {'V', 5},
            {'W', 6},
            {'X', 7},
            {'Y', 8},
            {'Z', 9}
        };

        private static readonly int[] WeightTable = { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };

        /// <summary>
        /// Runs the check digit validation on the provided (UPPERCASE) VIN
        /// </summary>
        /// <param name="vin">VIN to run the check digit validation. Should be in Uppercase</param>
        /// <returns>true if the VIN passes the check digit check. Otherwise, false.</returns>
        public static bool ValidateCheckDigit(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
                return false;

            return ValidateCheckDigit(vin.AsSpan());
        }


        /// <summary>
        /// Runs the check digit validation on the provided (UPPERCASE) VIN
        /// </summary>
        /// <param name="vin">VIN to run the check digit validation. Should be in Uppercase</param>
        /// <returns>true if the VIN passes the check digit check. Otherwise, false.</returns>
        public static bool ValidateCheckDigit(ReadOnlySpan<char> vin)
        {
            if (vin.Length != 17)
                return false;

            int sum = 0;

            for (int i = 0; i < vin.Length; i++)
            {
                if (!TransliterationTable.TryGetValue(vin[i], out int value))
                    return false;

                sum += value * WeightTable[i];
            }

            int remainder = sum % 11;
            char expectedCheckDigit = remainder == 10 ? 'X' : (char)('0' + remainder);

            return vin[8] == expectedCheckDigit;
        }
    }
}