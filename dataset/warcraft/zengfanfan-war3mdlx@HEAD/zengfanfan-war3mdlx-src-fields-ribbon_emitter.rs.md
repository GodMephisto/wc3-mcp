---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/ribbon_emitter.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct RibbonEmitter {
    pub base: Node,

    pub height_above: f32,
    pub height_below: f32,
    pub alpha: f32,
    #[dbg(formatter = "fmtx")]
    pub color: Vec3, // RGB
    pub lifespan: f32,
    #[dbg(skip)]
    pub _unknown: i32,
    pub emit_rate: i32,
    pub rows: i32,
    pub columns: i32,
    pub material_id: i32,
    pub gravity: f32,

    #[dbg(formatter = "fmtxx")]
    pub height_above_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub height_below_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub alpha_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub color_anim: Option<Animation<Vec3>>, // BGR
    #[dbg(formatter = "fmtxx")]
    pub texslot_anim: Option<Animation<i32>>,
    #[dbg(formatter = "fmtxx")]
    pub visibility: Option<Animation<f32>>,
}

impl RibbonEmitter {
    pub const ID: u32 = MdlxMagic::RIBB;
    const ID_HA: u32 = MdlxMagic::KRHA; /* Height Above */
    const ID_HB: u32 = MdlxMagic::KRHB; /* Height Below */
    const ID_A: u32 = MdlxMagic::KRAL; /* Alpha */
    const ID_C: u32 = MdlxMagic::KRCO; /* Color */
    const ID_TS: u32 = MdlxMagic::KRTX; /* TextureSlot */
    const ID_V: u32 = MdlxMagic::KRVS; /* Visibility */

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdx(cur)? };

        this.height_above = cur.readx()?;
        this.height_below = cur.readx()?;
        this.alpha = cur.readx()?;
        this.color = cur.readx()?;
        this.lifespan = cur.readx()?;
        this._unknown = cur.readx()?;
        this.emit_rate = cur.readx()?;
        this.rows = cur.readx()?;
        this.columns = cur.readx()?;
        this.material_id = cur.readx()?;
        this.gravity = cur.readx()?;

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_HA => this.height_above_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_HB => this.height_below_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_A => this.alpha_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_C => this.color_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_TS => this.texslot_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_V => this.visibility = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        self.base.write_mdx(chunk)?;

        chunk.write(&self.height_above)?;
        chunk.write(&self.height_below)?;
        chunk.write(&self.alpha)?;
        chunk.write(&self.color)?; //3*4
        chunk.write(&self.lifespan)?;
        chunk.write(&self._unknown)?;
        chunk.write(&self.emit_rate)?;
        chunk.write(&self.rows)?;
        chunk.write(&self.columns)?;
        chunk.write(&self.material_id)?;
        chunk.write(&self.gravity)?;

        MdxWriteAnim!(chunk,
            Self::ID_HA => self.height_above_anim,
            Self::ID_HB => self.height_below_anim,
            Self::ID_A  => self.alpha_anim,
            Self::ID_C  => self.color_anim,
            Self::ID_TS => self.texslot_anim,
            Self::ID_V  => self.visibility,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 56; // sz + ha + hb + a + c + lifespan + _ + er + rows + cols + mat_id + g
        sz += self.base.calc_mdx_size();
        sz += self.height_above_anim.calc_mdx_size();
        sz += self.height_below_anim.calc_mdx_size();
        sz += self.alpha_anim.calc_mdx_size();
        sz += self.color_anim.calc_mdx_size();
        sz += self.texslot_anim.calc_mdx_size();
        sz += self.visibility.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { base: Node::read_mdl(block)? };
        this.base.flags.insert(NodeFlags::RibbonEmitter);
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "HeightAbove" => this.height_above = f.value.to()?,
                "HeightBelow" => this.height_below = f.value.to()?,
                "Alpha" => this.alpha = f.value.to()?,
                "Color" => this.color = f.value.to()?,
                "EmissionRate" => this.emit_rate = f.value.to()?,
                "LifeSpan" => this.lifespan = f.value.to()?,
                "Gravity" => this.gravity = f.value.to()?,
                "Rows" => this.rows = f.value.to()?,
                "Columns" => this.columns = f.value.to()?,
                "MaterialID" => this.material_id = f.value.to()?,
                _other => this.base.unexpect_mdl_field(f)?,
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "HeightAbove" => this.height_above_anim = Some(Animation::read_mdl(f)?),
                "HeightBelow" => this.height_below_anim = Some(Animation::read_mdl(f)?),
                "Alpha" => this.alpha_anim = Some(Animation::read_mdl(f)?),
                "Color" => this.color_anim = Some(Animation::read_mdl(f)?),
                "TextureSlot" => this.texslot_anim = Some(Animation::read_mdl(f)?),
                "Visibility" => this.visibility = Some(Animation::read_mdl(f)?),
                _other => this.base.unexpect_mdl_block(f)?,
            );
        }
        if *mdl_rgb!() {
            this.color_anim = this.color_anim.map(|a| a.convert(|v| v.reverse()));
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];

        lines.append(&mut self.base.write_mdl(depth)?);

        let bgr_anim = self.color_anim.as_ref().and_then(|a| Some(a.convert(|v| v.reverse())));
        let color_anim = yesno!(*mdl_rgb!(), &bgr_anim, &self.color_anim);

        lines.pushx_if_n0(&F!("{indent}EmissionRate"), &self.emit_rate);
        lines.pushx_if_n0(&F!("{indent}LifeSpan"), &self.lifespan);
        lines.pushx_if_n0(&F!("{indent}Gravity"), &self.gravity);
        lines.pushx_if_n0(&F!("{indent}Rows"), &self.rows);
        lines.pushx_if_n0(&F!("{indent}Columns"), &self.columns);
        lines.pushx_if_nneg1(&F!("{indent}MaterialID"), &self.material_id);

        MdlWriteAnimBoth!(lines, depth,
            "HeightAbove" => self.height_above_anim => 0.0 => self.height_above,
            "HeightBelow" => self.height_below_anim => 0.0 => self.height_below,
            "Alpha" => self.alpha_anim => 0.0 => self.alpha,
            "Color" => color_anim => Vec3::ZERO => self.color,
        );
        MdlWriteAnimIfSome!(lines, depth,
            "TextureSlot" => self.texslot_anim,
            "Visibility" => self.visibility,
        );

        return Ok(lines);
    }
}
