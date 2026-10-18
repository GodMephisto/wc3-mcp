---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/sequence.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, SmartDefault)]
pub struct Sequence {
    pub name: String,
    pub start_frame: i32,
    pub end_frame: i32,
    pub move_speed: f32,
    #[default = true]
    pub looping: bool,
    pub rarity: f32,
    #[dbg(skip)]
    pub _unknown: i32,
    pub bounds_radius: f32,
    #[dbg(formatter = "fmtx")]
    pub min_extent: Vec3,
    #[dbg(formatter = "fmtx")]
    pub max_extent: Vec3,
}

impl Sequence {
    pub const ID: u32 = MdlxMagic::SEQS;
    const NAME_SIZE: u32 = 80;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self {
            name: cur.read_string(Self::NAME_SIZE)?,
            start_frame: cur.readx()?,
            end_frame: cur.readx()?,
            move_speed: cur.readx()?,
            looping: cur.readx::<i32>()? == 0,
            rarity: cur.readx()?,
            _unknown: cur.readx()?,
            bounds_radius: cur.readx()?,
            min_extent: cur.readx()?,
            max_extent: cur.readx()?,
        })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write_string(&self.name, Self::NAME_SIZE)?;
        chunk.write(&self.start_frame)?;
        chunk.write(&self.end_frame)?;
        chunk.write(&self.move_speed)?;
        chunk.write(yesno!(self.looping, &0u32, &1u32))?;
        chunk.write(&self.rarity)?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.bounds_radius)?;
        chunk.write(&self.min_extent)?;
        chunk.write(&self.max_extent)?;
        return Ok(());
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        block.unexpect_blocks()?;
        let mut this = Build! { name: block.name.clone() };
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "MoveSpeed" => this.move_speed = f.value.to()?,
                "NonLooping" => this.looping = f.expect_flag(false)?,
                "Rarity" => this.rarity = f.value.to()?,
                "BoundsRadius" => this.bounds_radius = f.value.to()?,
                "MinimumExtent" => this.min_extent = f.value.to()?,
                "MaximumExtent" => this.max_extent = f.value.to()?,
                "Interval" => {
                    let interval: Vec<i32> = f.value.to()?;
                    no!(interval.len()==2, return f.value.expect_but("2 integers", interval.len().s().as_str()));
                    this.start_frame = interval.get(0).cloned().unwrap_or(0);
                    this.end_frame = interval.get(1).cloned().unwrap_or(0);
                },
                _other => f.unexpect()?,
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];
        lines.push(F!("{indent}Anim \"{}\" {{", self.name.escape()));
        lines.push(F!("{indent2}Interval {{ {}, {} }},", self.start_frame, self.end_frame));
        lines.pushx_if_n0(&F!("{indent2}MoveSpeed"), &self.move_speed);
        no!(self.looping, lines.push(F!("{indent2}NonLooping,")));
        lines.pushx_if_n0(&F!("{indent2}Rarity"), &self.rarity);
        if !(self.bounds_radius.is0() && self.min_extent.is0() && self.max_extent.is0()) {
            lines.pushx(&F!("{indent2}BoundsRadius"), &self.bounds_radius);
            lines.pushx(&F!("{indent2}MinimumExtent"), &self.min_extent);
            lines.pushx(&F!("{indent2}MaximumExtent"), &self.max_extent);
        }
        lines.push(F!("{indent}}}"));
        return Ok(lines);
    }
}
