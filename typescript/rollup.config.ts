import { defineConfig, RollupOptions } from "rollup"

import dts from "rollup-plugin-dts"
import commonjs from "@rollup/plugin-commonjs"
import resolve from "@rollup/plugin-node-resolve"
import typescript from "@rollup/plugin-typescript"

const entries = [
  { name: "VIN", input: "src/VIN.ts" },
  { name: "index", input: "src/index.ts" },
  { name: "Validator", input: "src/Validator.ts" },
  { name: "VINExtractor", input: "src/VINExtractor.ts" },
]
const jsConfigs: RollupOptions[] = entries.map(({ name, input }) => ({
  input,
  output: [
    {
      file: `dist/${name}.mjs`,
      format: "es",
      sourcemap: true,
    },
    {
      file: `dist/${name}.cjs`,
      format: "cjs",
      sourcemap: true,
      exports: "named",
    },
  ],
  plugins: [resolve(), commonjs(), typescript({ tsconfig: "./tsconfig.json" })],
  external: [], // Add dependencies here if you ever have any
}))

const dtsConfigs: RollupOptions[] = entries.map(({ name, input }) => ({
  input,
  output: {
    file: `dist/${name}.d.ts`,
    format: "es",
  },
  plugins: [dts()],
}))

export default defineConfig([...jsConfigs, ...dtsConfigs])
