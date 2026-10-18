---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/particle_emitter2.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, SmartDefault)]
pub struct ParticleEmitter2 {
    pub base: Node,

    pub speed: f32,
    pub variation: f32,
    pub latitude: f32,
    pub gravity: f32,
    pub lifespan: f32,
    pub emit_rate: f32,
    pub length: f32,
    pub width: f32,

    pub filter_mode: PE2FilterMode,
    pub rows: i32,
    pub columns: i32,
    pub head_or_tail: HeadOrTail,

    pub tail_length: f32,
    pub time: f32,

    #[dbg(formatter = "fmtx")]
    #[default(vec![Vec3::ONE; 3])]
    pub segment_color: Vec<Vec3>, // RGB
    #[default(vec![255u8; 3])]
    #[dbg(formatter = "fmtx")]
    pub segment_alpha: Vec<u8>, // 0~255
    #[default(vec![1f32; 3])]
    #[dbg(formatter = "fmtx")]
    pub segment_scaling: Vec<f32>,

    pub head_life: PE2UVAnim,
    pub head_decay: PE2UVAnim,
    pub tail_life: PE2UVAnim,
    pub tail_decay: PE2UVAnim,

    pub texture_id: i32,
    pub squirt: bool,
    pub priority_plane: i32,
    pub replace_id: i32,

    #[dbg(formatter = "fmtxx")]
    pub speed_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub variation_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub latitude_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub gravity_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub emit_rate_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub length_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub width_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub visibility: Option<Animation<f32>>,
}

