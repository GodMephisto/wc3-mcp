---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/materialize.rs"
ref: HEAD
---

pub use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::{le_f32, times};
pub use crate::parser::{error::MdxParseError as ParseError, Parser};
pub use nom::error::context;
use nom::{
    multi::count,
    number::complete::{le_i32, le_u16, le_u32, le_u8},
};

/// Types that can be parsed and encoded to bytes
pub trait Materialized: Sized {
    /// Extra info that is passed to parser to decode
    /// from bytes correctly.
    type Version: PartialEq + PartialOrd;

    /// Parse type from the given input with default version
    fn parse(input: &[u8]) -> Parser<Self> {
        Self::parse_versioned(None, input)
    }

    /// Parse the type from given input
    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self>;

    /// Encode the type to byte stream
    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError>;
}

/// Helper to parse extra fields if we have non default version
pub fn parse_versioned_opt<'a, F, T: Materialized>(
    version: Option<T::Version>,
    input: &'a [u8],
    body: F,
) -> Parser<'a, Option<T>>
where
    F: FnOnce(T::Version) -> Parser<'a, Option<T>>,
{
    if let Some(v) = version {
        body(v)
    } else {
        Ok((input, None))
    }
}

/// Helper to parse extra fields if we have version greater than
/// given value.
pub fn parse_versioned_greater<'a, T: Materialized, F>(
    version: Option<T::Version>,
    val: T::Version,
    body: F,
) -> impl FnOnce(&'a [u8]) -> Parser<'a, Option<T>>
where
    F: FnOnce(&'a [u8]) -> Parser<'a, T>,
{
    move |input| {
        if let Some(v) = version {
            if v > val {
                let (input, value) = body(input)?;
                Ok((input, Some(value)))
            } else {
                Ok((input, None))
            }
        } else {
            Ok((input, None))
        }
    }
}

/// Parse vector with known length in advance
pub fn parse_fixed_vec<T: Materialized>(n: usize) -> impl FnOnce(&[u8]) -> Parser<Vec<T>> {
    move |input| {
        let (input, vec) = context("vector elements", count(Materialized::parse, n))(input)?;
        Ok((input, vec))
    }
}

/// Encode vector with known length in advance
pub fn encode_fixed_vec<'a, T: Materialized>(
    elements: &'a [T],
) -> impl FnMut(&'a mut Vec<u8>) -> Result<(), EncodeError> + Copy {
    move |output| {
        for v in elements {
            v.encode(output)?;
        }
        Ok(())
    }
}

/// Parse `uint32` before collection and apply parser N times, collect result.
pub fn parse_len_vec<T: Materialized, F>(parser: F) -> impl FnMut(&[u8]) -> Parser<Vec<T>>
where
    F: FnMut(&[u8]) -> Parser<T> + Copy,
{
    move |input| {
        let (input, n): (&[u8], u32) = context("vector length", Materialized::parse)(input)?;
        let (input, vec) = context("vector elements", count(parser, n as usize))(input)?;
        Ok((input, vec))
    }
}

/// Encodes count of elements first with `uint32` and next all elements.
pub fn encode_len_vec<T: Materialized>(vec: &[T], output: &mut Vec<u8>) -> Result<(), EncodeError> {
    (vec.len() as u32).encode(output)?;
    for v in vec.iter() {
        v.encode(output)?;
    }
    Ok(())
}

/// Helper that fetches `uint32` and executes subparser only on the region of input
/// that is covered by the inclusive size and checks that subparser consume all
/// input.
pub fn parse_inclusive_sized<F, T>(body: F) -> impl FnOnce(&[u8]) -> Parser<T>
where
    F: FnOnce(&[u8]) -> Parser<T>,
{
    move |input| {
        let (input, inclusive_size): (&[u8], u32) =
            context("inclusive_size", Materialized::parse)(input)?;
        if inclusive_size < 4 {
            return Err(nom::Err::Failure(ParseError::InclusiveSizeTooSmall {
                size: inclusive_size,
            }));
        } else if ((inclusive_size - 4) as usize) > input.len() {
            return Err(nom::Err::Failure(ParseError::InclusiveSizeNotEhoughInput {
                size: inclusive_size,
                input: input.len(),
            }));
        }

        let sub_input = &input[0..(inclusive_size - 4) as usize];
        let (inclusive_rest, value) = body(sub_input)?;
        if !inclusive_rest.is_empty() {
            return Err(nom::Err::Failure(ParseError::InclusiveLeftover {
                input: inclusive_rest.len(),
            }));
        }
        Ok((&input[(inclusive_size - 4) as usize..], value))
    }
}

/// Records size of all enclosed encoders and writes before them inlcusive size of `uint32`
pub fn encode_inclusive_sized<F>(output: &mut Vec<u8>, body: F) -> Result<(), EncodeError>
where
    F: FnOnce(&mut Vec<u8>) -> Result<(), EncodeError>,
{
    let mut buff = vec![];
    body(&mut buff)?;
    let inclusive_size = (buff.len() + 4) as u32;
    inclusive_size.encode(output)?;
    output.extend(buff);
    Ok(())
}

/// Records size of all enclosed encoders and writes before them exclusive size of `uint32`
pub fn encode_exclusive_sized<F>(output: &mut Vec<u8>, body: F) -> Result<(), EncodeError>
where
    F: FnOnce(&mut Vec<u8>) -> Result<(), EncodeError>,
{
    let mut buff = vec![];
    body(&mut buff)?;
    let exclusive_size = buff.len() as u32;
    exclusive_size.encode(output)?;
    output.extend(buff);
    Ok(())
}

impl Materialized for u8 {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        le_u8(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.push(*self);
        Ok(())
    }
}

impl Materialized for f32 {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        le_f32(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(self.to_le_bytes());
        Ok(())
    }
}

impl Materialized for u16 {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        le_u16(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(self.to_le_bytes());
        Ok(())
    }
}

impl Materialized for u32 {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        le_u32(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(self.to_le_bytes());
        Ok(())
    }
}

impl Materialized for [u32; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, u32, _>(le_u32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [u32; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, u32, _>(le_u32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [u32; 4] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<4, u32, _>(le_u32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for i32 {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        le_i32(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(self.to_le_bytes());
        Ok(())
    }
}

impl Materialized for [i32; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, i32, _>(le_i32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [i32; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, i32, _>(le_i32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [i32; 4] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<4, i32, _>(le_i32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [f32; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, f32, _>(le_f32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [f32; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, f32, _>(le_f32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [f32; 4] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<4, f32, _>(le_f32)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}

impl Materialized for [u8; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, u8, _>(le_u8)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.push(*v);
        }
        Ok(())
    }
}

impl Materialized for [u8; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, u8, _>(le_u8)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.push(*v);
        }
        Ok(())
    }
}

impl Materialized for [u8; 4] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<4, u8, _>(le_u8)(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for v in self {
            output.push(*v);
        }
        Ok(())
    }
}

impl Materialized for [[f32; 3]; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, [f32; 3], _>(|input| times::<3, f32, _>(le_f32)(input))(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for a in self {
            for v in a {
                output.extend(v.to_le_bytes());
            }
        }
        Ok(())
    }
}

impl Materialized for [[f32; 3]; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, [f32; 3], _>(|input| times::<3, f32, _>(le_f32)(input))(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for a in self {
            for v in a {
                output.extend(v.to_le_bytes());
            }
        }
        Ok(())
    }
}

impl Materialized for [[u32; 3]; 2] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<2, [u32; 3], _>(|input| times::<3, u32, _>(le_u32)(input))(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for a in self {
            for v in a {
                output.extend(v.to_le_bytes());
            }
        }
        Ok(())
    }
}

impl Materialized for [[u32; 3]; 3] {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        times::<3, [u32; 3], _>(|input| times::<3, u32, _>(le_u32)(input))(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        for a in self {
            for v in a {
                output.extend(v.to_le_bytes());
            }
        }
        Ok(())
    }
}
