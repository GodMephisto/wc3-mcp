---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/camera.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Camera {
    pub name: String,
    #[dbg(formatter = "fmtx")]
    pub position: Vec3,
    pub field_of_view: f32,
    pub far_clip: f32,
    pub near_clip: f32,
    #[dbg(formatter = "fmtx")]
    pub target: Vec3,

    #[dbg(formatter = "fmtxx")]
    pub translation: Option<Animation<Vec3>>,
    #[dbg(formatter = "fmtxx")]
    pub rotation: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub target_translation: Option<Animation<Vec3>>,
}

impl Camera {
    pub const ID: u32 = MdlxMagic::CAMS;
    const ID_T: u32 = MdlxMagic::KCTR; /* Translation */
    const ID_R: u32 = MdlxMagic::KCRL; /* Rotation (radians) */
    const ID_TT: u32 = MdlxMagic::KTTR; /* Target translation */
    const NAME_SIZE: u32 = 80;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! {
            name: cur.read_string(Self::NAME_SIZE)?,
            position: cur.readx()?,
            field_of_view: cur.readx()?,
            far_clip: cur.readx()?,
            near_clip: cur.readx()?,
            target: cur.readx()?,
        };

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_T => this.translation = Some(Animation::read_mdx(cur)?),
                Self::ID_R => this.rotation = Some(Animation::read_mdx(cur)?),
                Self::ID_TT => this.target_translation = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        chunk.write_string(&self.name, Self::NAME_SIZE)?;
        chunk.write(&self.position)?;
        chunk.write(&self.field_of_view)?;
        chunk.write(&self.far_clip)?;
        chunk.write(&self.near_clip)?;
        chunk.write(&self.target)?;
        MdxWriteAnim!(chunk,
            Self::ID_T  => self.translation,
            Self::ID_R  => self.rotation,
            Self::ID_TT => self.target_translation,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 40 + Self::NAME_SIZE; // sz + name + pos + fov + far + near + target
        sz += self.translation.calc_mdx_size();
        sz += self.rotation.calc_mdx_size();
        sz += self.target_translation.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        let mut this = Build! { name: block.name.clone() };
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Position" => this.position = f.value.to()?,
                "FieldOfView" => this.field_of_view = f.value.to()?,
                "FarClip" => this.far_clip = f.value.to()?,
                "NearClip" => this.near_clip = f.value.to()?,
                _other => f.unexpect()?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Target" => this.read_mdl_target(f)?,
                "Translation" => this.translation = Some(Animation::read_mdl(f)?),
                "Rotation" => this.rotation = Some(Animation::read_mdl(f)?),
                _other => f.unexpect()?,
            );
        }
        return Ok(this);
    }
    pub fn read_mdl_target(&mut self, block: &MdlBlock) -> Result<(), MyError> {
        block.unexpect_frames()?;
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Position" => self.target = f.value.to()?,
                _other => f.unexpect()?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Translation" => self.target_translation = Some(Animation::read_mdl(f)?),
                _other => f.unexpect()?,
            );
        }
        return Ok(());
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];

        lines.pushx_if_n0(&F!("{indent}Position"), &self.position);
        lines.pushx_if_n0(&F!("{indent}FieldOfView"), &self.field_of_view);
        lines.pushx_if_n0(&F!("{indent}FarClip"), &self.far_clip);
        lines.pushx_if_n0(&F!("{indent}NearClip"), &self.near_clip);
        MdlWriteAnimIfSome!(lines, depth,
            "Translation" => self.translation,
            "Rotation" => self.rotation,
        );

        {
            let mut tines: Vec<String> = vec![];
            tines.pushx_if_n0(&F!("{indent2}Position"), &self.target);
            MdlWriteAnimIfSome!(tines, depth + 1, "Translation" => self.target_translation);
            if !tines.is_empty() {
                lines.push(F!("{indent}Target {{"));
                lines.append(&mut tines);
                lines.push(F!("{indent}}}"));
            }
        }

        return Ok(lines);
    }
}
