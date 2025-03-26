namespace ADP.VINDecode
{
    public class VIN
    {
        private readonly string vin;
        public string WMI { get; }
        public string VDS { get; }
        public string CD { get; }
        public string VIS { get; }

        public VIN(string vin)
        {
            this.vin = vin.ToUpperInvariant();

            if (!Validator.IsValidVIN(this.vin))
                throw new System.Exception("Invalid VIN");

            this.WMI = this.vin.Substring(0, 3);
            this.VDS = this.vin.Substring(3, 6);
            this.CD = this.vin.Substring(9, 1);
            this.VIS = this.vin.Substring(9, 8);
        }

        public override string ToString()
        {
            return vin; // Return stored uppercase VIN
        }

        public override bool Equals(object obj)
        {
            if (obj is VIN other)
            {
                return this.vin.Equals(other.vin, System.StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return vin.ToUpperInvariant().GetHashCode(); // Case-insensitive hash
        }
    }
}