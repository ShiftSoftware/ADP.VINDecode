import { defineConfig, RollupOptions } from "rollup"

import dts from "rollup-plugin-dts"
import commonjs from "@rollup/plugin-commonjs"
import resolve from "@rollup/plugin-node-resolve"
import typescript from "@rollup/plugin-typescript"

const entries = [
  { name: "index", input: "src/index.ts" },
  { name: "part1", input: "src/part1.ts" },
  { name: "part2", input: "src/part2.ts" },
]

const jsConfigs: RollupOptions[] = entries.map(({ name, input }) => ({
  input,
  output: [
    {
      file: `dist/${name}.mjs`,
      format: "es" as const,
      sourcemap: true,
    },
    {
      file: `dist/${name}.cjs`,
      format: "cjs" as const,
      sourcemap: true,
      exports: "named",
    },
  ],
  plugins: [resolve(), commonjs(), typescript({ tsconfig: "./tsconfig.json" })],
  external: [],
}))

const dtsConfigs: RollupOptions[] = entries.map(({ name, input }) => ({
  input,
  output: {
    file: `dist/${name}.d.ts`,
    format: "es" as const,
  },
  plugins: [dts()],
}))

export default defineConfig([...jsConfigs, ...dtsConfigs])
