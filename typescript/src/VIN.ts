import Validator from "./Validator"

export default class VIN {
  readonly vin: string
  public readonly CD: string
  public readonly WMI: string
  public readonly VDS: string
  public readonly VIS: string

  private constructor(vin: string) {
    this.vin = vin.toUpperCase()
    this.WMI = this.vin.substring(0, 3)
    this.VDS = this.vin.substring(3, 9)
    this.CD = this.vin.substring(9, 10)
    this.VIS = this.vin.substring(9, 17)
  }

  /**
   * Tries to parse a VIN string and returns an instance if valid.
   * @param vin VIN string
   * @returns A tuple [success, VIN instance or null]
   */
  public static tryParse(vin: string): [boolean, VIN | null] {
    vin = vin.toUpperCase()

    if (!Validator.validateCheckDigit(vin)) return [false, null]

    return [true, new VIN(vin)]
  }

  /**
   * Parses a VIN or throws an error if it's invalid.
   */
  public static parse(vin: string): VIN {
    vin = vin.toUpperCase()

    if (!Validator.validateCheckDigit(vin)) throw new Error("Invalid VIN")

    return new VIN(vin)
  }

  toString(): string {
    return this.vin
  }

  public equals(other: VIN): boolean {
    return this.vin === other.vin.toUpperCase()
  }

  public getHashCode(): number {
    let hash = 0
    for (let i = 0; i < this.vin.length; i++) {
      hash = (hash * 31 + this.vin.charCodeAt(i)) >>> 0
    }
    return hash
  }

  // automatic string coercion
  [Symbol.toPrimitive](hint: string): string | null {
    if (hint === "string" || hint === "default") return this.vin

    return null
  }
}
