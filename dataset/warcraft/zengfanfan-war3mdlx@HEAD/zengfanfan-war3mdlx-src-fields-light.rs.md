---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/light.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Light {
    pub base: Node,

    #[dbg(fmt = "{:?}")]
    pub typ: LightType,
    pub attenuate_start: f32,
    pub attenuate_end: f32,
    #[dbg(formatter = "fmtx")]
    pub color: Vec3, // RGB
    pub intensity: f32,
    #[dbg(formatter = "fmtx")]
    pub amb_color: Vec3, // RGB
    pub amb_intensity: f32,

    #[dbg(formatter = "fmtxx")]
    pub attenuate_start_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub attenuate_end_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub color_anim: Option<Animation<Vec3>>, // BGR
    #[dbg(formatter = "fmtxx")]
    pub intensity_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub amb_color_anim: Option<Animation<Vec3>>, // BGR
    #[dbg(formatter = "fmtxx")]
    pub amb_intensity_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub visibility: Option<Animation<f32>>,
}

impl Light {
    pub const ID: u32 = MdlxMagic::LITE;
    const ID_AS: u32 = MdlxMagic::KLAS; /* Attenuate start */
    const ID_AE: u32 = MdlxMagic::KLAE; /* Attenuate end */
    const ID_C: u32 = MdlxMagic::KLAC; /* Color */
    const ID_I: u32 = MdlxMagic::KLAI; /* Intensity */
    const ID_AC: u32 = MdlxMagic::KLBC; /* Ambient color */
    const ID_AI: u32 = MdlxMagic::KLBI; /* Ambient intensity */
    const ID_V: u32 = MdlxMagic::KLAV; /* Visibility */

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };

        this.typ = LightType::from(cur.readx()?);
        if let LightType::Error(v) = this.typ {
            return ERR!("Unknown light type: {}", v);
        }

        this.attenuate_start = cur.readx()?;
        this.attenuate_end = cur.readx()?;
        this.color = cur.readx()?;
        this.intensity = cur.readx()?;
        this.amb_color = cur.readx()?;
        this.amb_intensity = cur.readx()?;

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_AS => this.attenuate_start_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_AE => this.attenuate_end_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_C => this.color_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_I => this.intensity_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_AC => this.amb_color_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_AI => this.amb_intensity_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_V => this.visibility = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        self.base.write_mdx(chunk)?;

        chunk.write(&self.typ.to())?;
        chunk.write(&self.attenuate_start)?;
        chunk.write(&self.attenuate_end)?;
        chunk.write(&self.color)?;
        chunk.write(&self.intensity)?;
        chunk.write(&self.amb_color)?;
        chunk.write(&self.amb_intensity)?;

        MdxWriteAnim!(chunk,
            Self::ID_AS => self.attenuate_start_anim,
            Self::ID_AE => self.attenuate_end_anim,
            Self::ID_C  => self.color_anim,
            Self::ID_I  => self.intensity_anim,
            Self::ID_AC => self.amb_color_anim,
            Self::ID_AI => self.amb_intensity_anim,
            Self::ID_V  => self.visibility,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 48; // sz + type + as + ae + color + intensity + amb_color + amb_intensity
        sz += self.base.calc_mdx_size();
        sz += self.attenuate_start_anim.calc_mdx_size();
        sz += self.attenuate_end_anim.calc_mdx_size();
        sz += self.color_anim.calc_mdx_size();
        sz += self.intensity_anim.calc_mdx_size();
        sz += self.amb_color_anim.calc_mdx_size();
        sz += self.amb_intensity_anim.calc_mdx_size();
        sz += self.visibility.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::Light);

        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "AttenuationStart" => this.attenuate_start = f.value.to()?,
                "AttenuationEnd" => this.attenuate_end = f.value.to()?,
                "Color" => this.color = f.value.to()?,
                "Intensity" => this.intensity = f.value.to()?,
                "AmbColor" => this.amb_color = f.value.to()?,
                "AmbIntensity" => this.amb_intensity = f.value.to()?,
                _other => this.typ = this.base.unexpect_mdl_field(f).or(LightType::from_mdl(f))?,
            );
        }

        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "AttenuationStart" => this.attenuate_start_anim = Some(Animation::read_mdl(f)?),
                "AttenuationEnd" => this.attenuate_end_anim = Some(Animation::read_mdl(f)?),
                "Color" => this.color_anim = Some(Animation::read_mdl(f)?),
                "Intensity" => this.intensity_anim = Some(Animation::read_mdl(f)?),
                "AmbColor" => this.amb_color_anim = Some(Animation::read_mdl(f)?),
                "AmbIntensity" => this.amb_intensity_anim = Some(Animation::read_mdl(f)?),
                "Visibility" => this.visibility = Some(Animation::read_mdl(f)?),
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }

        if *mdl_rgb!() {
            this.color_anim = this.color_anim.map(|a| a.convert(|v| v.reverse()));
            this.amb_color_anim = this.amb_color_anim.map(|a| a.convert(|v| v.reverse()));
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];

        lines.append(&mut self.base.write_mdl(depth)?);
        lines.push(F!("{indent}{:?},", self.typ));

        let bgr_anim = self.color_anim.as_ref().and_then(|a| Some(a.convert(|v| v.reverse())));
        let bgr2_anim = self.amb_color_anim.as_ref().and_then(|a| Some(a.convert(|v| v.reverse())));
        let color_anim = yesno!(*mdl_rgb!(), &bgr_anim, &self.color_anim);
        let amb_color_anim = yesno!(*mdl_rgb!(), &bgr2_anim, &self.amb_color_anim);

        MdlWriteAnimBoth!(lines, depth,
            "AttenuationStart" => self.attenuate_start_anim => 0.0 => self.attenuate_start,
            "AttenuationEnd" => self.attenuate_end_anim => 0.0 => self.attenuate_end,
            "Color" => color_anim => Vec3::ZERO => self.color,
            "Intensity" => self.intensity_anim => 0.0 => self.intensity,
            "AmbColor" => amb_color_anim => Vec3::ZERO => self.amb_color,
            "AmbIntensity" => self.amb_intensity_anim => 0.0 => self.amb_intensity,
        );
        MdlWriteAnimIfSome!(lines, depth, "Visibility" => self.visibility);

        return Ok(lines);
    }
}

// region: LightType

#[derive(Debug, Default)]
pub enum LightType {
    #[default]
    Omnidirectional,
    Directional,
    Ambient,
    Error(i32),
}

impl LightType {
    fn from(v: i32) -> Self {
        match v {
            0 => Self::Omnidirectional,
            1 => Self::Directional,
            2 => Self::Ambient,
            _ => Self::Error(v),
        }
    }

    fn from_mdl(f: &MdlField) -> Result<Self, MyError> {
        match_istr!(f.name.as_str(),
            "Omnidirectional" => f.expect_flag(Self::Omnidirectional),
            "Directional" => f.expect_flag(Self::Directional),
            "Ambient" => f.expect_flag(Self::Ambient),
            _err => f.unexpect(),
        )
    }

    fn to(&self) -> i32 {
        match self {
            Self::Omnidirectional => 0,
            Self::Directional => 1,
            Self::Ambient => 2,
            Self::Error(v) => *v,
        }
    }
}

// endregion
