---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/helper.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Helper {
    pub base: Node,
}

impl Helper {
    pub const ID: u32 = MdlxMagic::HELP;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self { base: Node::read_mdx(cur)? })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        self.base.write_mdx(chunk)
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut base: Node = Node::read_mdl(block)?;
        base.flags.insert(NodeFlags::Helper);
        base.unexpect_mdl_fields()?;
        base.unexpect_mdl_blocks()?;
        return Ok(Self { base });
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        self.base.write_mdl(depth)
    }
}
