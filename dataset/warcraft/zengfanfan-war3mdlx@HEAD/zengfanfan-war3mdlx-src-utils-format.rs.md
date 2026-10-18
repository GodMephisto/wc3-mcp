---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/format.rs"
ref: HEAD
---

use crate::*;

#[macro_export]
macro_rules! F {
    () => {{
        String::new()
    }};
    ($var:ident) => {{
        $var.to_string()
    }};
    ($($arg:tt)*) => {{
        format!($($arg)*)
    }};
}

pub fn fmtx<T: Formatter>(v: &T) -> String {
    T::fmt(v)
}
pub fn fmtxx<T: FormatterXX>(v: &T) -> String {
    T::fmtxx(v)
}

fn trim_float_str(s: &str) -> String {
    let s = s.trim();
    let signstr = yesno!(s.starts_with('-'), "-", "");
    let s = s.trim_start_matches('-');
    let (i, f) = match s.split_once('.') {
        Some((i, f)) => (i, f),
        None => (s, ""),
    };
    let mut i = i.trim_start_matches(&[' ', '0']);
    let f = f.trim_end_matches(&[' ', '0']);
    yes!(i.len() == 0, i = "0");
    if f.len() == 0 {
        return F!("{}{}", yesno!(i == "0", "", signstr), i);
    } else {
        return F!("{}{}.{}", signstr, i, f);
    }
}

pub fn fmt_float(v: &f32, len: u32, precision: u32) -> String {
    let (len, precision) = (len as usize, precision as usize);
    let s = F!("{:.*}", precision, v);
    let s = trim_float_str(&s);
    let ret = if s.len() <= len {
        s
    } else {
        let s = F!("{:.*e}", precision, v);
        let (i, e) = s.split_once('e').unwrap();
        let i = trim_float_str(&i);
        let s = F!("{}e{}", i, e);
        let ev: i32 = e.parse().unwrap();
        match ev.abs() as usize > (len + precision) / 2 {
            true => s,
            false => {
                let v: f32 = s.parse().unwrap();
                let s = F!("{:.*}", precision, v);
                trim_float_str(&s)
            },
        }
    };
    if let Ok(f) = ret.parse::<f32>() {
        return yesno!(f == 0.0, "0".to_string(), ret); // avoid "-0"
    } else {
        return ret;
    }
}

#[allow(dead_code)]
pub fn fmt_id4s(v: &u32) -> String {
    u32_to_ascii(*v)
}

pub fn u32_to_ascii(n: u32) -> String {
    let bytes = n.to_be_bytes();
    String::from_utf8_lossy(&bytes).into_owned()
}

// region: trait: Formatter

pub trait Formatter {
    fn fmt(&self) -> String;
}
pub trait FormatterXX {
    fn fmtxx(&self) -> String;
}

macro_rules! impl_Formatter {
    ($($t:ty),*) => {
        $(impl Formatter for $t {
            fn fmt(&self) -> String { self.to_string() }
        })*
    };
}
macro_rules! impl_Formatter_array {
    ($($t:ty),*) => {
        $(
            impl Formatter for Vec<$t> {
                fn fmt(&self) -> String {
                    F!("{{ {} }}", self.iter().map(|x| Formatter::fmt(x)).collect::<Vec<_>>().join(", "))
                }
            }
            impl Formatter for &[$t] {
                fn fmt(&self) -> String { Formatter::fmt(&self.to_vec()) }
            }
        )*
    };
}
macro_rules! impl_Formatter_vecN {
    ($($t:ty),*) => {
        $(
            impl Formatter for $t {
                fn fmt(&self) -> String {
                    let s = Formatter::fmt(&self.to_array().to_vec());
                    F!("{{{}}}", &s[1..s.len()-1])
                }
            }
        )*
    };
}

impl_Formatter!(i8, u8, i16, u16, i32, u32);
impl_Formatter_array!(i8, u8, i16, u16, i32, u32, f32);
impl_Formatter_vecN!(Vec2, Vec3, Vec4);
impl_Formatter_array!(Vec2, Vec3, Vec4);
impl_Formatter_array!(Vec<Vec2>, Vec<Vec3>, Vec<Vec4>);

impl Formatter for f32 {
    fn fmt(&self) -> String {
        let p = *precision!() as u32;
        return fmt_float(self, p * 2 + 1, p);
    }
}
impl Formatter for String {
    fn fmt(&self) -> String {
        F!("\"{self}\"")
    }
}
impl Formatter for str {
    fn fmt(&self) -> String {
        F!("\"{self}\"")
    }
}

impl<T: stdDebug> Formatter for Option<T> {
    fn fmt(&self) -> String {
        match self {
            Some(v) => F!("{:?}", v),
            None => "None".to_string(),
        }
    }
}
impl<T: stdDebug> FormatterXX for Option<T> {
    fn fmtxx(&self) -> String {
        match self {
            Some(v) => F!("{:#?}", v),
            None => "None".to_string(),
        }
    }
}

pub trait _ExtendFormatter {
    fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError>;
}
impl<T: Formatter> _ExtendFormatter for T {
    fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        Ok(vec![F!("{}{},", indent!(depth), fmtx(self))])
    }
}

// endregion