impl ParticleEmitter2 {
    pub const ID: u32 = MdlxMagic::PRE2;
    const ID_SPD: u32 = MdlxMagic::KP2S; /* Speed */
    const ID_VA: u32 = MdlxMagic::KP2R; /* Variation */
    const ID_LA: u32 = MdlxMagic::KP2L; /* Latitude */
    const ID_G: u32 = MdlxMagic::KP2G; /* Gravity */
    const ID_ER: u32 = MdlxMagic::KP2E; /* Emission Rate */
    const ID_V: u32 = MdlxMagic::KP2V; /* Visibility */
    const ID_L: u32 = MdlxMagic::KP2N; /* Length */
    const ID_W: u32 = MdlxMagic::KP2W; /* Width */

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };

        this.speed = cur.readx()?;
        this.variation = cur.readx()?;
        this.latitude = cur.readx()?;
        this.gravity = cur.readx()?;
        this.lifespan = cur.readx()?;
        this.emit_rate = cur.readx()?;
        this.length = cur.readx()?;
        this.width = cur.readx()?;

        this.filter_mode = PE2FilterMode::from(cur.readx()?);
        if let PE2FilterMode::Error(v) = this.filter_mode {
            return ERR!("Unknown filter mode: {}", v);
        }
        this.rows = cur.readx()?;
        this.columns = cur.readx()?;
        this.head_or_tail = HeadOrTail::from(cur.readx()?);
        if let HeadOrTail::Error(v) = this.head_or_tail {
            return ERR!("Unknown filter mode: {}", v);
        }

        this.tail_length = cur.readx()?;
        this.time = cur.readx()?;

        this.segment_color = cur.read_array(3)?;
        this.segment_alpha = cur.read_array(3)?;
        this.segment_scaling = cur.read_array(3)?;

        this.head_life = PE2UVAnim { start: cur.readx()?, end: cur.readx()?, repeat: cur.readx()? };
        this.head_decay = PE2UVAnim { start: cur.readx()?, end: cur.readx()?, repeat: cur.readx()? };
        this.tail_life = PE2UVAnim { start: cur.readx()?, end: cur.readx()?, repeat: cur.readx()? };
        this.tail_decay = PE2UVAnim { start: cur.readx()?, end: cur.readx()?, repeat: cur.readx()? };

        this.texture_id = cur.readx()?;
        this.squirt = 0i32 != cur.readx()?;
        this.priority_plane = cur.readx()?;
        this.replace_id = cur.readx()?;

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_SPD => this.speed_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_VA => this.variation_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_LA => this.latitude_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_G => this.gravity_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_ER => this.emit_rate_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_L => this.length_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_W => this.width_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_V => this.visibility = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        self.base.write_mdx(chunk)?;

        chunk.write(&self.speed)?;
        chunk.write(&self.variation)?;
        chunk.write(&self.latitude)?;
        chunk.write(&self.gravity)?;
        chunk.write(&self.lifespan)?;
        chunk.write(&self.emit_rate)?;
        chunk.write(&self.length)?;
        chunk.write(&self.width)?;

        chunk.write(&self.filter_mode.to())?;
        chunk.write(&self.rows)?;
        chunk.write(&self.columns)?;
        chunk.write(&self.head_or_tail.to())?;

        chunk.write(&self.tail_length)?;
        chunk.write(&self.time)?;

        chunk.write(&self.segment_color)?;
        chunk.write(&self.segment_alpha)?;
        chunk.write(&self.segment_scaling)?;

        self.head_life.write_mdx(chunk)?;
        self.head_decay.write_mdx(chunk)?;
        self.tail_life.write_mdx(chunk)?;
        self.tail_decay.write_mdx(chunk)?;

        chunk.write(&self.texture_id)?;
        chunk.write(&yesno!(self.squirt, 1, 0))?;
        chunk.write(&self.priority_plane)?;
        chunk.write(&self.replace_id)?;

        MdxWriteAnim!(chunk,
            Self::ID_SPD=> self.speed_anim,
            Self::ID_VA => self.variation_anim,
            Self::ID_LA => self.latitude_anim,
            Self::ID_G  => self.gravity_anim,
            Self::ID_ER => self.emit_rate_anim,
            Self::ID_V  => self.visibility,
            Self::ID_L  => self.length_anim,
            Self::ID_W  => self.width_anim,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 36; // sz + spd + va + la + g + lifespan + er + len + width
        sz += 24; // filter_mode + row + cols + head_or_tail + tail_len + time
        sz += 51; // seg_color(36) + seg_alpha(3) + seg_scale(12)
        sz += PE2UVAnim::size() * 4;
        sz += 16; // texture_id + squirt + priority_plane + replace_id
        sz += self.base.calc_mdx_size();
        sz += self.emit_rate_anim.calc_mdx_size();
        sz += self.width_anim.calc_mdx_size();
        sz += self.length_anim.calc_mdx_size();
        sz += self.speed_anim.calc_mdx_size();
        sz += self.latitude_anim.calc_mdx_size();
        sz += self.variation_anim.calc_mdx_size();
        sz += self.gravity_anim.calc_mdx_size();
        sz += self.visibility.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::ParticleEmitter);

        let (mut head, mut tail) = (false, false);

        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Speed" => this.speed = f.value.to()?,
                "Variation" => this.variation = f.value.to()?,
                "Latitude" => this.latitude = f.value.to()?,
                "Gravity" => this.gravity = f.value.to()?,
                "LifeSpan" => this.lifespan = f.value.to()?,
                "EmissionRate" => this.emit_rate = f.value.to()?,
                "Length" => this.length = f.value.to()?,
                "Width" => this.width = f.value.to()?,

                "Alpha" => this.segment_alpha = f.value.to_ivec(3)?.convert(|f| *f as u8),
                "ParticleScaling" => this.segment_scaling = f.value.to_fvec(3)?,
                "LifeSpanUVAnim" => this.head_life = PE2UVAnim::read_mdl(&f.value)?,
                "DecayUVAnim" => this.head_decay = PE2UVAnim::read_mdl(&f.value)?,
                "TailUVAnim" => this.tail_life = PE2UVAnim::read_mdl(&f.value)?,
                "TailDecayUVAnim" => this.tail_decay = PE2UVAnim::read_mdl(&f.value)?,

                "Rows" => this.rows = f.value.to()?,
                "Columns" => this.columns = f.value.to()?,
                "TailLength" => this.tail_length = f.value.to()?,
                "Time" => this.time = f.value.to()?,

                "TextureID" => this.texture_id = f.value.to()?,
                "PriorityPlane" => this.priority_plane = f.value.to()?,
                "ReplaceableId" => this.replace_id = f.value.to()?,

                "Squirt" => this.squirt = f.expect_flag(true)?,
                "Head" => head = f.expect_flag(true)?,
                "Tail" => tail = f.expect_flag(true)?,
                "Both" => (head, tail) = f.expect_flag((true, true))?,
                "SortPrimsFarZ" => this.base.flags |= f.expect_flag(NodeFlags::PE2SortPrimFarZ)?,
                "LineEmitter" => this.base.flags |= f.expect_flag(NodeFlags::LineEmitter)?,
                "ModelSpace" => this.base.flags |= f.expect_flag(NodeFlags::ModelSpace)?,
                "Unshaded" => this.base.flags |= f.expect_flag(NodeFlags::PE2Unshaded)?,
                "Unfogged" => this.base.flags |= f.expect_flag(NodeFlags::Unfogged)?,
                "XYQuad" => this.base.flags |= f.expect_flag(NodeFlags::XYQuad)?,

                _other => this.filter_mode = this.base.unexpect_mdl_field(f).or(PE2FilterMode::from_mdl(f))?,
            );
        }

        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "SegmentColor" => this.segment_color = f.to_array_n("Color", 3)?,
                "Speed" => this.speed_anim = Some(Animation::read_mdl(f)?),
                "Variation" => this.variation_anim = Some(Animation::read_mdl(f)?),
                "Latitude" => this.latitude_anim = Some(Animation::read_mdl(f)?),
                "Gravity" => this.gravity_anim = Some(Animation::read_mdl(f)?),
                "EmissionRate" => this.emit_rate_anim = Some(Animation::read_mdl(f)?),
                "Length" => this.length_anim = Some(Animation::read_mdl(f)?),
                "Width" => this.width_anim = Some(Animation::read_mdl(f)?),
                "Visibility" => this.visibility = Some(Animation::read_mdl(f)?),
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }

        this.head_or_tail = yesno!(head && tail, HeadOrTail::Both, yesno!(tail, HeadOrTail::Tail, HeadOrTail::Head));
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];

        lines.append(&mut self.base.write_mdl(depth)?);

        lines.push(F!("{indent}{:?},", &self.filter_mode));
        {
            let mut clines: Vec<String> = vec![];
            for c in self.segment_color.iter() {
                clines.pushx(&F!("{indent2}Color"), c);
            }
            if !clines.is_empty() {
                lines.push(F!("{indent}SegmentColor {{"));
                lines.append(&mut clines);
                lines.push(F!("{indent}}},"));
            }
        }
        {
            lines.pushx(&F!("{indent}Alpha"), &self.segment_alpha);
            lines.pushx(&F!("{indent}ParticleScaling"), &self.segment_scaling);
            lines.pushx_if_n0(&F!("{indent}LifeSpanUVAnim"), &self.head_life);
            lines.pushx_if_n0(&F!("{indent}DecayUVAnim"), &self.head_decay);
            lines.pushx_if_n0(&F!("{indent}TailUVAnim"), &self.tail_life);
            lines.pushx_if_n0(&F!("{indent}TailDecayUVAnim"), &self.tail_decay);
        }
        {
            lines.pushx_if_n0(&F!("{indent}Rows"), &self.rows);
            lines.pushx_if_n0(&F!("{indent}Columns"), &self.columns);
            lines.pushx_if_n0(&F!("{indent}Time"), &self.time);
            lines.pushx_if_n0(&F!("{indent}LifeSpan"), &self.lifespan);
            lines.pushx_if_n0(&F!("{indent}TailLength"), &self.tail_length);
            lines.pushx_if_nneg1(&F!("{indent}TextureID"), &self.texture_id);
            lines.pushx_if_n0(&F!("{indent}ReplaceableId"), &self.replace_id);
            lines.pushx_if_n0(&F!("{indent}PriorityPlane"), &self.priority_plane);
        }
        {
            let flags = self.base.flags;
            lines.push_if(flags.contains(NodeFlags::PE2SortPrimFarZ), F!("{indent}SortPrimsFarZ,"));
            lines.push_if(flags.contains(NodeFlags::LineEmitter), F!("{indent}LineEmitter,"));
            lines.push_if(flags.contains(NodeFlags::ModelSpace), F!("{indent}ModelSpace,"));
            lines.push_if(flags.contains(NodeFlags::PE2Unshaded), F!("{indent}Unshaded,"));
            lines.push_if(flags.contains(NodeFlags::Unfogged), F!("{indent}Unfogged,"));
            lines.push_if(flags.contains(NodeFlags::XYQuad), F!("{indent}XYQuad,"));
            lines.push_if(self.squirt, F!("{indent}Squirt,"));
            lines.push_if(self.head_or_tail.is_valid(), F!("{indent}{:?},", self.head_or_tail));
        }

        MdlWriteAnimBoth!(lines, depth,
            "Speed" => self.speed_anim => 0.0 => self.speed,
            "Variation" => self.variation_anim => 0.0 => self.variation,
            "Latitude" => self.latitude_anim => 0.0 => self.latitude,
            "Gravity" => self.gravity_anim => 0.0 => self.gravity,
            "EmissionRate" => self.emit_rate_anim => 0.0 => self.emit_rate,
            "Length" => self.length_anim => 0.0 => self.length,
            "Width" => self.width_anim => 0.0 => self.width,
        );
        MdlWriteAnimIfSome!(lines, depth, "Visibility" => self.visibility);

        return Ok(lines);
    }
}

