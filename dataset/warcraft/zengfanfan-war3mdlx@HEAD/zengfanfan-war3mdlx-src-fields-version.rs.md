---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/version.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Version {
    pub format_version: i32,
}

impl Version {
    pub const ID: u32 = MdlxMagic::VERS;
    pub const SUPPORTED_VERSION: [i32; 1] = [800];

    fn validate(&self) -> Result<(), MyError> {
        let ver = self.format_version;
        let svers = Version::SUPPORTED_VERSION.to_vec();
        if !svers.contains(&ver) {
            EXIT1!("Unsupported version {ver} (must be {})", svers.to_or_string());
        }
        return Ok(());
    }

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let this = Self { format_version: cur.readx()? };
        this.validate()?;
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.format_version)
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        block.unexpect_blocks()?;
        let mut this = Build!();
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "FormatVersion" => this.format_version = f.value.to()?,
                _other => f.unexpect()?,
            );
        }
        this.validate()?;
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2) = (indent!(depth), indent!(depth + 1));
        return Ok(vec![
            F!("{indent}Version {{"),
            F!("{indent2}FormatVersion {},", self.format_version),
            F!("{indent}}}"),
        ]);
    }
}
