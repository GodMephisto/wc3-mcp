---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/utils.rs"
ref: HEAD
---

use crate::encoder::error::Error as EncodeError;
use crate::encoder::primitives::push_le_u32;
use crate::parser::{error::MdxParseError as ParseError, Parser};
use crate::types::materialize::Materialized;
use nom::{bytes::complete::take, error::context, number::complete::le_u32};
use std::fmt;

/// Fixed 4-byte tag, usually ASCII  
#[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord)]
pub struct Tag(pub [u8; 4]);

impl fmt::Display for Tag {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        let str = std::str::from_utf8(&self.0)
            .map(|s| s.to_owned())
            .unwrap_or_else(|_| format!("{:?}", &self.0));
        write!(f, "{}", str)
    }
}

impl Materialized for Tag {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Tag> {
        let mut tag_res: [u8; 4] = Default::default();
        let (input, tag) = take(4usize)(input)?;
        tag_res.copy_from_slice(&tag);
        Ok((input, Tag(tag_res)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(&self.0);
        Ok(())
    }
}

impl Tag {
    /// Parse tag and check that it equals expected one
    pub fn expect<'a>(&self, input: &'a [u8]) -> Parser<'a, Tag> {
        let (input, tag) = Tag::parse(input)?;
        if tag != *self {
            let expected: String = format!("{}", self);
            let found: String = format!("{}", tag);
            Err(nom::Err::Failure(ParseError::UnexpectedChunkTag(
                expected, found,
            )))
        } else {
            Ok((input, tag))
        }
    }
}

/// Chunk header with tag and size
pub struct Header {
    pub tag: Tag,
    pub size: usize,
}

impl Materialized for Header {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Header> {
        let (input, tag) = context("tag", Tag::parse)(input)?;
        let (input, size) = context("size", le_u32)(input)?;
        Ok((
            input,
            Header {
                tag,
                size: size as usize,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.tag.encode(output)?;
        if self.size > u32::MAX as usize {
            return Err(EncodeError::SizeUintOverflow(self.size));
        }
        push_le_u32(self.size as u32, output);
        Ok(())
    }
}

impl Header {
    /// Get amount of bytes the header has when serialized
    pub fn size() -> usize {
        8
    }

    /// Expect header with given tag
    pub fn expect(tag: Tag, input: &[u8]) -> Parser<Self> {
        let (input, header) = Header::parse(input)?;
        if tag != header.tag {
            let expected: String = format!("{}", tag);
            let found: String = format!("{}", header.tag);

            Err(nom::Err::Failure(ParseError::UnexpectedChunkTag(
                expected, found,
            )))
        } else {
            Ok((input, header))
        }
    }
}

/// Literal string that has fixed size in bytes
#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Literal<const S: usize> {
    pub content: String,
}

impl<const S: usize> Materialized for Literal<S> {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, literal_bytes) = context("literal", take(S))(input)?;
        if literal_bytes.len() != S {
            Err(nom::Err::Failure(ParseError::TooShortLiteral {
                expected: S,
                found: literal_bytes.len() as usize,
            }))
        } else {
            let content = std::str::from_utf8(&literal_bytes)
                .map_err(|e| nom::Err::Failure(ParseError::Utf8Conv(e)))?
                .to_owned();
            Ok((input, Literal { content }))
        }
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let content_bytes = self.content.as_bytes();
        let n = content_bytes.len();
        if n > S {
            Err(EncodeError::LiteralSizeOverflow {
                expected: S,
                passed: n,
            })
        } else {
            output.extend(content_bytes);
            if n < S {
                let padding = S - n;
                output.extend(vec![0; padding]);
            }
            Ok(())
        }
    }
}
