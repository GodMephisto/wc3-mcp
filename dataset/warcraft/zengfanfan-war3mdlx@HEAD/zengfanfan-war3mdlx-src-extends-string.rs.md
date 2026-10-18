---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/extends/string.rs"
ref: HEAD
---

use crate::*;

lazy_static! {
    static ref re_trailing_backslash: Regex = Regex::new(r"\\+$").unwrap();
}

pub trait _ExtendString {
    #[allow(unused)]
    fn or(&self, def: &str) -> String;
    fn eq_icase(&self, s: &str) -> bool;
    fn escape(&self) -> String;
    fn escape_path(&self) -> String;
    fn unescape(&self) -> String;
}

impl _ExtendString for str {
    fn or(&self, def: &str) -> String {
        if self.is_empty() { def.to_string() } else { self.to_string() }
    }

    fn eq_icase(&self, s: &str) -> bool {
        self.eq_ignore_ascii_case(s)
    }

    fn escape(&self) -> String {
        self.replace("\"", "\\\"").replace("\\", "\\\\")
    }

    fn escape_path(&self) -> String {
        re_trailing_backslash
            .replace_all(&self.replace("\"", "\\\""), |caps: &regex::Captures| "\\".repeat(caps[0].len() * 2))
            .to_string()
    }

    fn unescape(&self) -> String {
        self.replace("\\\\", "\\").replace("\\\"", "\"")
    }
}

#[allow(dead_code)]
pub trait S {
    fn s(&self) -> String;
}
impl<T: ToString> S for T {
    fn s(&self) -> String {
        self.to_string()
    }
}

#[allow(dead_code)]
pub trait _ExtendStringArray {
    fn push_if(&mut self, cond: bool, s: String);
}
impl _ExtendStringArray for Vec<String> {
    fn push_if(&mut self, cond: bool, s: String) {
        yes!(cond, self.push(s));
    }
}

#[allow(dead_code)]
pub trait _ExtendStringArrayDisplay<T> {
    fn push_if_n0(&mut self, name: &str, v: &T);
    fn push_if_n1(&mut self, name: &str, v: &T);
    fn push_if_nneg1(&mut self, name: &str, v: &T);
}
impl<T: CheckValue + Display> _ExtendStringArrayDisplay<T> for Vec<String> {
    fn push_if_n0(&mut self, name: &str, v: &T) {
        yes!(!v.is0(), self.push(F!("{} {},", name, v)));
    }
    fn push_if_n1(&mut self, name: &str, v: &T) {
        yes!(!v.is1(), self.push(F!("{} {},", name, v)));
    }
    fn push_if_nneg1(&mut self, name: &str, v: &T) {
        yes!(!v.isneg1(), self.push(F!("{} {},", name, v)));
    }
}

#[allow(dead_code)]
pub trait _ExtendStringArrayFMTX<T> {
    fn pushx(&mut self, name: &str, v: &T);
}
impl<T: Formatter> _ExtendStringArrayFMTX<T> for Vec<String> {
    fn pushx(&mut self, name: &str, v: &T) {
        self.push(F!("{} {},", name, v.fmt()));
    }
}

#[allow(dead_code)]
pub trait _ExtendStringArrayIfFMTX<T> {
    fn pushx_if_n0(&mut self, name: &str, v: &T);
    fn pushx_if_n1(&mut self, name: &str, v: &T);
    fn pushx_if_nneg1(&mut self, name: &str, v: &T);
}
impl<T: CheckValue + Formatter> _ExtendStringArrayIfFMTX<T> for Vec<String> {
    fn pushx_if_n0(&mut self, name: &str, v: &T) {
        yes!(!v.is0(), self.pushx(name, v));
    }
    fn pushx_if_n1(&mut self, name: &str, v: &T) {
        yes!(!v.is1(), self.pushx(name, v));
    }
    fn pushx_if_nneg1(&mut self, name: &str, v: &T) {
        yes!(!v.isneg1(), self.pushx(name, v));
    }
}

pub trait _DisplayX {
    fn s(&self) -> String;
    fn to_string(&self) -> String;
}

impl<T: Display> _DisplayX for Vec<T> {
    fn s(&self) -> String {
        self.to_string()
    }
    fn to_string(&self) -> String {
        F!("[{}]", self.iter().map(|v| v.to_string()).collect::<Vec<_>>().join(", "))
    }
}
