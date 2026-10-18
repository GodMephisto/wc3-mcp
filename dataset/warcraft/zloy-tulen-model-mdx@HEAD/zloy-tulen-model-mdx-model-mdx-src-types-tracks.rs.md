---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/tracks.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::materialize::*;
use nom::multi::count;
use std::fmt;

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum InterpolationType {
    None,
    Linear,
    Hermite,
    Bezier,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub struct UnknownInterpolationType(pub u32);

impl std::error::Error for UnknownInterpolationType {}

impl fmt::Display for UnknownInterpolationType {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Unknown interpolation type {}", self.0)
    }
}

impl TryFrom<u32> for InterpolationType {
    type Error = UnknownInterpolationType;

    fn try_from(value: u32) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(InterpolationType::None),
            1 => Ok(InterpolationType::Linear),
            2 => Ok(InterpolationType::Hermite),
            3 => Ok(InterpolationType::Bezier),
            _ => Err(UnknownInterpolationType(value)),
        }
    }
}

impl From<InterpolationType> for u32 {
    fn from(value: InterpolationType) -> u32 {
        match value {
            InterpolationType::None => 0,
            InterpolationType::Linear => 1,
            InterpolationType::Hermite => 2,
            InterpolationType::Bezier => 3,
        }
    }
}

impl Materialized for InterpolationType {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        let filter = flag
            .try_into()
            .map_err(|e: UnknownInterpolationType| nom::Err::Failure(e.into()))?;
        Ok((input, filter))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let flag: u32 = (*self).into();
        flag.encode(output)
    }
}

// TracksChunk {
//     uint32 tag
//     uint32 tracksCount
//     uint32 interpolationType // 0: none
//                              // 1: linear
//                              // 2: hermite
//                              // 3: bezier
//     uint32 globalSequenceId
//     Track[tracksCount] tracks
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct TrackChunk<T: Clone> {
    pub tag: Tag,
    pub interpolation_type: InterpolationType,
    pub global_sequence_id: u32,
    pub tracks: Vec<Track<T>>,
}

impl<T: Clone + Materialized> Materialized for TrackChunk<T> {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, tag) = context("tag", Materialized::parse)(input)?;
        let (input, tracks_count): (&[u8], u32) =
            context("tracks_count", Materialized::parse)(input)?;
        let (input, interpolation_type) =
            context("interpolation_type", Materialized::parse)(input)?;
        let (input, global_sequence_id) =
            context("global_sequence_id", Materialized::parse)(input)?;
        let (input, tracks) = context(
            "tracks",
            count(
                |input| Materialized::parse_versioned(Some(interpolation_type), input),
                tracks_count as usize,
            ),
        )(input)?;
        Ok((
            input,
            TrackChunk {
                tag,
                interpolation_type,
                global_sequence_id,
                tracks,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.tag.encode(output)?;
        (self.tracks.len() as u32).encode(output)?;
        self.interpolation_type.encode(output)?;
        self.global_sequence_id.encode(output)?;
        for t in self.tracks.iter() {
            t.encode(output)?;
        }
        Ok(())
    }
}

// Track {
//     int32 frame // Probably should be uint32, but I saw a model with negative values
//     X value
//     if (interpolationType > 1) {
//       X inTan
//       X outTan
//     }
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub enum Track<T: Clone> {
    /// Interpolation is [InterpolationType::None] or [InterpolationType::Linear]
    Linear { frame: i32, value: T },
    /// Interpolation [InterpolationType::Hermite] or [InterpolationType::Bizier]
    Complex {
        frame: i32,
        value: T,
        in_tan: T,
        out_tan: T,
    },
}

impl<T: Clone + Materialized> Materialized for Track<T> {
    type Version = InterpolationType;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, frame) = context("frame", Materialized::parse)(input)?;
        let (input, value) = context("value", Materialized::parse)(input)?;
        match version {
            Some(v) if v > InterpolationType::Linear => {
                let (input, in_tan) = context("in_tan", Materialized::parse)(input)?;
                let (input, out_tan) = context("out_tan", Materialized::parse)(input)?;
                Ok((
                    input,
                    Track::Complex {
                        frame,
                        value,
                        in_tan,
                        out_tan,
                    },
                ))
            }
            _ => Ok((input, Track::Linear { frame, value })),
        }
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        match self {
            Track::Linear { frame, value } => {
                frame.encode(output)?;
                value.encode(output)?;
            }
            Track::Complex {
                frame,
                value,
                in_tan,
                out_tan,
            } => {
                frame.encode(output)?;
                value.encode(output)?;
                in_tan.encode(output)?;
                out_tan.encode(output)?;
            }
        }
        Ok(())
    }
}
