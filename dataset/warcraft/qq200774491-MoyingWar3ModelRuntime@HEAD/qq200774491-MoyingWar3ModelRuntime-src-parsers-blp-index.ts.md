---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/blp/index.ts"
ref: HEAD
---

/**
 * BLP 图像解析器模块
 */

export { BLPType, BLPContent } from './blpimage';
export type { BLPImage, BLPMipMap } from './blpimage';
export { decode, getImageData } from './decode';
export { default as decodeJPEG } from './decoder';
