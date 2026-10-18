---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/node.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, SmartDefault)]
pub struct Node {
    pub name: String,
    pub object_id: i32,
    #[default(-1)]
    pub parent_id: i32,
    #[dbg(fmt = "{:?}")]
    pub flags: NodeFlags, // see NodeFlags

    #[dbg(formatter = "fmtxx")]
    pub translation: Option<Animation<Vec3>>,
    #[dbg(formatter = "fmtxx")]
    pub rotation: Option<Animation<Vec4>>,
    #[dbg(formatter = "fmtxx")]
    pub scaling: Option<Animation<Vec3>>,

    mdl_fields: HashSet<String>,
    mdl_blocks: HashSet<String>,
    #[default(Ok(()))]
    mdl_unexpected_field: Result<(), MyError>,
    #[default(Ok(()))]
    mdl_unexpected_block: Result<(), MyError>,
}

impl Node {
    const NAME_SIZE: u32 = 80;
    const ID_T: u32 = MdlxMagic::KGTR; /* Translation */
    const ID_R: u32 = MdlxMagic::KGRT; /* Rotation */
    const ID_S: u32 = MdlxMagic::KGSC; /* Scaling */

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build!();

        let sz = cur.readx::<u32>()?;
        yes!(sz < 4, EXIT1!("{} node size: {} (need >= 4)", TNAME!(), sz));
        let body = cur.read_bytes(sz - 4)?;
        let mut cur = Cursor::new(&body); // use a new cursor

