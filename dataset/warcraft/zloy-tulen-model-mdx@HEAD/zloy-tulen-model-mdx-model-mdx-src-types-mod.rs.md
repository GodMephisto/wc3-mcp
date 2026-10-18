---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/mod.rs"
ref: HEAD
---

pub mod animation;
pub mod attachment;
pub mod bone;
pub mod camera;
pub mod chunk;
pub mod collision;
pub mod emitter;
pub mod event;
pub mod extent;
pub mod geoset;
pub mod layer;
pub mod light;
pub mod material;
pub mod materialize;
pub mod node;
pub mod sequence;
#[cfg(test)]
mod tests;
pub mod texture;
pub mod tracks;

use super::encoder::error::Error as EncodeError;
use super::parser::error::Error as ParseError;
pub use chunk::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct MdxModel {
    pub root: Mdlx,
}

impl MdxModel {
    pub fn from_slice(slice: &[u8]) -> Result<Self, ParseError> {
        super::parser::parse_mdx(slice)
    }

    pub fn to_vec(&self) -> Result<Vec<u8>, EncodeError> {
        super::encoder::encode_mdx(self)
    }
}
