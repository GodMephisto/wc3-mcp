---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/index.ts"
ref: HEAD
---

export * from "./api";
export * from "./reporter";
export * from "./tasks";
export * from "./types";
export * from "./utils";

export { parallelBatchTranslateNeedToBeTranslateObject as default } from "./tasks";
