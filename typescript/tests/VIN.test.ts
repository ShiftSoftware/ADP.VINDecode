import { describe, test, expect } from "vitest"

import VIN from "../src/VIN"

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
