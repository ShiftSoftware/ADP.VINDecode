import VIN from "./VIN"
export interface VINExtractionOptions {
  /**
   * Removes all whitespace from the input text.
   */
  RemoveWhiteSpace?: boolean

  /**
   * Replaces un allowed VIN characters with their allowed counterparts.
   * I -> 1, O -> 0, Q -> 0
   */
  MutateSpecialCharacters?: boolean
}

export function normalizeInput(
  text: string,
  options?: VINExtractionOptions
): string {
  if (!options) return text

  let result = text

  if (options.RemoveWhiteSpace) result = result.replace(/\s+/g, "")
  if (options.MutateSpecialCharacters)
    result = result.replace(/[IOQ]/g, (c) => (c === "I" ? "1" : "0"))

  return result
}

type FindVins =
  | string[]
  | [first: string, ...rest: [...string[], string | VINExtractionOptions]]

export function findVINs(...texts: FindVins): VIN[] {
  const vins: VIN[] = []
  const seen = new Set<string>()

  const last = texts[texts.length - 1]
  const hasOptions =
    typeof last === "object" && last !== null && !Array.isArray(last)
  const options = hasOptions ? (texts.pop() as VINExtractionOptions) : undefined

  for (let i = 0; i < texts.length; i++) {
    const text = (texts as string[])[i]
    if (!text) continue

    const normalized = normalizeInput(text, options)
    const limit = normalized.length - 17

    for (let j = 0; j <= limit; j++) {
      const slice = normalized.substring(j, j + 17)
      const [valid, vin] = VIN.tryParse(slice)
      if (valid && vin) {
        const key = vin.toString()
        if (!seen.has(key)) {
          seen.add(key)
          vins.push(vin)
        }
      }
    }
  }

  return vins
}

const VINExtractor = { normalizeInput, findVINs }

export default VINExtractor
