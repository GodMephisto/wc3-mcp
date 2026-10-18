---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "docs/common/shim.ts"
ref: HEAD
---

if (!Float32Array.prototype.reverse) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    Float32Array.prototype.reverse = Array.prototype.reverse;
}

export {}