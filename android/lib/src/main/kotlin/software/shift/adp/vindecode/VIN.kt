package software.shift.adp.vindecode

class VIN private constructor(private val vin: String) {

    val wmi: String
    val vds: String
    val cd: String
    val vis: String

    init {
        wmi = vin.substring(0, 3)
        vds = vin.substring(3, 9)
        cd = vin.substring(9, 10)
        vis = vin.substring(9, 17)
    }

    companion object {
        /**
         * Tries to parse a VIN. Returns VIN if successful, otherwise null.
         */
        fun tryParseVIN(vin: String): VIN? {
            val normalizedVin = vin.uppercase()
            if (!Validator.validateCheckDigit(normalizedVin)) {
                return null
            }
            return  VIN(normalizedVin)
        }

        /**
         * Tries to parse a VIN. Returns VIN if successful, otherwise null.
         */
        fun tryParseVIN(vin: CharArray):  VIN? {
            if (vin.size != 17) return null

            val vinUpper = CharArray(17) {
                val c = vin[it]
                if (c in 'a'..'z') (c - 32).toChar() else c
            }

            if (!Validator.validateCheckDigit(vinUpper)) {
                return null
            }

            return VIN(String(vinUpper))
        }

        /**
         * Parses a VIN. Throws an exception if invalid.
         */
        fun parseVIN(vin: String): VIN {
            val normalizedVin = vin.uppercase()
            if (!Validator.validateCheckDigit(normalizedVin)) {
                throw IllegalArgumentException("Invalid VIN")
            }
            return VIN(normalizedVin)
        }
    }

    override fun toString(): String = vin

    override fun equals(other: Any?): Boolean {
        return other is VIN && this.vin.equals(other.vin, ignoreCase = true)
    }

    override fun hashCode(): Int {
        return vin.uppercase().hashCode()
    }
}