// region: PE2UVAnim

#[derive(Default)]
pub struct PE2UVAnim {
    pub start: i32,
    pub end: i32,
    pub repeat: i32,
}

impl PE2UVAnim {
    pub fn read_mdl(value: &MdlValue) -> Result<Self, MyError> {
        let iv = value.to_ivec(3)?;
        Ok(Self { start: iv[0], end: iv[1], repeat: iv[2] })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.start)?;
        chunk.write(&self.end)?;
        chunk.write(&self.repeat)?;
        return Ok(());
    }
    pub fn size() -> u32 {
        12
    }
}

impl stdDebug for PE2UVAnim {
    fn fmt(&self, f: &mut stdFormatter<'_>) -> stdResult {
        write!(f, "({} ~ {}) x {}", self.start, self.end, self.repeat)
    }
}

impl CheckValue for PE2UVAnim {
    fn is0(&self) -> bool {
        (self.start, self.end, self.repeat) == (0, 0, 0)
    }
    fn is1(&self) -> bool {
        (self.start, self.end, self.repeat) == (1, 1, 1)
    }
    fn isneg1(&self) -> bool {
        (self.start, self.end, self.repeat) == (-1, -1, -1)
    }
}

impl Formatter for PE2UVAnim {
    fn fmt(&self) -> String {
        F!("{{ {}, {}, {} }}", self.start, self.end, self.repeat)
    }
}

