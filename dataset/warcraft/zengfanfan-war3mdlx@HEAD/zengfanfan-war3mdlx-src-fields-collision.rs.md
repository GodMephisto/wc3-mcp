---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/collision.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct CollisionShape {
    pub base: Node,
    pub shape: CollisionType,
    #[dbg(formatter = "fmtx")]
    pub vertices: Vec<Vec3>,
    pub bounds_radius: f32,
}

impl CollisionShape {
    pub const ID: u32 = MdlxMagic::CLID;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };
        this.shape = CollisionType::from(cur.readx()?);
        match this.shape {
            CollisionType::Box => this.vertices = cur.read_array(2)?,
            CollisionType::Plane => this.vertices = cur.read_array(1)?,
            CollisionType::Sphere => {
                this.vertices = cur.read_array(1)?;
                this.bounds_radius = cur.readx()?;
            },
            CollisionType::Cylinder => {
                this.vertices = cur.read_array(1)?;
                this.bounds_radius = cur.readx()?;
            },
            CollisionType::Error(v) => return ERR!("Unknown collision type: {}", v),
        }
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        self.base.write_mdx(chunk)?;
        chunk.write(&self.shape.to())?;
        chunk.write(&self.vertices)?;
        match &self.shape {
            CollisionType::Sphere => chunk.write(&self.bounds_radius)?,
            CollisionType::Cylinder => chunk.write(&self.bounds_radius)?,
            _other => (),
        }
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::CollisionShape);
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "BoundsRadius" => this.bounds_radius = f.value.to()?,
                _other => this.shape = this.base.unexpect_mdl_field(f).or(CollisionType::from_mdl(f))?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Vertices" => this.vertices = f.fields.to("")?,
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }
        this.vertices.resize(yesno!(this.shape == CollisionType::Box, 2, 1), Vec3::ZERO);
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];
        lines.append(&mut self.base.write_mdl(depth)?);
        lines.push(F!("{indent}{:?},", self.shape));
        MdlWriteType2!(lines, depth, "Vertices" => self.vertices);
        lines.pushx_if_n0(&F!("{indent}BoundsRadius"), &self.bounds_radius);
        return Ok(lines);
    }
}

// region: CollisionType

#[derive(Debug, Default, PartialEq, Eq)]
pub enum CollisionType {
    #[default]
    Box,
    Plane,
    Sphere,
    Cylinder,
    Error(i32),
}

impl CollisionType {
    fn from(v: i32) -> Self {
        match v {
            0 => Self::Box,
            1 => Self::Plane,
            2 => Self::Sphere,
            3 => Self::Cylinder,
            _ => Self::Error(v),
        }
    }

    fn from_mdl(f: &MdlField) -> Result<Self, MyError> {
        match_istr!(f.name.as_str(),
            "Box" => f.expect_flag(Self::Box),
            "Plane" => f.expect_flag(Self::Plane),
            "Sphere" => f.expect_flag(Self::Sphere),
            "Cylinder" => f.expect_flag(Self::Cylinder),
            _err => f.unexpect(),
        )
    }

    fn to(&self) -> i32 {
        match self {
            Self::Box => 0,
            Self::Plane => 1,
            Self::Sphere => 2,
            Self::Cylinder => 3,
            Self::Error(x) => *x,
        }
    }
}

// endregion
