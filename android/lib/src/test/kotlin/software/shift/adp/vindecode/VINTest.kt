package software.shift.adp.vindecode

import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertTrue

class VINTest {
    @Test
    fun `01 Equality`() {
        val vinString1 = "19XFB2F57EE209415"
        val vinString2 = "19xfB2F57Ee209415"
        val vinString3 = "19XFb2F57eE209415"

        val vin1 = VIN.tryParseVIN(vinString1)  // Assuming `parse` or equivalent method in VIN class
        val vin2 = VIN.tryParseVIN(vinString2)
        val vin3 = VIN.tryParseVIN(vinString3)

        // Assuming parse method returns a result similar to TryParse
        assertTrue(vin1 != null)
        assertTrue(vin2 != null)
        assertTrue(vin3 != null)

        assertEquals(vin1, vin2)
        assertEquals(vin1, vin3)
        assertEquals(vin2, vin3)

        assertEquals(vin1.hashCode(), vin2.hashCode())
        assertEquals(vin1.hashCode(), vin3.hashCode())
        assertEquals(vin2.hashCode(), vin3.hashCode())

        // Check the Implicit string converter
        assertEquals(vinString1.uppercase(), vin1.toString())
        assertEquals(vinString2.uppercase(), vin2.toString())
        assertEquals(vinString3.uppercase(), vin3.toString())
    }

    @Test
    fun `02 Components`() {  // No period in the test name
        val vin = VIN.tryParseVIN("19XFB2F57EE209415")  // Assuming `parse` or equivalent method in VIN class

        assertEquals("19X", vin?.wmi)
        assertEquals("FB2F57", vin?.vds)
        assertEquals("E", vin?.cd)
        assertEquals("EE209415", vin?.vis)
    }

    @Test
    fun `03 VIN Extractor - Leading and Trailing Symbols`() {  // Removed period and used camelCase
        val vinString = "*1FUJGLDRXblaY2150*"
        val vinString2 = "* 1fuJGLDRXBLAY2150 *"

        val vinSet = VINExtractor.findVINs(vinString)  // Assuming `findVINs` method in VINExtractor
        val vinSet2 = VINExtractor.findVINs(vinString2)

        assertTrue(vinSet.size == 1)
        assertTrue(vinSet2.size == 1)

        val vin = vinSet.first()
        val vin2 = vinSet2.first()

        assertEquals(expected = "1FUJGLDRXBLAY2150", actual = vin.toString())
        assertEquals(expected = "1FUJGLDRXBLAY2150", actual = vin2.toString())
    }

    @Test
    fun `04 VIN Extractor - Leading and Trailing Text`() {  // Removed period and used camelCase
        val vinString = "VIN: 1GKEK63U83J235900 (Brand Name)"

        val vinSet = VINExtractor.findVINs(vinString)  // Assuming `findVINs` method in VINExtractor

        assertTrue(vinSet.size == 1)

        val vin = vinSet.first()

        assertEquals("1GKEK63U83J235900", vin.toString())
    }

    @Test
    fun `05 VIN Extractor - Multiple VINs in Same Line`() {  // Removed period and used camelCase
        val vinString = "K249 2HGES25724H591789 239 JH4DC54865S056007"

        val vinSet = VINExtractor.findVINs(vinString)  // Assuming `findVINs` method in VINExtractor

        assertEquals(2, vinSet.size)

        val vin1 = vinSet.first()
        val vin2 = vinSet.drop(1).first()

        assertEquals("2HGES25724H591789", vin1.toString())
        assertEquals("JH4DC54865S056007", vin2.toString())
    }

