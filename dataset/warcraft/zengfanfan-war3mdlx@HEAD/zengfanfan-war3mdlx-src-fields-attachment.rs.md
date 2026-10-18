---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/attachment.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Attachment {
    pub base: Node,

    pub child_path: String,
    #[dbg(skip)]
    pub _unknown: i32,
    #[dbg(fmt = "{:?}")]
    pub attachment_id: Option<i32>,
    pub appear_order: i32, // the order appears in the file

    #[dbg(formatter = "fmtxx")]
    pub visibility: Option<Animation<f32>>,
}

impl Attachment {
    pub const ID: u32 = MdlxMagic::ATCH;
    const ID_V: u32 = MdlxMagic::KATV; /* Visibility */
    const PATH_SIZE: u32 = 256;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! {
            base: Node::read_mdx(cur)?,
            child_path: cur.read_string(Self::PATH_SIZE)?,
            _unknown: cur.readx()?,
            attachment_id: Some(cur.readx()?),
        };
        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_V => this.visibility = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        self.base.write_mdx(chunk)?;
        chunk.write_string(&self.child_path, Self::PATH_SIZE)?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.attachment_id.unwrap_or(self.appear_order))?;
        MdxWriteAnim!(chunk, Self::ID_V => self.visibility);
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 12 + Self::PATH_SIZE; // sz + path + unknown + attachment_id
        sz += self.base.calc_mdx_size();
        sz += self.visibility.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::Attachment);
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Path" => this.child_path = f.value.to()?,
                "AttachmentID" => this.attachment_id = Some(f.value.to()?),
                _other => this.base.unexpect_mdl_field(f)?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Visibility" => this.visibility = Some(Animation::read_mdl(f)?),
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];
        lines.append(&mut self.base.write_mdl(depth)?);
        if let Some(aid) = self.attachment_id {
            lines.push_if(aid != self.appear_order, F!("{indent}AttachmentID {},", aid));
        }
        lines.pushx_if_n0(&F!("{indent}Path"), &self.child_path.escape_path());
        MdlWriteAnimIfSome!(lines, depth, "Visibility" => self.visibility);
        return Ok(lines);
    }
}
