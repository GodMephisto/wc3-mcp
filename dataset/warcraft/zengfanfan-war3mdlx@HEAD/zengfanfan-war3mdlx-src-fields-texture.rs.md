---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/texture.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Texture {
    pub replace_id: i32,
    pub path: String,
    #[dbg(skip)]
    pub _unknown: i32,
    #[dbg(fmt = "{:?}")]
    pub flags: TextureFlags,
}
bitflags! {
    #[derive(Debug, Default, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
    pub struct TextureFlags : u32 {
        const WrapWidth = 1 << 0;
        const WrapHeight = 1 << 1;
    }
}

impl Texture {
    pub const ID: u32 = MdlxMagic::TEXS;
    const PATH_SIZE: u32 = 256;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self {
            replace_id: cur.readx()?,
            path: cur.read_string(Self::PATH_SIZE)?,
            _unknown: cur.readx()?,
            flags: TextureFlags::from_bits_retain(cur.readx()?),
        })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.replace_id)?;
        chunk.write_string(&self.path, Self::PATH_SIZE)?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.flags.bits())?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        block.unexpect_blocks()?;
        let mut this = Build!();
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "ReplaceableId" => this.replace_id = f.value.to()?,
                "Image" => this.path = f.value.to()?,
                "WrapWidth" => this.flags |= f.expect_flag(TextureFlags::WrapWidth)?,
                "WrapHeight" => this.flags |= f.expect_flag(TextureFlags::WrapHeight)?,
                _other => f.unexpect()?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];
        lines.push(F!("{indent}Bitmap {{"));
        lines.push_if_n0(&F!("{indent2}ReplaceableId"), &self.replace_id);
        lines.push_if(self.replace_id.is0(), F!("{indent2}Image \"{}\",", self.path.escape_path()));
        lines.push_if(self.flags.contains(TextureFlags::WrapWidth), F!("{indent2}WrapWidth,"));
        lines.push_if(self.flags.contains(TextureFlags::WrapHeight), F!("{indent2}WrapHeight,"));
        lines.push(F!("{indent}}}"));
        return Ok(lines);
    }
}

#[derive(Dbg, Default)]
pub struct TextureAnim {
    #[dbg(formatter = "fmtxx")]
    pub translation: Option<Animation<Vec3>>,
    #[dbg(formatter = "fmtxx")]
    pub rotation: Option<Animation<Vec4>>,
    #[dbg(formatter = "fmtxx")]
    pub scaling: Option<Animation<Vec3>>,
}

impl TextureAnim {
    pub const ID: u32 = MdlxMagic::TXAN;
    const ID_T: u32 = MdlxMagic::KTAT; /* Translation */
    const ID_R: u32 = MdlxMagic::KTAR; /* Rotation */
    const ID_S: u32 = MdlxMagic::KTAS; /* Scaling */

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build!();
        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_T => this.translation = Some(Animation::read_mdx(cur)?),
                Self::ID_R => this.rotation = Some(Animation::read_mdx(cur)?),
                Self::ID_S => this.scaling = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        MdxWriteAnim!(chunk,
            Self::ID_T => self.translation,
            Self::ID_R => self.rotation,
            Self::ID_S => self.scaling,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 4;
        sz += self.translation.calc_mdx_size();
        sz += self.rotation.calc_mdx_size();
        sz += self.scaling.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_fields()?;
        block.unexpect_frames()?;
        let mut this = Build!();
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Translation" => this.translation = Some(Animation::read_mdl(f)?),
                "Rotation" => this.rotation = Some(Animation::read_mdl(f)?),
                "Scaling" => this.scaling = Some(Animation::read_mdl(f)?),
                _other => f.unexpect()?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];
        lines.push(F!("{indent}TVertexAnim {{"));
        MdlWriteAnimIfSome!(lines, 2,
            "Translation" => self.translation,
            "Rotation" => self.rotation,
            "Scaling" => self.scaling,
        );
        lines.push(F!("{indent}}}"));
        return Ok(lines);
    }
}
