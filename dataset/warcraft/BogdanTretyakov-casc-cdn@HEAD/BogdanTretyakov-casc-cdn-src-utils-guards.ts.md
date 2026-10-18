---
type: vendor-doc
source_repo: "https://github.com/BogdanTretyakov/casc-cdn"
source_path: "src/utils/guards.ts"
ref: HEAD
---

export function isNotNil<T>(value: T): value is NonNullable<T> {
  return value !== null && value !== undefined;
}
