﻿using System;

namespace ADP.VINDecode
{
    public class VIN
    {
        private readonly string vin;
        public string WMI { get; }
        public string VDS { get; }
        public string CD { get; }
        public string VIS { get; }

        private VIN(string vin)
        {
            this.vin = vin;
            this.WMI = this.vin.Substring(0, 3);
            this.VDS = this.vin.Substring(3, 6);
            this.CD = this.vin.Substring(9, 1);
            this.VIS = this.vin.Substring(9, 8);
        }

        private VIN(ReadOnlySpan<char> vin)
        {
            this.vin = vin.ToString().ToUpperInvariant();
            this.WMI = vin.Slice(0, 3).ToString();
            this.VDS = vin.Slice(3, 6).ToString();
            this.CD = vin.Slice(9, 1).ToString();
            this.VIS = vin.Slice(9, 8).ToString();
        }

        /// <summary>
        /// Tries to parse a VIN. Returns true if successful, otherwise false.
        /// </summary>
        public static bool TryParse(string vin, out VIN result)
        {
            result = null;

            vin = vin.ToUpperInvariant();

            if (!Validator.ValidateCheckDigit(vin))
                return false;

            result = new VIN(vin);

            return true;
        }

        /// <summary>
        /// Tries to parse a VIN. Returns true if successful, otherwise false.
        /// </summary>
        public static bool TryParse(ReadOnlySpan<char> vin, out VIN result)
        {
            Span<char> vinUpper = stackalloc char[17]; // Allocate stack memory for uppercase conversion

            if (vin.Length != 17)
            {
                result = null;
                return false;
            }

            for (int i = 0; i < 17; i++)
            {
                char c = vin[i];

                if (c >= 'a' && c <= 'z') // Convert lowercase to uppercase
                    c = (char)(c - 32);

                vinUpper[i] = c;
            }

            if (!Validator.ValidateCheckDigit(vinUpper))
            {
                result = null;
                return false;
            }

            result = new VIN(vinUpper); // Ensure VIN constructor accepts ReadOnlySpan<char>

            return true;
        }

        /// <summary>
        /// Parses a VIN. Throws an exception if invalid.
        /// </summary>
        public static VIN Parse(string vin)
        {
            vin = vin.ToUpperInvariant();

            if (!Validator.ValidateCheckDigit(vin))
                throw new ArgumentException("Invalid VIN", nameof(vin));

            return new VIN(vin);
        }

        public override string ToString() => vin;
        public override bool Equals(object obj) => obj is VIN other && this.vin.Equals(other.vin, System.StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => vin.ToUpperInvariant().GetHashCode();

        public static implicit operator string(VIN vin) => vin.ToString();
    }
}