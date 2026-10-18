---
type: vendor-doc
source_repo: "https://github.com/IceWolf0110/warsmash-vue"
source_path: "src/lib/utils.ts"
ref: HEAD
---

import type { ClassValue } from "clsx"
import { clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}
