---
type: vendor-doc
source_repo: "https://github.com/barncastle/wow-mdx-viewer"
source_path: "src/managers/itexturemanager.ts"
ref: HEAD
---

import { Model } from '../formats/mdx/model';
import { TextureVariation } from './consts';

export interface ITextureManager {
    load: () => void;
    exists: (filename: string) => boolean;
    getTextures: (model: Model) => TextureVariation[];
}