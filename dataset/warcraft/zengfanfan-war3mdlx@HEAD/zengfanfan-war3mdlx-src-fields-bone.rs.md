---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/bone.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, SmartDefault)]
pub struct Bone {
    pub base: Node,
    #[default(-1)]
    pub geoset_id: i32,
    #[default(-1)]
    pub geoanim_id: i32,
}

impl Bone {
    pub const ID: u32 = MdlxMagic::BONE;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self { base: Node::read_mdx(cur)?, geoset_id: cur.readx()?, geoanim_id: cur.readx()? })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        self.base.write_mdx(chunk)?;
        chunk.write(&self.geoset_id)?;
        chunk.write(&self.geoanim_id)?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::Bone);
        this.base.unexpect_mdl_blocks()?;
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "GeosetId" => this.geoset_id = f.value.to()?,
                "GeosetAnimId" => this.geoanim_id = f.value.to()?,
                _other => this.base.unexpect_mdl_field(f)?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];
        lines.append(&mut self.base.write_mdl(depth)?);
        lines.push_if_nneg1(&F!("{indent}GeosetId"), &self.geoset_id);
        lines.push_if_nneg1(&F!("{indent}GeosetAnimId"), &self.geoanim_id);
        return Ok(lines);
    }
}
