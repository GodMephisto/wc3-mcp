---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/mdlx/index.ts"
ref: HEAD
---

/**
 * MDX/MDL 模型解析器模块
 */

export { default as Model } from './model';
export { default as Sequence } from './sequence';
export { default as Material } from './material';
export { default as Layer, FilterMode, Flags as LayerFlags } from './layer';
export { default as Texture } from './texture';
export { default as TextureAnimation } from './textureanimation';
export { default as Geoset } from './geoset';
export { default as GeosetAnimation } from './geosetanimation';
export { default as GenericObject } from './genericobject';
export { default as Bone } from './bone';
export { default as Light } from './light';
export { default as Helper } from './helper';
export { default as Attachment } from './attachment';
export { default as ParticleEmitter } from './particleemitter';
export { default as ParticleEmitter2, Flags as ParticleEmitter2Flags, HeadOrTail } from './particleemitter2';
export { default as ParticleEmitterPopcorn } from './particleemitterpopcorn';
export { default as RibbonEmitter } from './ribbonemitter';
export { default as Camera } from './camera';
export { default as EventObject } from './eventobject';
export { default as CollisionShape } from './collisionshape';
export { default as FaceEffect } from './faceeffect';
export { default as Extent } from './extent';
export { default as UnknownChunk } from './unknownchunk';
export { default as TokenStream } from './tokenstream';
export { Animation, InterpolationType } from './animations';
export { isMdx, isMdl } from './isformat';
