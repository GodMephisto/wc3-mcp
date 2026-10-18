---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/extends/cursor.rs"
ref: HEAD
---

use crate::*;

pub trait _ExtendCursor {
    fn pos(&self) -> u32;
    fn len(&self) -> u32;
    fn left(&self) -> u32;
    fn eol(&self) -> bool;
    fn clear(&mut self);
}
impl _ExtendCursor for Cursor<&Vec<u8>> {
    fn pos(&self) -> u32 {
        self.position() as u32
    }
    fn len(&self) -> u32 {
        self.get_ref().len() as u32
    }
    fn left(&self) -> u32 {
        self.len() - self.pos()
    }
    fn eol(&self) -> bool {
        self.position() >= self.get_ref().len() as u64
    }
    fn clear(&mut self) {
        panic!("DON'T DO THIS!");
    }
}
impl _ExtendCursor for Cursor<Vec<u8>> {
    fn pos(&self) -> u32 {
        self.position() as u32
    }
    fn len(&self) -> u32 {
        self.get_ref().len() as u32
    }
    fn left(&self) -> u32 {
        self.len() - self.pos()
    }
    fn eol(&self) -> bool {
        self.position() >= self.get_ref().len() as u64
    }
    fn clear(&mut self) {
        self.set_position(0);
        self.get_mut().clear();
    }
}

pub trait _ExtendCursorRead {
    fn readx<T: ReadFromCursor>(&mut self) -> Result<T, MyError>;
    fn read_le<T: ReadFromCursor>(&mut self) -> Result<T, MyError>;
    fn read_be<T: ReadFromCursor>(&mut self) -> Result<T, MyError>;
    fn read_bytes(&mut self, n: u32) -> Result<Vec<u8>, MyError>;
    fn read_array<T: ReadFromCursor>(&mut self, n: u32) -> Result<Vec<T>, MyError>;
    fn read_array_be<T: ReadFromCursor>(&mut self, n: u32) -> Result<Vec<T>, MyError>;
    fn read_string(&mut self, n: u32) -> Result<String, MyError>;
}
impl _ExtendCursorRead for Cursor<&Vec<u8>> {
    fn readx<T: ReadFromCursor>(&mut self) -> Result<T, MyError> {
        self.read_le()
    }
    fn read_le<T: ReadFromCursor>(&mut self) -> Result<T, MyError> {
        T::read_from(self)
    }
    fn read_be<T: ReadFromCursor>(&mut self) -> Result<T, MyError> {
        T::read_from_be(self)
    }

    fn read_bytes(&mut self, n: u32) -> Result<Vec<u8>, MyError> {
        let mut body = vec![0u8; n as usize];
        self.read_exact(&mut body)?;
        return Ok(body);
    }

    fn read_array<T: ReadFromCursor>(&mut self, n: u32) -> Result<Vec<T>, MyError> {
        let mut v = Vec::with_capacity(n as usize);
        for _ in 0..n {
            v.push(T::read_from(self)?);
        }
        return Ok(v);
    }
    fn read_array_be<T: ReadFromCursor>(&mut self, n: u32) -> Result<Vec<T>, MyError> {
        let mut v = Vec::with_capacity(n as usize);
        for _ in 0..n {
            v.push(T::read_from_be(self)?);
        }
        return Ok(v);
    }

    fn read_string(&mut self, n: u32) -> Result<String, MyError> {
        let buf = self.read_bytes(n)?;
        let end = buf.iter().position(|&b| b == 0).unwrap_or(buf.len());
        match str::from_utf8(&buf[..end]) {
            Ok(s) => Ok(s.to_string()),
            Err(e) => {
                let valid_end = e.valid_up_to();
                let mut s = unsafe { str::from_utf8_unchecked(&buf[..valid_end]).to_string() };
                for b in &buf[valid_end..end] {
                    s.push(yesno!(b.is_ascii_graphic() || *b == 0x20, *b as char, '?'));
                }
                Ok(s)
            },
        }
    }
}

pub trait _ExtendCursorWrite {
    fn writex<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError>;
    fn write_le<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError>;
    fn write_be<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError>;
    fn write_string(&mut self, s: &str, n: u32) -> Result<(), MyError>;
}
impl _ExtendCursorWrite for Cursor<Vec<u8>> {
    fn writex<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError> {
        self.write_le(v)
    }
    fn write_le<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError> {
        v.write_to(self)
    }
    fn write_be<T: WriteToCursor>(&mut self, v: &T) -> Result<(), MyError> {
        v.write_to_be(self)
    }

