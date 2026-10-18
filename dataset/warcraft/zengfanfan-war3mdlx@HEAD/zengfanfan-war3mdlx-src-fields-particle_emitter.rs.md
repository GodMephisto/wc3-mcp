---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/particle_emitter.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct ParticleEmitter {
    pub base: Node,

    pub emit_rate: f32,
    pub gravity: f32,
    pub longitude: f32,
    pub latitude: f32,
    pub path: String,
    #[dbg(skip)]
    pub _unknown: i32,
    pub lifespan: f32,
    pub speed: f32,

    #[dbg(formatter = "fmtxx")]
    pub emit_rate_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub gravity_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub longitude_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub latitude_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub lifespan_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub speed_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub visibility: Option<Animation<f32>>,
}

impl ParticleEmitter {
    pub const ID: u32 = MdlxMagic::PREM;
    const ID_ER: u32 = MdlxMagic::KPEE; /* Emission rate */
    const ID_G: u32 = MdlxMagic::KPEG; /* Gravity */
    const ID_LO: u32 = MdlxMagic::KPLN; /* Longitude */
    const ID_LA: u32 = MdlxMagic::KPLT; /* Latitude */
    const ID_LS: u32 = MdlxMagic::KPEL; /* Life span */
    const ID_SPD: u32 = MdlxMagic::KPES; /* Speed */
    const ID_V: u32 = MdlxMagic::KPEV; /* Visibility */
    const PATH_SIZE: u32 = 256;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };

        this.emit_rate = cur.readx()?;
        this.gravity = cur.readx()?;
        this.longitude = cur.readx()?;
        this.latitude = cur.readx()?;
        this.path = cur.read_string(Self::PATH_SIZE)?;
        this._unknown = cur.readx()?;
        this.lifespan = cur.readx()?;
        this.speed = cur.readx()?;

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_ER => this.emit_rate_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_G => this.gravity_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_LO => this.longitude_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_LA => this.latitude_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_LS => this.lifespan_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_SPD => this.speed_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_V => this.visibility = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        self.base.write_mdx(chunk)?;

        chunk.write(&self.emit_rate)?;
        chunk.write(&self.gravity)?;
        chunk.write(&self.longitude)?;
        chunk.write(&self.latitude)?;
        chunk.write_string(&self.path, Self::PATH_SIZE)?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.lifespan)?;
        chunk.write(&self.speed)?;

        MdxWriteAnim!(chunk,
            Self::ID_ER => self.emit_rate_anim,
            Self::ID_G  => self.gravity_anim,
            Self::ID_LO => self.longitude_anim,
            Self::ID_LA => self.latitude_anim,
            Self::ID_LS => self.lifespan_anim,
            Self::ID_SPD=> self.speed_anim,
            Self::ID_V  => self.visibility,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 32 + Self::PATH_SIZE; // sz + er + g + lo + la + path + unknown + lifespan + spd
        sz += self.base.calc_mdx_size();
        sz += self.emit_rate_anim.calc_mdx_size();
        sz += self.gravity_anim.calc_mdx_size();
        sz += self.longitude_anim.calc_mdx_size();
        sz += self.latitude_anim.calc_mdx_size();
        sz += self.lifespan_anim.calc_mdx_size();
        sz += self.speed_anim.calc_mdx_size();
        sz += self.visibility.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::ParticleEmitter);
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "EmissionRate" => this.emit_rate = f.value.to()?,
                "Gravity" => this.gravity = f.value.to()?,
                "Longitude" => this.longitude = f.value.to()?,
                "Latitude" => this.latitude = f.value.to()?,
                "EmitterUsesMDL" => this.base.flags |= f.expect_flag(NodeFlags::PE1UsesMdl)?,
                "EmitterUsesTGA" => this.base.flags |= f.expect_flag(NodeFlags::PE1UsesTga)?,
                _other => this.base.unexpect_mdl_field(f)?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Particle" => this.read_mdl_particle(f)?,
                "EmissionRate" => this.emit_rate_anim = Some(Animation::read_mdl(f)?),
                "Gravity" => this.gravity_anim = Some(Animation::read_mdl(f)?),
                "Longitude" => this.longitude_anim = Some(Animation::read_mdl(f)?),
                "Latitude" => this.latitude_anim = Some(Animation::read_mdl(f)?),
                "Visibility" => this.visibility = Some(Animation::read_mdl(f)?),
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }
        return Ok(this);
    }
    fn read_mdl_particle(&mut self, block: &MdlBlock) -> Result<(), MyError> {
        block.unexpect_frames()?;
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Path" => self.path = f.value.to()?,
                "LifeSpan" => self.lifespan = f.value.to()?,
                "InitVelocity" => self.speed = f.value.to()?,
                _other => f.unexpect()?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "LifeSpan" => self.lifespan_anim = Some(Animation::read_mdl(f)?),
                "InitVelocity" => self.speed_anim = Some(Animation::read_mdl(f)?),
                _other => f.unexpect()?,
            );
        }
        return Ok(());
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        let mut lines: Vec<String> = vec![];

        lines.append(&mut self.base.write_mdl(depth)?);
        lines.push_if(self.base.flags.contains(NodeFlags::PE1UsesMdl), F!("{indent}EmitterUsesMDL,"));
        lines.push_if(self.base.flags.contains(NodeFlags::PE1UsesTga), F!("{indent}EmitterUsesTGA,"));

        MdlWriteAnimBoth!(lines, depth,
            "EmissionRate" => self.emit_rate_anim => 0.0 => self.emit_rate,
            "Gravity" => self.gravity_anim => 0.0 => self.gravity,
            "Longitude" => self.longitude_anim => 0.0 => self.longitude,
            "Latitude" => self.latitude_anim => 0.0 => self.latitude,
        );
        MdlWriteAnimIfSome!(lines, depth, "Visibility" => self.visibility);

        {
            let mut tlines: Vec<String> = vec![];
            MdlWriteAnimBoth!(tlines, depth + 1,
                "LifeSpan" => self.lifespan_anim => 0.0 => self.lifespan,
                "InitVelocity" => self.speed_anim => 0.0 => self.speed,
            );
            tlines.pushx_if_n0(&F!("{indent2}Path"), &self.path.escape_path());
            if !tlines.is_empty() {
                lines.push(F!("{indent}Particle {{"));
                lines.append(&mut tlines);
                lines.push(F!("{indent}}}"));
            }
        }

        return Ok(lines);
    }
}
