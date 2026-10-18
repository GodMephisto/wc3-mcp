---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/collision.rs"
ref: HEAD
---

use super::materialize::*;
use super::node::Node;
use crate::parser::error::MdxParseError;
use log::*;

// CollisionShape {
//     Node node
//     uint32 type // 0: cube
//                 // 1: plane
//                 // 2: sphere
//                 // 3: cylinder
//     float[?][3] vertices // type 0: 2
//                          // type 1: 2
//                          // type 2: 1
//                          // type 3: 2
//     if (type == 2 || type == 3) {
//       float radius
//     }
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub enum CollisionShape {
    Cube {
        node: Node,
        vertices: [[f32; 3]; 2],
    },
    Plane {
        node: Node,
        vertices: [[f32; 3]; 2],
    },
    Sphere {
        node: Node,
        vertices: [f32; 3],
        radius: f32,
    },
    Cylinder {
        node: Node,
        vertices: [[f32; 3]; 2],
        radius: f32,
    },
}

impl Materialized for CollisionShape {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, node) = context("node", Materialized::parse)(input)?;
        let (input, shape_type) = context("type", Materialized::parse)(input)?;
        match shape_type {
            0 => {
                let (input, vertices) = context("vertices cube", Materialized::parse)(input)?;
                Ok((input, CollisionShape::Cube { node, vertices }))
            }
            1 => {
                let (input, vertices) = context("vertices plane", Materialized::parse)(input)?;
                Ok((input, CollisionShape::Plane { node, vertices }))
            }
            2 => {
                let (input, vertices) = context("vertices sphere", Materialized::parse)(input)?;
                let (input, radius) = context("radius", Materialized::parse)(input)?;
                Ok((
                    input,
                    CollisionShape::Sphere {
                        node,
                        vertices,
                        radius,
                    },
                ))
            }
            3 => {
                let (input, vertices) = context("vertices cylinder", Materialized::parse)(input)?;
                let (input, radius) = context("radius", Materialized::parse)(input)?;
                Ok((
                    input,
                    CollisionShape::Cylinder {
                        node,
                        vertices,
                        radius,
                    },
                ))
            }
            _ => {
                error!("Unknown shape type {}", shape_type);
                Err(nom::Err::Failure(MdxParseError::UnknownCollisionShape {
                    tag: shape_type,
                }))
            }
        }
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        match self {
            CollisionShape::Cube{node, vertices} => {
                node.encode(output)?;
                0u32.encode(output)?;
                vertices.encode(output)
            }
            CollisionShape::Plane{node, vertices} => {
                node.encode(output)?;
                1u32.encode(output)?;
                vertices.encode(output)
            }
            CollisionShape::Sphere{node, vertices, radius} => {
                node.encode(output)?;
                2u32.encode(output)?;
                vertices.encode(output)?;
                radius.encode(output)
            }
            CollisionShape::Cylinder{node, vertices, radius} => {
                node.encode(output)?;
                3u32.encode(output)?;
                vertices.encode(output)?;
                radius.encode(output)
            }
        }
    }
}
