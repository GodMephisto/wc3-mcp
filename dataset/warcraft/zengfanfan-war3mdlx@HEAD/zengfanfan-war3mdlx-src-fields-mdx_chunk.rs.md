---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/mdx_chunk.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct MdxChunk {
    ///* read */
    pub id: u32,
    pub size: u32,
    pub body: Vec<u8>,
    ///* write */
    #[dbg(skip)]
    cursor: Option<Cursor<Vec<u8>>>,
}

impl MdxChunk {
    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        yes!(left = cur.left(), left < 4, EXIT1!("reading chunk id: {}B left (need 4)", left));
        let id = cur.read_be()?;
        let estr = F!("reading chunk 0x{id:08X}({})", u32_to_ascii(id));

        yes!(left = cur.left(), left < 4, EXIT1!("{} size: {}B left (need 4)", estr, left));
        let size = cur.readx().or_else(|e| ERR!("{} size: {}", estr, e))?;
        vlog!("chunk = 0x{:08X} ({}) [{}]", id, u32_to_ascii(id), size);

        yes!(left = cur.left(), left < size, EXIT1!("{} body: {}B left (need {})", estr, left, size));
        let body = cur.read_bytes(size).or_else(|e| ERR!("{} body({}B): {}", estr, size, e))?;
        vvvlog!("{}", hexdump(&body, "\t"));

        return Ok(MdxChunk { id, size, body, cursor: None });
    }

    pub fn new(id: u32) -> Self {
        Build! { id:id, cursor: Some(Cursor::new(vec![])) }
    }

    pub fn write<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError> {
        if let Some(cur) = &mut self.cursor {
            v.write_to(cur)?;
        }
        return Ok(());
    }
    pub fn write_be<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError> {
        if let Some(cur) = &mut self.cursor {
            v.write_to_be(cur)?;
        }
        return Ok(());
    }

    pub fn write_string(&mut self, s: &str, n: u32) -> Result<(), MyError> {
        if let Some(cur) = &mut self.cursor {
            cur.write_string(s, n)?;
        }
        return Ok(());
    }

    pub fn flush_to(&mut self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        cur.write_be(&self.id)?;
        if let Some(body_cur) = &mut self.cursor {
            cur.writex(&body_cur.len())?;
            cur.write_all(body_cur.get_ref())?;
            body_cur.clear();
        } else {
            cur.write_all(&self.body)?;
            self.body.clear();
        }
        return Ok(());
    }
}
