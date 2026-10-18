---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/geoset.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::extent::Extent;
use super::materialize::*;
use log::*;
use std::fmt;

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum FaceTypeGroup {
    Points,
    Lines,
    LineLoop,
    LineStrip,
    Triangles,
    TriangleStrip,
    TriangleFan,
    Quads,
    QuadStrip,
    Polygons,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub struct UnknownFaceTypeGroup(pub u32);

impl std::error::Error for UnknownFaceTypeGroup {}

impl fmt::Display for UnknownFaceTypeGroup {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Unknown face type group {}", self.0)
    }
}

impl TryFrom<u32> for FaceTypeGroup {
    type Error = UnknownFaceTypeGroup;

    fn try_from(value: u32) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(FaceTypeGroup::Points),
            1 => Ok(FaceTypeGroup::Lines),
            2 => Ok(FaceTypeGroup::LineLoop),
            3 => Ok(FaceTypeGroup::LineStrip),
            4 => Ok(FaceTypeGroup::Triangles),
            5 => Ok(FaceTypeGroup::TriangleStrip),
            6 => Ok(FaceTypeGroup::TriangleFan),
            7 => Ok(FaceTypeGroup::Quads),
            8 => Ok(FaceTypeGroup::QuadStrip),
            9 => Ok(FaceTypeGroup::Polygons),
            _ => Err(UnknownFaceTypeGroup(value)),
        }
    }
}

impl From<FaceTypeGroup> for u32 {
    fn from(value: FaceTypeGroup) -> u32 {
        match value {
            FaceTypeGroup::Points => 0,
            FaceTypeGroup::Lines => 1,
            FaceTypeGroup::LineLoop => 2,
            FaceTypeGroup::LineStrip => 3,
            FaceTypeGroup::Triangles => 4,
            FaceTypeGroup::TriangleStrip => 5,
            FaceTypeGroup::TriangleFan => 6,
            FaceTypeGroup::Quads => 7,
            FaceTypeGroup::QuadStrip => 8,
            FaceTypeGroup::Polygons => 9,
        }
    }
}

impl Materialized for FaceTypeGroup {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        let filter = flag
            .try_into()
            .map_err(|e: UnknownFaceTypeGroup| nom::Err::Failure(e.into()))?;
        Ok((input, filter))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let flag: u32 = (*self).into();
        flag.encode(output)
    }
}

// Geoset {
//     uint32 inclusiveSize
//     char[4] "VRTX"
//     uint32 vertexCount
//     float[vertexCount * 3] vertexPositions
//     char[4] "NRMS"
//     uint32 normalCount
//     float[normalCount * 3] vertexNormals
//     char[4] "PTYP"
//     uint32 faceTypeGroupsCount
//     uint32[faceTypeGroupsCount] faceTypeGroups // 0: points
//                                                // 1: lines
//                                                // 2: line loop
//                                                // 3: line strip
//                                                // 4: triangles
//                                                // 5: triangle strip
//                                                // 6: triangle fan
//                                                // 7: quads
//                                                // 8: quad strip
//                                                // 9: polygons
//     char[4] "PCNT"
//     uint32 faceGroupsCount
//     uint32[faceGroupsCount] faceGroups
//     char[4] "PVTX"
//     uint32 facesCount
//     uint16[facesCount] faces
//     char[4] "GNDX"
//     uint32 vertexGroupsCount
//     uint8[vertexGroupsCount] vertexGroups
//     char[4] "MTGC"
//     uint32 matrixGroupsCount
//     uint32[matrixGroupsCount] matrixGroups
//     char[4] "MATS"
//     uint32 matrixIndicesCount
//     uint32[matrixIndicesCount] matrixIndices
//     uint32 materialId
//     uint32 selectionGroup
//     uint32 selectionFlags
//     if (version > 800) {
//       uint32 lod
//       char[80] lodName
//     }
//     Extent extent
//     uint32 extentsCount
//     Extent[extentsCount] sequenceExtents
//     if (version > 800) {
//       (Tangents)
//       (Skin)
//     }
//     char[4] "UVAS"
//     uint32 textureCoordinateSetsCount
//     TextureCoordinateSet[textureCoordinateSetsCount] textureCoordinateSets
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Geoset {
    pub vertex_positions: Vec<[f32; 3]>,
    pub vertex_normals: Vec<[f32; 3]>,
    pub face_type_groups: Vec<FaceTypeGroup>,
    pub face_groups: Vec<u32>,
    pub faces: Vec<u16>,
    pub vertex_groups: Vec<u8>,
    pub matrix_groups: Vec<u32>,
    pub matrix_indicies: Vec<u32>,
    pub material_id: u32,
    pub selection_group: u32,
    pub selection_flags: u32,
    // if (version > 800) {
    pub lod_extra: Option<LodExtra>,
    // }
    pub extent: Extent,
    pub sequence_extents: Vec<Extent>,
    // if (version > 800) {
    pub tangents: Option<Tangents>,
    pub skin: Option<Skin>,
    pub ordered: Option<Vec<Tag>>,
    // }
    pub texture_coordinate_sets: Vec<TextureCoordinateSet>,
}

