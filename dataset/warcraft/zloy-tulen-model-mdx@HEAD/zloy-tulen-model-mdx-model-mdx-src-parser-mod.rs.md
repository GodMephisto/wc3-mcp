---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/parser/mod.rs"
ref: HEAD
---

pub mod error;
pub mod primitives;

use crate::types::{
    chunk::Mdlx,
    materialize::Materialized,
    MdxModel,
};
use error::*;
use nom::IResult;

/// Binary parser for BLP format that produces [Error] when something went wrong
pub type Parser<'a, T> = IResult<&'a [u8], T, MdxParseError<&'a [u8]>>;

/// Parse MDX model from input bytes
pub fn parse_mdx(input: &[u8]) -> Result<MdxModel, Error> {
    match mdx_parser(input) {
        Ok((_, mdx)) => Ok(mdx),
        Err(nom::Err::Incomplete(needed)) => Err(Error::Incomplete(needed)),
        Err(nom::Err::Error(e)) => Err(Error::Parsing(format!("{}", e))),
        Err(nom::Err::Failure(e)) => Err(Error::Parsing(format!("{}", e))),
    }
}

fn mdx_parser(input: &[u8]) -> Parser<MdxModel> {
    let (input, mdxl_chunk) = Mdlx::parse(input)?;
    Ok((input, MdxModel { root: mdxl_chunk }))
}
