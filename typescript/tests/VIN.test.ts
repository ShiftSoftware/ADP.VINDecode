import { describe, test, expect } from "vitest"

import VIN from "../src/VIN"
// import { VINExtractor } from "../src/VINExtractor"
// import { VINExtractionOptions } from "../src/VINExtractionOptions" // if applicable

describe("VIN Tests", () => {
  test("01. Equality", () => {
    const vinString1 = "19XFB2F57EE209415"
    const vinString2 = "19xfB2F57Ee209415"
    const vinString3 = "19XFb2F57eE209415"

    const [vin1IsValid, vin1] = VIN.tryParse(vinString1)
    const [vin2IsValid, vin2] = VIN.tryParse(vinString2)
    const [vin3IsValid, vin3] = VIN.tryParse(vinString3)

    expect(vin1IsValid).toBe(true)
    expect(vin2IsValid).toBe(true)
    expect(vin3IsValid).toBe(true)

    expect(vin1!.equals(vin2!)).toBe(true)
    expect(vin1!.equals(vin3!)).toBe(true)
    expect(vin2!.equals(vin3!)).toBe(true)

    expect(vin1!.getHashCode()).toBe(vin2?.getHashCode())
    expect(vin1!.getHashCode()).toBe(vin3?.getHashCode())
    expect(vin2!.getHashCode()).toBe(vin3?.getHashCode())

    expect(String(vin1)).toBe(vinString1.toUpperCase())
    expect(String(vin2)).toBe(vinString2.toUpperCase())
    expect(String(vin3)).toBe(vinString3.toUpperCase())
  })

  test("02. Components", () => {
    const [valid, vin] = VIN.tryParse("19XFB2F57EE209415")

    expect(valid).toBe(true)
    expect(vin!.WMI).toBe("19X")
    expect(vin!.VDS).toBe("FB2F57")
    expect(vin!.CD).toBe("E")
    expect(vin!.VIS).toBe("EE209415")
  })
})

// describe("VIN Tests", () => {

//   test("03. VIN Extractor: Leading and Trailing Symbols", () => {
//     const vin1 = VINExtractor.findVINs("*1FUJGLDRXblaY2150*")
//     const vin2 = VINExtractor.findVINs("* 1fuJGLDRXBLAY2150 *")

//     expect(vin1).toHaveLength(1)
//     expect(vin2).toHaveLength(1)

//     expect(vin1[0]).toBe("1FUJGLDRXBLAY2150")
//     expect(vin2[0]).toBe("1FUJGLDRXBLAY2150")
//   })

//   test("04. VIN Extractor: Leading and Trailing Text", () => {
//     const vinSet = VINExtractor.findVINs("VIN: 1GKEK63U83J235900 (Brand Name)")
//     expect(vinSet).toHaveLength(1)
//     expect(vinSet[0]).toBe("1GKEK63U83J235900")
//   })

//   test("05. VIN Extractor: Multiple VINs in Same Line", () => {
//     const vinSet = VINExtractor.findVINs(
//       "K249 2HGES25724H591789 239 JH4DC54865S056007"
//     )
//     expect(vinSet).toHaveLength(2)
//     expect(vinSet[0]).toBe("2HGES25724H591789")
//     expect(vinSet[1]).toBe("JH4DC54865S056007")
//   })

//   test("06. VIN Extractor: Multiple VINs in Same Line Without Whitespace (False Positives)", () => {
//     const vin1 = "5N1AA08A15N732965"
//     const vin2 = "1FMCU9G99FUA05110"
//     const combined = `KKJ948${vin1}${vin2}113948LJH`
//     const result = VINExtractor.findVINs(combined)

//     expect(result).toHaveLength(4)
//     expect(result[0]).toBe("485N1AA08A15N7329")
//     expect(result[1]).toBe(vin1)
//     expect(result[2]).toBe(vin2)
//     expect(result[3]).toBe("FUA05110113948LJH")

//     const cleaned = VINExtractor.findVINs(`KKJ948 ${vin1}${vin2} 113948LJH`)
//     expect(cleaned).toHaveLength(2)
//   })

//   test("07. VIN Extractor: Whitespace inside VIN", () => {
//     const vinString = "3FAFP 3136YR25 9819"
//     const result = VINExtractor.findVINs({ RemoveWhiteSpace: true }, vinString)
//     expect(result).toHaveLength(1)
//     expect(result[0]).toBe("3FAFP3136YR259819")
//   })

//   test("08. VIN Extractor: Multiple Lines", () => {
//     const lines = [
//       "Engine Size: 4.0 L",
//       "VIN: 1FDKF37GXVEB34368 - 2025-03-26",
//       "Brand: Some Brand",
//       "Color: White",
//     ]
//     const vinSet = VINExtractor.findVINs(...lines)
//     expect(vinSet).toHaveLength(1)
//     expect(vinSet[0]).toBe("1FDKF37GXVEB34368")

//     const multiLine = `
// Engine Size: 4.0 L
// VIN: 1FDKF37GXVEB34368 - 2025-03-26
// Brand: Some Brand
// Color: White
// `
//     const vinSet2 = VINExtractor.findVINs(multiLine)
//     expect(vinSet2).toHaveLength(1)
//     expect(vinSet2[0]).toBe("1FDKF37GXVEB34368")
//   })

//   test("09. VIN Extractor: Multiple VINs in Multiple Lines", () => {
//     const content = `
//       VIN 1 & VIN2:
//       2C4RDGBG6ER101765
//       3GNDA13D48S626939

//       VIN 3:
//       1GNES16S932111256 - 2C4RDGCG2ER226597
//     `

//     const vinSet = VINExtractor.findVINs(content)
//     expect(vinSet).toHaveLength(4)
//     const vinSet2 = VINExtractor.findVINs({ RemoveWhiteSpace: true }, content)
//     expect(vinSet2).toHaveLength(4)

//     expect(vinSet[0]).toBe("2C4RDGBG6ER101765")
//     expect(vinSet[1]).toBe("3GNDA13D48S626939")
//     expect(vinSet[2]).toBe("1GNES16S932111256")
//     expect(vinSet[3]).toBe("2C4RDGCG2ER226597")
//   })

//   test("10. VIN Extractor: Character Replacement", () => {
//     const invalidVin = "IGCHK236O8F181Q70"

//     expect(VINExtractor.findVINs(invalidVin)).toHaveLength(0)

//     const result = VINExtractor.findVINs(
//       { MutateSpecialCharacters: true },
//       invalidVin
//     )

//     expect(result).toHaveLength(1)
//     expect(result[0]).toBe("1GCHK23608F181070")
//   })
// })
