---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/model.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, SmartDefault)]
pub struct Model {
    pub name: String,
    #[dbg(skip)]
    pub _unknown: i32,
    pub extent: BoundExtent,
    #[default = 100]
    pub blend_time: u32,
}

impl Model {
    pub const ID: u32 = MdlxMagic::MODL;
    const NAME_SIZE: u32 = 336;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self {
            name: cur.read_string(Self::NAME_SIZE)?,
            _unknown: cur.readx()?,
            extent: BoundExtent::read_mdx(cur)?,
            blend_time: cur.readx()?,
        })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write_string(&self.name, Self::NAME_SIZE)?;
        chunk.write(&self._unknown)?;
        self.extent.write_mdx(chunk)?;
        chunk.write(&self.blend_time)?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        block.unexpect_blocks()?;
        let mut this = Build! { name: block.name.clone(), extent: BoundExtent::read_mdl(&block, false)? };
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "BoundsRadius" | "MinimumExtent" | "MaximumExtent" => (),
                "BlendTime" => this.blend_time = f.value.to()?,
                _other => if _other.starts_with("Num") {
                    _ = f.value.to::<i32>()?
                } else {
                    f.unexpect()?
                },
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth + 1);
        let mut lines: Vec<String> = vec![];
        lines.push(F!("Model \"{}\" {{", self.name.escape()));
        lines.append(&mut self.extent.write_mdl(depth + 1)?);
        lines.push(F!("{indent}BlendTime {},", self.blend_time));
        lines.push(F!("}}"));
        return Ok(lines);
    }
}
