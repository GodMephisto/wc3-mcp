---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/mdx.rs"
ref: HEAD
---

use crate::*;

macro_rules! MdxReadType1 {
    ($chunk:expr, $cur:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $chunk.id == <$ty>::ID {
            $var = <$ty>::read_mdx(&mut $cur)
            .or_else(|e| ERR!("{}({}): {}", TNAME!($ty), u32_to_ascii(<$ty>::ID), e))?;
            return Ok(());
        })+
    };
}
macro_rules! MdxReadType2 {
    ($chunk:expr, $cur:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $chunk.id == <$ty>::ID {
            while !$cur.eol() {
                $var.push(
                    <$ty>::read_mdx(&mut $cur).or_else(|e| {
                        ERR!("{}({})[{}th]: {}", TNAME!($ty), u32_to_ascii(<$ty>::ID), $var.len(), e)
                    })?,
                );
            }
            return Ok(());
        })+
    };
}
macro_rules! MdxReadType3 {
    ($chunk:expr, $cur:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $chunk.id == <$ty>::ID {
            while !$cur.eol() {
                let left = $cur.left();
                yes!(left < 4, EXIT!("{} size: {}B left (need 4)", TNAME!($ty), left));
                let sz = $cur.readx::<u32>()?;
                yes!(sz < 4, EXIT!("{} size: {} (need >= 4)", TNAME!($ty), sz));
                let sz = sz - 4;

                let left = $cur.left();
                yes!(left < sz, EXIT!("{} body: {}B left (need {})", TNAME!($ty), left, sz));
                let body = $cur.read_bytes(sz).or_else(|e| ERR!("{} body({}B): {}", TNAME!($ty), sz, e))?;

                let mut cur2 = Cursor::new(&body);
                $var.push(
                    <$ty>::read_mdx(&mut cur2).or_else(|e| {
                        ERR!("{}({})[{}th]: {}", TNAME!($ty), u32_to_ascii(<$ty>::ID), $var.len(), e)
                    })?,
                );
            }
            EXIT!();
        })+
    };
}

macro_rules! MdxWriteType1 {
    ($cur:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(
            let mut chunk = MdxChunk::new(<$ty>::ID);
            $var.write_mdx(&mut chunk)
            .or_else(|e| ERR!("{}: {}", TNAME!($ty), e))?;
            chunk.flush_to(&mut $cur)?;
        )+
    };
}
macro_rules! MdxWriteType2 {
    ($cur:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if !$var.is_empty() {
            let mut chunk = MdxChunk::new(<$ty>::ID);
            for a in $var.iter() {
                a.write_mdx(&mut chunk).or_else(|e| ERR!("{}: {}", TNAME!($ty), e))?;
            }
            chunk.flush_to(&mut $cur)?;
        })+
    };
}

impl MdlxData {
    pub fn write_mdx(&self, path: &Path) -> Result<(), MyError> {
        let mut cur = Cursor::new(Vec::<u8>::with_capacity(0x40000_usize));

        if let Err(e) = cur.write_be(&MdlxMagic::MDLX) {
            EXIT1!("writing magic: {}", e);
        }

        MdxWriteType1!(cur,
            Version         => self.version,
            Model           => self.model,
        );
        MdxWriteType2!(cur,
            Sequence        => self.sequences,
            GlobalSequence  => self.globalseqs,
            Material        => self.materials,
            Texture         => self.textures,
            TextureAnim     => self.texanims,
            Geoset          => self.geosets,
            GeosetAnim      => self.geoanims,
            Bone            => self.bones,
            Light           => self.lights,
            Helper          => self.helpers,
            Attachment      => self.attachments,
            PivotPoint      => self.pivot_points,
            ParticleEmitter => self.particle_emitters,
            ParticleEmitter2=> self.particle_emitters2,
            RibbonEmitter   => self.ribbon_emitters,
            Camera          => self.cameras,
            EventObject     => self.eventobjs,
            CollisionShape  => self.collisions,
        );

        std::fs::write(path, cur.into_inner())?;
        EXIT!();
    }

    pub fn read_mdx(input: &Vec<u8>) -> Result<Self, MyError> {
        let mut this = MdlxData::default();
        let mut cur = Cursor::new(input);

        let magic = cur.read_be::<u32>().unwrap_or(0);
        if magic != MdlxMagic::MDLX as u32 {
            return ERR!("Invalid magic: 0x{:08X} ({})", magic, u32_to_ascii(magic));
        }

        while !cur.eol() {
            let chunk = MdxChunk::read_mdx(&mut cur)?;
            this.parse_mdx_chunk(&chunk)?;
        }

        vvvlog!("{}", F!("{:#?}", &this).replace("    ", "  "));
        return Ok(this);
    }

    fn parse_mdx_chunk(&mut self, chunk: &MdxChunk) -> Result<(), MyError> {
        let mut cur = Cursor::new(&chunk.body);
        MdxReadType1!(chunk, cur,
            Version         => self.version,
            Model           => self.model,
        );
        MdxReadType2!(chunk, cur,
            Sequence        => self.sequences,
            GlobalSequence  => self.globalseqs,
            Texture         => self.textures,
            Bone            => self.bones,
            Helper          => self.helpers,
            EventObject     => self.eventobjs,
            CollisionShape  => self.collisions,
            PivotPoint      => self.pivot_points,
        );
        MdxReadType3!(chunk, cur,
            TextureAnim     => self.texanims,
            Material        => self.materials,
            Geoset          => self.geosets,
            GeosetAnim      => self.geoanims,
            Attachment      => self.attachments,
            Light           => self.lights,
            ParticleEmitter => self.particle_emitters,
            ParticleEmitter2=> self.particle_emitters2,
            RibbonEmitter   => self.ribbon_emitters,
            Camera          => self.cameras,
        );
        return Ok(());
    }
}
