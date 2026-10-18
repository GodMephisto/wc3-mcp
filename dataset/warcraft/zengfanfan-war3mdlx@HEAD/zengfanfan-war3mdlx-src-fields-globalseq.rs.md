---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/globalseq.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct GlobalSequence {
    pub duration: u32,
}

impl GlobalSequence {
    pub const ID: u32 = MdlxMagic::GLBS;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Build! { duration: cur.readx()? })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.duration)
    }

    pub fn read_mdl(f: &MdlField) -> Result<Self, MyError> {
        yesno!(f.name.eq_icase("Duration"), Ok(Self { duration: f.value.to()? }), f.unexpect())
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        Ok(vec![F!("{}Duration {},", indent!(depth), self.duration)])
    }
}

impl Formatter for Vec<GlobalSequence> {
    fn fmt(&self) -> String {
        let s = self.iter().map(|x| fmtx(&x.duration)).collect::<Vec<_>>().join(", ");
        return F!("[{}]", s);
    }
}