impl Geoset {
    pub fn vrtx_tag() -> Tag {
        Tag([0x56, 0x52, 0x54, 0x58]) // VRTX
    }

    pub fn nrms_tag() -> Tag {
        Tag([0x4E, 0x52, 0x4D, 0x53]) // NRMS
    }

    pub fn ptyp_tag() -> Tag {
        Tag([0x50, 0x54, 0x59, 0x50]) // PTYP
    }

    pub fn pcnt_tag() -> Tag {
        Tag([0x50, 0x43, 0x4E, 0x54]) // PCNT
    }

    pub fn pvtx_tag() -> Tag {
        Tag([0x50, 0x56, 0x54, 0x58]) // PVTX
    }

    pub fn gndx_tag() -> Tag {
        Tag([0x47, 0x4E, 0x44, 0x58]) // GNDX
    }

    pub fn mtgc_tag() -> Tag {
        Tag([0x4D, 0x54, 0x47, 0x43]) // MTGC
    }

    pub fn mats_tag() -> Tag {
        Tag([0x4D, 0x41, 0x54, 0x53]) // MATS
    }

    pub fn uvas_tag() -> Tag {
        Tag([0x55, 0x56, 0x41, 0x53]) // UVAS
    }
}

impl Materialized for Geoset {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, _) = context("VRTX", |input| Self::vrtx_tag().expect(input))(input)?;
            let (input, vertex_positions) =
                context("vertex_positions", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("NRMS", |input| Self::nrms_tag().expect(input))(input)?;
            let (input, vertex_normals) =
                context("vertex_normals", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("PTYP", |input| Self::ptyp_tag().expect(input))(input)?;
            let (input, face_type_groups) =
                context("face_type_groups", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("PCNT", |input| Self::pcnt_tag().expect(input))(input)?;
            let (input, face_groups) =
                context("face_groups", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("PVTX", |input| Self::pvtx_tag().expect(input))(input)?;
            let (input, faces) = context("faces", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("GNDX", |input| Self::gndx_tag().expect(input))(input)?;
            let (input, vertex_groups) =
                context("vertex_groups", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("MTGC", |input| Self::mtgc_tag().expect(input))(input)?;
            let (input, matrix_groups) =
                context("matrix_groups", parse_len_vec(Materialized::parse))(input)?;
            let (input, _) = context("MATS", |input| Self::mats_tag().expect(input))(input)?;
            let (input, matrix_indicies) =
                context("matrix_indicies", parse_len_vec(Materialized::parse))(input)?;
            let (input, material_id) = context("material_id", Materialized::parse)(input)?;
            let (input, selection_group) = context("selection_group", Materialized::parse)(input)?;
            let (input, selection_flags) = context("selection_flags", Materialized::parse)(input)?;
            let (input, lod_extra) =
                parse_versioned_greater(version, 800, context("lod_extra", Materialized::parse))(
                    input,
                )?;
            let (input, extent) = context("extent", Materialized::parse)(input)?;
            let (input, sequence_extents) =
                context("sequence_extents", parse_len_vec(Materialized::parse))(input)?;
            let mut tangents: Option<Tangents> = None;
            let mut skin: Option<Skin> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Tangents::tag() {
                    let (input, value) = context("tangents", Materialized::parse)(input)?;
                    tangents = Some(value);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Skin::tag() {
                    let (input, value) = context("skin", Materialized::parse)(input)?;
                    skin = Some(value);
                    ordered.push(tag);
                    Ok((input, false))
                } else {
                    trace!("Found tag {}, finish searching geoset extra chunks", tag);
                    Ok((input, true))
                }
            })(input)?;
            let (input, _) = context("UVAS", |input| Self::uvas_tag().expect(input))(input)?;
            let (input, texture_coordinate_sets) = context(
                "texture_coordinate_sets",
                parse_len_vec(Materialized::parse),
            )(input)?;
            Ok((
                input,
                Geoset {
                    vertex_positions,
                    vertex_normals,
                    face_type_groups,
                    face_groups,
                    faces,
                    vertex_groups,
                    matrix_groups,
                    matrix_indicies,
                    material_id,
                    selection_group,
                    selection_flags,
                    lod_extra,
                    extent,
                    sequence_extents,
                    tangents,
                    skin,
                    ordered: Some(ordered),
                    texture_coordinate_sets,
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            Self::vrtx_tag().encode(output)?;
            encode_len_vec(&self.vertex_positions, output)?;
            Self::nrms_tag().encode(output)?;
            encode_len_vec(&self.vertex_normals, output)?;
            Self::ptyp_tag().encode(output)?;
            encode_len_vec(&self.face_type_groups, output)?;
            Self::pcnt_tag().encode(output)?;
            encode_len_vec(&self.face_groups, output)?;
            Self::pvtx_tag().encode(output)?;
            encode_len_vec(&self.faces, output)?;
            Self::gndx_tag().encode(output)?;
            encode_len_vec(&self.vertex_groups, output)?;
            Self::mtgc_tag().encode(output)?;
            encode_len_vec(&self.matrix_groups, output)?;
            Self::mats_tag().encode(output)?;
            encode_len_vec(&self.matrix_indicies, output)?;
            self.material_id.encode(output)?;
            self.selection_group.encode(output)?;
            self.selection_flags.encode(output)?;
            if let Some(v) = &self.lod_extra {
                v.encode(output)?;
            }
            self.extent.encode(output)?;
            encode_len_vec(&self.sequence_extents, output)?;
            if let Some(ordered) = &self.ordered {
                for &tag in ordered {
                    if tag == Tangents::tag() {
                        if let Some(v) = &self.tangents {
                            v.encode(output)?;
                        }
                    } else if tag == Skin::tag() {
                        if let Some(v) = &self.skin {
                            v.encode(output)?;
                        }
                    } else {
                        warn!("Unkonwn tag {tag}, skipping it...");
                    }
                }
            } else {
                if let Some(v) = &self.tangents {
                    v.encode(output)?;
                }
                if let Some(v) = &self.skin {
                    v.encode(output)?;
                }
            }
            Self::uvas_tag().encode(output)?;
            encode_len_vec(&self.texture_coordinate_sets, output)
        })
    }
}

/// Maximum length of name of LOD for geoset
pub const GEOSET_LOD_NAME_LEN: usize = 80;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct LodExtra {
    pub lod: u32,
    pub lod_name: Literal<GEOSET_LOD_NAME_LEN>,
}

impl Materialized for LodExtra {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, lod) = context("lod", Materialized::parse)(input)?;
        let (input, lod_name) = context("lod_name", Materialized::parse)(input)?;
        Ok((input, LodExtra { lod, lod_name }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.lod.encode(output)?;
        self.lod_name.encode(output)
    }
}

// Tangents {
//     char[4] "TANG"
//     uint32 count
//     float[count * 4] tangents
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Tangents {
    pub tangents: Vec<[f32; 4]>,
}

impl Chunk for Tangents {
    fn tag() -> Tag {
        Tag([0x54, 0x41, 0x4E, 0x47]) // TANG
    }
}

impl Materialized for Tangents {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("TANG tag", Self::expect_tag)(input)?;
        let (input, tangents) = parse_len_vec(Materialized::parse)(input)?;
        Ok((input, Tangents { tangents }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        Self::encode_tag(output)?;
        encode_len_vec(&self.tangents, output)
    }
}

//   Skin {
//     char[4] "SKIN"
//     uint32 count
//     uint8[count] skin
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Skin {
    pub skin: Vec<u8>,
}

impl Chunk for Skin {
    fn tag() -> Tag {
        Tag([0x53, 0x4B, 0x49, 0x4E]) // SKIN
    }
}

impl Materialized for Skin {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("SKIN tag", Self::expect_tag)(input)?;
        let (input, skin) = parse_len_vec(Materialized::parse)(input)?;
        Ok((input, Skin { skin }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        Self::encode_tag(output)?;
        encode_len_vec(&self.skin, output)
    }
}

//   TextureCoordinateSet {
//     char[4] "UVBS"
//     uint32 count
//     float[count * 2] texutreCoordinates
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct TextureCoordinateSet {
    pub texture_coordinates: Vec<[f32; 2]>,
}

impl Chunk for TextureCoordinateSet {
    fn tag() -> Tag {
        Tag([0x55, 0x56, 0x42, 0x53]) // SKIN
    }
}

impl Materialized for TextureCoordinateSet {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("UVBS tag", Self::expect_tag)(input)?;
        let (input, texture_coordinates) = parse_len_vec(Materialized::parse)(input)?;
        Ok((
            input,
            TextureCoordinateSet {
                texture_coordinates,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        Self::encode_tag(output)?;
        encode_len_vec(&self.texture_coordinates, output)
    }
}