    @Test
    fun `06 VIN Extractor - Multiple VINs in Same Line Without Whitespace (Produces False Positives)`() {  // Removed period and used camelCase
        val vinString1 = "5N1AA08A15N732965"
        val vinString2 = "1FMCU9G99FUA05110"

        val combinedVINLine = "KKJ948$vinString1$vinString2" + "113948LJH"

        val vinSet = VINExtractor.findVINs(combinedVINLine).toList() // Assuming `findVINs` method in VINExtractor

        assertEquals(4, vinSet.size)

        val vin1 = vinSet[0].toString()  // Convert to string explicitly
        val vin2 = vinSet[1].toString()  // Convert to string explicitly
        val vin3 = vinSet[2].toString()  // Convert to string explicitly
        val vin4 = vinSet[3].toString()  // Convert to string explicitly

        // False Positive
        assertEquals("485N1AA08A15N7329", vin1)

        assertEquals(vinString1, vin2)
        assertEquals(vinString2, vin3)

        // False Positive 2
        assertEquals("FUA05110113948LJH", vin4)

        // Same test with whitespace does not produce false positives
        assertEquals(2, VINExtractor.findVINs("KKJ948 $vinString1$vinString2 113948LJH").size)
    }

    @Test
    fun `07 VIN Extractor - Whitespace inside VIN`() {  // Removed period and used camelCase
        val vinString = "3FAFP 3136YR25 9819"

        val vin = VINExtractor.findVINs(
            VINExtractionOptions(removeWhiteSpace = true),  // Assuming `removeWhiteSpace` is a named parameter
            vinString
        )

        assertEquals(1, vin.size)

        assertEquals("3FAFP3136YR259819", vin.first().toString())
    }

    @Test
    fun `08 VIN Extractor - Multiple Lines`() {
        val line1 = "Engine Size: 4.0 L"
        val line2 = "VIN: 1FDKF37GXVEB34368 - 2025-03-26"
        val line3 = "Brand: Some Brand"
        val line4 = "Color: White"

        val vinSet = VINExtractor.findVINs(line1, line2, line3, line4)

        assertEquals(1, vinSet.size)
        assertEquals("1FDKF37GXVEB34368", vinSet.first().toString())

        val multiLineString = """
            Engine Size: 4.0 L
            VIN: 1FDKF37GXVEB34368 - 2025-03-26
            Brand: Some Brand
            Color: White
        """

        val multiLineVinSet = VINExtractor.findVINs(multiLineString)

        assertEquals(1, multiLineVinSet.size)
        assertEquals("1FDKF37GXVEB34368", multiLineVinSet.first().toString())
    }

    @Test
    fun `09 VIN Extractor - Multiple VINs in Multiple lines`() {
        val vinString = """
            VIN 1 & VIN2:
            2C4RDGBG6ER101765
            3GNDA13D48S626939

            VIN 3:
            1GNES16S932111256 - 2C4RDGCG2ER226597
        """

        val vinSet = VINExtractor.findVINs(vinString).toList()

        assertEquals(4, vinSet.size)
        assertEquals(4, VINExtractor.findVINs(
            VINExtractionOptions(removeWhiteSpace = true),
            vinString
        ).size)

        val vin1 = vinSet[0].toString()  // Convert to string explicitly
        val vin2 = vinSet[1].toString()  // Convert to string explicitly
        val vin3 = vinSet[2].toString()  // Convert to string explicitly
        val vin4 = vinSet[3].toString()  // Convert to string explicitly

        assertEquals("2C4RDGBG6ER101765", vin1)
        assertEquals("3GNDA13D48S626939", vin2)
        assertEquals("1GNES16S932111256", vin3)
        assertEquals("2C4RDGCG2ER226597", vin4)
    }

    @Test
    fun `10 VIN Extractor - Character Replacement`() {
        val vinString = "IGCHK236O8F181Q70"

        // Without Character Replacement
        assertTrue(VINExtractor.findVINs(vinString).isEmpty())

        val vinSet = VINExtractor.findVINs(
            VINExtractionOptions(mutateSpecialCharacters = true),
            vinString
        )

        assertEquals(1, vinSet.size)

        val vin = vinSet.first().toString()  // Convert explicitly to string if needed

        assertEquals("1GCHK23608F181070", vin)
    }
}
