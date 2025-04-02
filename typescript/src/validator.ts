/**
 * Runs the check digit validation on the provided (UPPERCASE) VIN
 * @param vin VIN to run the check digit validation. Should be in Uppercase
 * @returns true if the VIN passes the check digit check. Otherwise, false.
 */

function validateCheckDigit(vin: string): boolean {
  if (!vin || vin.length !== 17) return false

  let sum = 0

  for (let i = 0; i < vin.length; i++) {
    const value = transliterationTable[vin[i]]

    if (value === undefined) return false

    sum += value * weightTable[i]
  }

  const remainder = sum % 11
  const expectedCheckDigit = remainder === 10 ? "X" : remainder.toString()

  return vin[8] === expectedCheckDigit
}

const weightTable: number[] = [
  8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2,
] as const

const transliterationTable: Record<string, number> = {
  "0": 0,
  "1": 1,
  "2": 2,
  "3": 3,
  "4": 4,
  "5": 5,
  "6": 6,
  "7": 7,
  "8": 8,
  "9": 9,

  A: 1,
  B: 2,
  C: 3,
  D: 4,
  E: 5,
  F: 6,
  G: 7,
  H: 8,

  J: 1,
  K: 2,
  L: 3,
  M: 4,
  N: 5,

  P: 7,

  R: 9,

  S: 2,
  T: 3,
  U: 4,
  V: 5,
  W: 6,
  X: 7,
  Y: 8,
  Z: 9,
} as const

const Validator = {
  validateCheckDigit,
}

export default Validator
