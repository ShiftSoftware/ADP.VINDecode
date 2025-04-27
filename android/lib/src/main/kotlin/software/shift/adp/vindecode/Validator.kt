package software.shift.adp.vindecode

object Validator {

    private val transliterationTable = mapOf(
        '0' to 0, '1' to 1, '2' to 2, '3' to 3, '4' to 4,
        '5' to 5, '6' to 6, '7' to 7, '8' to 8, '9' to 9,

        'A' to 1, 'B' to 2, 'C' to 3, 'D' to 4, 'E' to 5,
        'F' to 6, 'G' to 7, 'H' to 8,

        'J' to 1, 'K' to 2, 'L' to 3, 'M' to 4, 'N' to 5,

        'P' to 7,

        'R' to 9,

        'S' to 2, 'T' to 3, 'U' to 4, 'V' to 5, 'W' to 6,
        'X' to 7, 'Y' to 8, 'Z' to 9
    )

    private val weightTable = listOf(8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2)

    /**
     * Runs the check digit validation on the provided (UPPERCASE) VIN
     * @param vin VIN to validate. Should be in uppercase
     * @return true if VIN is valid, false otherwise
     */
    fun validateCheckDigit(vin: String): Boolean {
        if (vin.isBlank() || vin.length != 17) return false
        return validateCheckDigit(vin.toCharArray())
    }

    /**
     * Runs the check digit validation on the provided (UPPERCASE) VIN
     * @param vin VIN to validate. Should be in uppercase
     * @return true if VIN is valid, false otherwise
     */
    fun validateCheckDigit(vin: CharArray): Boolean {
        if (vin.size != 17) return false

        var sum = 0

        for (i in vin.indices) {
            val value = transliterationTable[vin[i]] ?: return false
            sum += value * weightTable[i]
        }

        val remainder = sum % 11
        val expectedCheckDigit = if (remainder == 10) 'X' else ('0' + remainder)

        return vin[8] == expectedCheckDigit
    }
}