// endregion
// region: HeadOrTail

#[derive(Debug, Default)]
pub enum HeadOrTail {
    #[default]
    Head,
    Tail,
    Both,
    Error(i32),
}

impl HeadOrTail {
    fn is_valid(&self) -> bool {
        matches!(self, Self::Head | Self::Tail | Self::Both)
    }

    fn from(v: i32) -> Self {
        match v {
            0 => Self::Head,
            1 => Self::Tail,
            2 => Self::Both,
            x => Self::Error(x),
        }
    }

    fn to(&self) -> i32 {
        match self {
            Self::Head => 0,
            Self::Tail => 1,
            Self::Both => 2,
            Self::Error(x) => *x,
        }
    }
}

// endregion
// region: PE2FilterMode

#[derive(Debug, Default)]
pub enum PE2FilterMode {
    #[default]
    Blend,
    Additive,
    Modulate,
    AlphaKey,
    Error(i32),
}

impl PE2FilterMode {
    fn from(v: i32) -> Self {
        match v {
            0 => Self::Blend,
            1 => Self::Additive,
            2 => Self::Modulate,
            4 => Self::AlphaKey,
            x => Self::Error(x),
        }
    }

    fn from_mdl(f: &MdlField) -> Result<Self, MyError> {
        match_istr!(f.name.as_str(),
            "Blend" => f.expect_flag(Self::Blend),
            "Additive" => f.expect_flag(Self::Additive),
            "Modulate" => f.expect_flag(Self::Modulate),
            "AlphaKey" => f.expect_flag(Self::AlphaKey),
            _err => f.unexpect(),
        )
    }

    fn to(&self) -> i32 {
        match self {
            Self::Blend => 0,
            Self::Additive => 1,
            Self::Modulate => 2,
            Self::AlphaKey => 4,
            Self::Error(v) => *v,
        }
    }
}

// endregion
