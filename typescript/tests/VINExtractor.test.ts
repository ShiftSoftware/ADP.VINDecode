import { describe, expect, test } from "vitest"
import VINExtractor from "../src/VINExtractor"

describe("VIN Extractor Tests", () => {
  test("03. VIN Extractor: Leading and Trailing Symbols", () => {
    const vin1 = VINExtractor.findVINs("*1FUJGLDRXblaY2150*")
    const vin2 = VINExtractor.findVINs("* 1fuJGLDRXBLAY2150 *")

    expect(vin1).toHaveLength(1)
    expect(vin2).toHaveLength(1)

    expect(vin1[0].vin).toBe("1FUJGLDRXBLAY2150")
    expect(vin2[0].vin).toBe("1FUJGLDRXBLAY2150")
  })

  test("04. VIN Extractor: Leading and Trailing Text", () => {
    const vins = VINExtractor.findVINs("VIN: 1GKEK63U83J235900 (Brand Name)")

    expect(vins).toHaveLength(1)
    expect(vins[0].vin).toBe("1GKEK63U83J235900")
  })

  test("05. VIN Extractor: Multiple VINs in Same Line", () => {
    const vinSet = VINExtractor.findVINs(
      "K249 2HGES25724H591789 239 JH4DC54865S056007"
    )

    expect(vinSet).toHaveLength(2)

    expect(vinSet[0].vin).toBe("2HGES25724H591789")
    expect(vinSet[1].vin).toBe("JH4DC54865S056007")
  })

  test("06. VIN Extractor: Multiple VINs in Same Line Without Whitespace (False Positives)", () => {
    const vin1 = "5N1AA08A15N732965"
    const vin2 = "1FMCU9G99FUA05110"

    const combined = `KKJ948${vin1}${vin2}113948LJH`

    const result = VINExtractor.findVINs(combined)

    expect(result).toHaveLength(4)

    // False Positive
    // Not only passes the check digit validation but also has a WMI that seems to be valid according to WMI wikiooks
    expect(result[0].vin).toBe("485N1AA08A15N7329")

    expect(result[1].vin).toBe(vin1)
    expect(result[2].vin).toBe(vin2)

    // False Positive 2
    expect(result[3].vin).toBe("FUA05110113948LJH")

    //Same test with whitespace does not produce false positives
    const cleaned = VINExtractor.findVINs(`KKJ948 ${vin1}${vin2} 113948LJH`)
    expect(cleaned).toHaveLength(2)
  })

  test("07. VIN Extractor: Whitespace inside VIN", () => {
    const vinString = "3FAFP 3136YR25 9819"

    const result = VINExtractor.findVINs(vinString, { RemoveWhiteSpace: true })

    expect(result).toHaveLength(1)

    expect(result[0].vin).toBe("3FAFP3136YR259819")
  })

  test("08. VIN Extractor: Multiple Lines", () => {
    const lines = [
      "Engine Size: 4.0 L",
      "VIN: 1FDKF37GXVEB34368 - 2025-03-26",
      "Brand: Some Brand",
      "Color: White",
    ]
    const vinSet = VINExtractor.findVINs(...lines)

    expect(vinSet).toHaveLength(1)
    expect(vinSet[0].vin).toBe("1FDKF37GXVEB34368")

    const multiLine = `
        Engine Size: 4.0 L
        VIN: 1FDKF37GXVEB34368 - 2025-03-26
        Brand: Some Brand
        Color: White
        `

    const vinSet2 = VINExtractor.findVINs(multiLine)

    expect(vinSet2).toHaveLength(1)

    expect(vinSet2[0].vin).toBe("1FDKF37GXVEB34368")
  })

  test("09. VIN Extractor: Multiple VINs in Multiple Lines", () => {
    const content = `
        VIN 1 & VIN2:
        2C4RDGBG6ER101765
        3GNDA13D48S626939

        VIN 3:
        1GNES16S932111256 - 2C4RDGCG2ER226597
      `

    const vinSet = VINExtractor.findVINs(content)
    expect(vinSet).toHaveLength(4)
    const vinSet2 = VINExtractor.findVINs(content, { RemoveWhiteSpace: true })
    expect(vinSet2).toHaveLength(4)

    expect(vinSet[0].vin).toBe("2C4RDGBG6ER101765")
    expect(vinSet[1].vin).toBe("3GNDA13D48S626939")
    expect(vinSet[2].vin).toBe("1GNES16S932111256")
    expect(vinSet[3].vin).toBe("2C4RDGCG2ER226597")
  })

  test("10. VIN Extractor: Character Replacement", () => {
    const invalidVin = "IGCHK236O8F181Q70"

    expect(VINExtractor.findVINs(invalidVin)).toHaveLength(0)

    const result = VINExtractor.findVINs(invalidVin, {
      MutateSpecialCharacters: true,
    })

    expect(result).toHaveLength(1)
    expect(result[0].vin).toBe("1GCHK23608F181070")
  })
})