    fn write_string(&mut self, s: &str, n: u32) -> Result<(), MyError> {
        let n = n as usize;
        let bytes = s.as_bytes();
        let len = bytes.len().min(n);
        self.write_all(&bytes[..len])?;
        if n > len {
            self.write_all(&vec![0u8; n - len])?;
        }
        return Ok(());
    }
}

// region: trait: ReadFromCursor

pub trait ReadFromCursor: Sized {
    fn read_from(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError>;
    fn read_from_be(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError>;
}

macro_rules! impl_ReadFromCursor {
    ($($a:ty),+) => {
        $(paste! {
            impl ReadFromCursor for $a {
                fn read_from(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
                    Ok(cur.[<read_ $a>]::<LittleEndian>()?)
                }
                fn read_from_be(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
                    Ok(cur.[<read_ $a>]::<BigEndian>()?)
                }
            }
        })+
    };
}
macro_rules! impl_ReadFromCursor_VecN {
    ($($a:tt),+) => {
        $(paste! {
            impl ReadFromCursor for [<Vec $a>] {
                fn read_from(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
                    let vs = cur.read_array::<f32>($a)?;
                    Ok(Self::from_slice(vs.as_slice()))
                }
                fn read_from_be(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
                    let vs = cur.read_array_be::<f32>($a)?;
                    Ok(Self::from_slice(vs.as_slice()))
                }
            }
        })+
    };
}

impl_ReadFromCursor!(i16, u16, i32, u32, f32);
impl_ReadFromCursor_VecN!(2, 3, 4);

impl ReadFromCursor for u8 {
    fn read_from(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(cur.read_u8()?)
    }
    fn read_from_be(_: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        ERR!("DON'T DO THIS!")
    }
}
impl ReadFromCursor for usize {
    fn read_from(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(cur.read_u32::<LittleEndian>()? as Self)
    }
    fn read_from_be(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(cur.read_u32::<BigEndian>()? as Self)
    }
}

// endregion
// region: trait: WriteToCursor

pub trait WriteToCursor: Sized {
    fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError>;
    fn write_to_be(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError>;
    fn size() -> u32;
    fn calc_size(&self) -> u32;
}

macro_rules! impl_WriteToCursor {
    ($($a:ty),+) => {
        $(paste! {
            impl WriteToCursor for $a {
                fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
                    Ok(cur.[<write_ $a>]::<LittleEndian>(*self)?)
                }
                fn write_to_be(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
                    Ok(cur.[<write_ $a>]::<BigEndian>(*self)?)
                }
                fn size() -> u32 {
                    std::mem::size_of::<Self>() as u32
                }
                fn calc_size(&self) -> u32 {
                    Self::size()
                }
            }
        })+
    };
}
macro_rules! impl_WriteToCursor_VecN {
    ($($a:tt),+) => {
        $(paste! {
            impl WriteToCursor for [<Vec $a>] {
                fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
                    self.to_array().to_vec().write_to(cur)
                }
                fn write_to_be(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
                    self.to_array().to_vec().write_to(cur)
                }
                fn size() -> u32 {
                    4u32 * $a
                }
                fn calc_size(&self) -> u32 {
                    Self::size()
                }
            }
        })+
    };
}

impl_WriteToCursor!(i16, u16, i32, u32, f32);
impl_WriteToCursor_VecN!(2, 3, 4);

impl WriteToCursor for u8 {
    fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        Ok(cur.write_u8(*self)?)
    }
    fn write_to_be(&self, _: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        ERR!("DON'T DO THIS!")
    }
    fn size() -> u32 {
        1
    }
    fn calc_size(&self) -> u32 {
        Self::size()
    }
}
impl WriteToCursor for usize {
    fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        Ok(cur.write_u32::<LittleEndian>(*self as u32)?)
    }
    fn write_to_be(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        Ok(cur.write_u32::<BigEndian>(*self as u32)?)
    }
    fn size() -> u32 {
        4
    }
    fn calc_size(&self) -> u32 {
        Self::size()
    }
}

impl<T: WriteToCursor> WriteToCursor for Vec<T> {
    fn write_to(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        for v in self {
            v.write_to(cur)?;
        }
        return Ok(());
    }
    fn write_to_be(&self, cur: &mut Cursor<Vec<u8>>) -> Result<(), MyError> {
        for v in self {
            v.write_to_be(cur)?;
        }
        return Ok(());
    }
    fn size() -> u32 {
        panic!("DON'T DO THIS!"); // [instead] T::size() * self.len()
    }
    fn calc_size(&self) -> u32 {
        T::size() * self.len() as u32
    }
}

// endregion
