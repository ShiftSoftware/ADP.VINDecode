namespace ShiftSoftware.ADP.VINDecode
{
    public class VINExtractionOptions
    {
        /// <summary>
        /// Removes all whitespace from the input text.
        /// </summary>
        public bool RemoveWhiteSpace { get; set; }

        /// <summary>
        /// Replaces unallowed VIN characters with their allowed counterparts.
        /// I -> 1, O -> 0, Q -> 0
        /// </summary>
        public bool MutateSpecialCharacters { get; set; }
    }
}