        this.name = cur.read_string(Self::NAME_SIZE)?;
        this.object_id = cur.readx()?;
        this.parent_id = cur.readx()?;
        this.flags = NodeFlags::from_bits_retain(cur.readx()?);

        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_T => this.translation = Some(Animation::read_mdx(&mut cur)?),
                Self::ID_R => this.rotation = Some(Animation::read_mdx(&mut cur)?),
                Self::ID_S => this.scaling = Some(Animation::read_mdx(&mut cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }

        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        chunk.write_string(&self.name, Self::NAME_SIZE)?;
        chunk.write(&self.object_id)?;
        chunk.write(&self.parent_id)?;
        chunk.write(&self.flags.bits())?;
        MdxWriteAnim!(chunk,
            Self::ID_T => self.translation,
            Self::ID_R => self.rotation,
            Self::ID_S => self.scaling,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = Self::NAME_SIZE + 16; // sz + name + object_id + parent_id + flags
        sz += self.translation.calc_mdx_size();
        sz += self.rotation.calc_mdx_size();
        sz += self.scaling.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        let mut this = Build! { name: block.name.clone() };
        for f in &block.fields {
            let (n, mut hit) = (&f.name, true);
            match_istr!(n,
                "ObjectId" => this.object_id = f.value.to()?,
                "Parent" => this.parent_id = f.value.to()?,
                "DontInherit" => {
                    let flags: Vec<String> = f.value.to()?;
                    for s in &flags {
                        match_istr!(s.as_str(),
                            "Translation" => this.flags |= NodeFlags::DontInheritT,
                            "Rotation" => this.flags |= NodeFlags::DontInheritR,
                            "Scaling" => this.flags |= NodeFlags::DontInheritS,
                            _other => f.value.expect_but("'Translation', 'Rotation', or 'Scaling'", _other)?,
                        );
                    }
                },
                "Billboarded" => this.flags |= f.expect_flag(NodeFlags::Billboarded)?,
                "BillboardedLockX" => this.flags |= f.expect_flag(NodeFlags::BillboardedLockX)?,
                "BillboardedLockY" => this.flags |= f.expect_flag(NodeFlags::BillboardedLockY)?,
                "BillboardedLockZ" => this.flags |= f.expect_flag(NodeFlags::BillboardedLockZ)?,
                "CameraAnchored" => this.flags |= f.expect_flag(NodeFlags::CameraAnchored)?,
                _other => hit = false,
            );
            if hit {
                this.mdl_fields.insert(n.s());
            } else if let Ok(()) = this.mdl_unexpected_field {
                this.mdl_unexpected_field = f.unexpect();
            }
        }
        for f in &block.blocks {
            let (t, mut hit) = (&f.typ, true);
            match_istr!(t,
                "Translation" => this.translation = Some(Animation::read_mdl(f)?),
                "Rotation" => this.rotation = Some(Animation::read_mdl(f)?),
                "Scaling" => this.scaling = Some(Animation::read_mdl(f)?),
                _other => hit = false,
            );
            if hit {
                this.mdl_blocks.insert(t.s());
            } else if let Ok(()) = this.mdl_unexpected_block {
                this.mdl_unexpected_block = f.unexpect();
            }
        }
        return Ok(this);
    }

    pub fn unexpect_mdl_field<T: Default>(&self, f: &MdlField) -> Result<T, MyError> {
        yesno!(self.mdl_fields.contains(&f.name), Ok(T::default()), f.unexpect())
    }
    pub fn unexpect_mdl_block<T: Default>(&self, f: &MdlBlock) -> Result<T, MyError> {
        yesno!(self.mdl_blocks.contains(&f.typ), Ok(T::default()), f.unexpect())
    }
    pub fn unexpect_mdl_fields(&mut self) -> Result<(), MyError> {
        std::mem::replace(&mut self.mdl_unexpected_field, Ok(()))
    }
    pub fn unexpect_mdl_blocks(&mut self) -> Result<(), MyError> {
        std::mem::replace(&mut self.mdl_unexpected_block, Ok(()))
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];

        lines.push(F!("{indent}ObjectId {},", self.object_id));
        lines.push_if_nneg1(&F!("{indent}Parent"), &self.parent_id);

        lines.push_if(self.flags.contains(NodeFlags::DontInheritT), F!("{indent}DontInherit {{ Translation }},"));
        lines.push_if(self.flags.contains(NodeFlags::DontInheritR), F!("{indent}DontInherit {{ Rotation }},"));
        lines.push_if(self.flags.contains(NodeFlags::DontInheritS), F!("{indent}DontInherit {{ Scaling }},"));
        lines.push_if(self.flags.contains(NodeFlags::Billboarded), F!("{indent}Billboarded,"));
        lines.push_if(self.flags.contains(NodeFlags::BillboardedLockX), F!("{indent}BillboardedLockX,"));
        lines.push_if(self.flags.contains(NodeFlags::BillboardedLockY), F!("{indent}BillboardedLockY,"));
        lines.push_if(self.flags.contains(NodeFlags::BillboardedLockZ), F!("{indent}BillboardedLockZ,"));
        lines.push_if(self.flags.contains(NodeFlags::CameraAnchored), F!("{indent}CameraAnchored,"));

        MdlWriteAnimIfSome!(lines, depth,
            "Translation"   => self.translation,
            "Rotation"      => self.rotation,
            "Scaling"       => self.scaling,
        );

        return Ok(lines);
    }
}

// region: NodeFlags

bitflags! {
    #[derive(Debug, Default, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
    pub struct NodeFlags: u32 {
        const All = !0;
        const Helper = 0;
        const DontInheritT = 1 << 0;
        const DontInheritR = 1 << 1;
        const DontInheritS = 1 << 2;
        const Billboarded = 1 << 3;
        const BillboardedLockX = 1 << 4;
        const BillboardedLockY = 1 << 5;
        const BillboardedLockZ = 1 << 6;
        const CameraAnchored = 1 << 7;
        const Bone = 1 << 8;
        const Light = 1 << 9;
        const EventObject = 1 << 10;
        const Attachment = 1 << 11;
        const ParticleEmitter = 1 << 12;
        const CollisionShape = 1 << 13;
        const RibbonEmitter = 1 << 14;
        const PE2Unshaded = 1 << 15;     // ParticleEmitter2.Unshaded
        const PE1UsesMdl = 1 << 15;      // ParticleEmitter.EmitterUsesMdl
        const PE2SortPrimFarZ = 1 << 16; // ParticleEmitter2.SortPrimitivesFarZ
        const PE1UsesTga = 1 << 16;      // ParticleEmitter.EmitterUsesTga
        const LineEmitter = 1 << 17;
        const Unfogged = 1 << 18;
        const ModelSpace = 1 << 19;
        const XYQuad = 1 << 20;
    }
}

// endregion
