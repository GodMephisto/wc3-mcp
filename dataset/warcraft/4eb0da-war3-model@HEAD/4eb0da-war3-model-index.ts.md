---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "index.ts"
ref: HEAD
---

export * as model from './model';
export {parse as parseMDL} from './mdl/parse';
export {parse as parseMDX} from './mdx/parse';
export {generate as generateMDL} from './mdl/generate';
export {generate as generateMDX} from './mdx/generate';
export * as blp from './blp/blpimage';
export {decode as decodeBLP, getImageData as getBLPImageData} from './blp/decode';
export {ModelRenderer} from './renderer/modelRenderer';
