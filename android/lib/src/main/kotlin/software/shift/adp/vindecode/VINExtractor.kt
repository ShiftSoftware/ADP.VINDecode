package software.shift.adp.vindecode

class VINExtractionOptions(
    val removeWhiteSpace: Boolean = false,
    val mutateSpecialCharacters: Boolean = false
)

object VINExtractor {

    fun findVINs(vararg texts: String): Set<VIN> = findVINs(null, *texts)

    fun findVINs(options: VINExtractionOptions?, vararg texts: String): Set<VIN> {
        val vins = mutableSetOf<VIN>()

        for (text in texts) {
            if (text.isBlank()) continue

            val span = normalizeInput(text, options)

            for (i in 0..(span.size - 17)) {
                val slice = span.sliceArray(i until i + 17)
                val vin = VIN.tryParseVIN(slice)
                if ( vin != null) {
                    vins.add(vin)
                }
            }
        }

        return vins
    }

    private fun normalizeInput(text: String, options: VINExtractionOptions?): CharArray {
        if (options == null) return text.toCharArray()

        val buffer = CharArray(text.length)
        var index = 0

        for (c in text) {
            if (options.removeWhiteSpace && c == ' ') continue

            buffer[index++] = when {
                options.mutateSpecialCharacters && c == 'I' -> '1'
                options.mutateSpecialCharacters && (c == 'O' || c == 'Q') -> '0'
                else -> c
            }
        }

        return buffer.copyOfRange(0, index)
    }
}
