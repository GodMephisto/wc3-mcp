---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/eventobj.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct EventObject {
    pub base: Node,
    pub track: EventTrack,
}

impl EventObject {
    pub const ID: u32 = MdlxMagic::EVTS;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };
        if cur.left() >= 8 {
            match cur.read_be()? {
                EventTrack::ID => this.track = EventTrack::read_mdx(cur)?,
                id => return ERR!("Unknown chunk in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        self.base.write_mdx(chunk)?;
        self.track.write_mdx(chunk)?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::EventObject);
        this.base.unexpect_mdl_fields()?;
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "EventTrack" => this.track = EventTrack::read_mdl(f)?,
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let mut lines: Vec<String> = vec![];
        lines.append(&mut self.base.write_mdl(depth)?);
        lines.append(&mut self.track.write_mdl(depth)?);
        return Ok(lines);
    }
}

#[derive(Dbg, SmartDefault)]
pub struct EventTrack {
    #[dbg(skip)]
    #[default(-1)]
    pub _unknown: i32,
    pub frames: Vec<i32>,
}

impl EventTrack {
    pub const ID: u32 = MdlxMagic::KEVT as u32;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build!();
        let n = cur.readx::<u32>()?;
        this._unknown = cur.readx()?;
        this.frames = cur.read_array(n)?;
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write_be(&Self::ID)?;
        chunk.write(&self.frames.len())?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.frames)?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_blocks()?;
        block.unexpect_frames()?;
        Ok(Build! { frames: block.to_array("")? })
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];
        if !self.frames.is_empty() {
            lines.push(F!("{indent}EventTrack {} {{", self.frames.len()));
            for f in &self.frames {
                lines.push(F!("{indent2}{},", f));
            }
            lines.push(F!("{indent}}}"));
        }
        return Ok(lines);
    }
}
