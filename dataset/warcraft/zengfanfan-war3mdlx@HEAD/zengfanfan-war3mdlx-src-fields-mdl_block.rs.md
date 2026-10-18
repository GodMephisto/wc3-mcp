---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/mdl_block.rs"
ref: HEAD
---

use crate::*;

// region: trait: ExtendPair

trait _ExtendPair {
    fn lineno(&self) -> u32;
}
impl _ExtendPair for Pair<'_, Rule> {
    fn lineno(&self) -> u32 {
        self.line_col().0 as u32
    }
}

// endregion
// region: MdlBlock

#[derive(Dbg, Default)]
pub struct MdlBlock {
    pub typ: String,
    pub name: String,
    pub scope: String,
    pub line: u32,
    #[dbg(formatter = "fmtx")]
    pub fields: Vec<MdlField>,
    pub frames: Vec<MdlFrame>,
    pub blocks: Vec<MdlBlock>,
}

impl MdlBlock {
    pub fn from(pair: Pair<'_, Rule>, scope: &str) -> Result<Self, MyError> {
        let mut this = Build! {line: pair.lineno(), scope: scope.s()};
        let inner = pair.into_inner();
        for p in inner {
            match p.as_rule() {
                Rule::identifier => this.typ = p.as_str().s(),
                Rule::string => this.name = MdlValue::unwrap_string(p.as_str()),
                Rule::block => this.blocks.push(MdlBlock::from(p, &this.subscope().as_str())?),
                Rule::field => this.fields.push(MdlField::from(p, &this.subscope().as_str())?),
                Rule::frame => this.frames.push(MdlFrame::from(p, &this.subscope().as_str())?),
                _other => (), // ignore integers
            }
        }
        return Ok(this);
    }

    fn subscope(&self) -> String {
        yesno!(self.name.is_empty(), self.typ.s(), F!("{} {:?}", &self.typ, &self.name))
    }

    pub fn unexpect<T>(&self) -> Result<T, MyError> {
        let (t, n, l, s) = (&self.typ, &self.name, &self.line, &self.scope);
        let typ = yesno!(n.is_empty(), F!("{:?}", t), t.s());
        let name = yesno!(n.is_empty(), "".s(), F!(" {n:?}"));
        let inscope = yesno!(s.is_empty(), "".s(), F!(" (in {s})"));
        ERR!("Unexpected {typ}{name}{inscope} at line {l}")
    }
    pub fn unexpect_fields(&self) -> Result<(), MyError> {
        for f in &self.fields {
            return f.unexpect();
        }
        return Ok(());
    }
    pub fn unexpect_frames(&self) -> Result<(), MyError> {
        for f in &self.frames {
            return f.unexpect();
        }
        return Ok(());
    }
    pub fn unexpect_blocks(&self) -> Result<(), MyError> {
        for f in &self.blocks {
            return f.unexpect();
        }
        return Ok(());
    }
    pub fn to_array<T: FromMdlFieldArray>(&self, name: &str) -> Result<T, MyError> {
        self.unexpect_blocks()?;
        self.unexpect_frames()?;
        return Ok(self.fields.to::<T>(name)?);
    }
    pub fn to_array_n<T: FromMdlFieldArray>(&self, name: &str, n: u32) -> Result<T, MyError> {
        let a = self.to_array::<T>(name)?;
        let (scope, line) = (self.subscope(), &self.line);
        let item_name = yesno!(name.is_empty(), yesno!(n > 1, "items", "item"), name);
        match a.len() == n {
            true => Ok(a),
            _ => ERR!("Expecting {n} {item_name} for {scope} at line {line}, got {}", a.len()),
        }
    }
}

// endregion
// region: MdlField

#[derive(Dbg, Default)]
pub struct MdlField {
    pub name: String,
    pub scope: String,
    pub line: u32,
    #[dbg(fmt = "{:?}")]
    pub value: MdlValue, // option
}

impl MdlField {
    pub fn from(pair: Pair<'_, Rule>, scope: &str) -> Result<Self, MyError> {
        let mut this = Build! {scope: scope.s(), line: pair.lineno()};
        this.value.line = this.line;
        this.value.scope = this.scope.s();
        let inner = pair.into_inner();
        let mut first_ident = true;
        for p in inner {
            match p.as_rule() {
                Rule::identifier | Rule::tankey if first_ident => {
                    this.name = p.as_str().s();
                    this.value.name = this.name.s();
                    first_ident = false;
                },
                _value => this.value = MdlValue::from(p, &this.name, scope)?,
            }
        }
        return Ok(this);
    }

    pub fn unexpect<T>(&self) -> Result<T, MyError> {
        let name = yesno!(self.name.is_empty(), &self.value.raw, &self.name);
        ERR!("Unexpected {:?} (in {}) at line {}", name, self.scope, self.line)
    }
    pub fn expect_flag<T>(&self, v: T) -> Result<T, MyError> {
        yesno!(!self.name.is_empty() && self.value.is_empty(), Ok(v), self.value.unexpect())
    }
}

impl Formatter for MdlField {
    fn fmt(&self) -> String {
        F!("{} = {:?}", self.name, &self.value)
    }
}

impl Formatter for Vec<MdlField> {
    fn fmt(&self) -> String {
        F!("{:#?}", self.iter().map(|x| Formatter::fmt(x)).collect::<Vec<_>>())
    }
}

// endregion
// region: MdlFrame

#[derive(Dbg, Default)]
pub struct MdlFrame {
    pub scope: String,
    pub line: u32,
    pub frame: i32,
    #[dbg(fmt = "{:?}")]
    pub value: MdlValue,
    #[dbg(fmt = "{:?}")]
    pub intan: MdlValue,
    #[dbg(fmt = "{:?}")]
    pub outan: MdlValue,
}

impl MdlFrame {
    pub fn from(pair: Pair<'_, Rule>, scope: &str) -> Result<Self, MyError> {
        let mut this = Build! { scope:scope.s(), line:pair.lineno() };
        let mut inner = pair.into_inner();
        let fstr = inner.next().unwrap().as_str();
        this.frame = fstr.parse().unwrap();
        this.value = MdlValue::from(inner.next().unwrap(), fstr, &this.scope)?;
        this.intan = Build!(MdlValue, name:"InTan".s(),  scope:scope.s(), line:this.value.line);
        this.outan = Build!(MdlValue, name:"OutTan".s(), scope:scope.s(), line:this.value.line);
        for p in inner {
            let f = MdlField::from(p, &this.scope)?;
            match_istr!(f.name.as_str(),
                "InTan" => this.intan = f.value,
                "OutTan"=> this.outan = f.value,
                _other => f.unexpect()?,
            );
        }
        return Ok(this);
    }

    pub fn unexpect<T>(&self) -> Result<T, MyError> {
        ERR!("Unexpected '{}:' (in {}) at line {}", self.frame, self.scope, self.line)
    }
}

// endregion
// region: MdlValue

#[derive(Debug, Default, PartialEq)]
pub enum MdlValueType {
    #[default]
    None,
    Integer(i32),
    Float(f32),
    String(String),
    Flag(String),
    IntegerArray(Vec<i32>),
    FloatArray(Vec<f32>),
    FlagArray(Vec<String>),
}

#[derive(Debug, Default)]
pub struct MdlValue {
    pub name: String,
    pub scope: String,
    pub raw: String,
    pub typ: MdlValueType,
    pub line: u32,
}

impl Display for MdlValue {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{}", self.raw)
    }
}

impl MdlValue {
    pub fn from(p: Pair<'_, Rule>, name: &str, scope: &str) -> Result<Self, MyError> {
        let raw = p.as_str();
        let mut this = Build! {name:name.s(), scope:scope.s(), line: p.lineno(), raw:raw.s()};
        this.typ = match p.as_rule() {
            Rule::integer => MdlValueType::Integer(raw.parse()?),
            Rule::float => MdlValueType::Float(raw.parse()?),
            Rule::identifier => MdlValueType::Flag(raw.s()),
            Rule::string => MdlValueType::String(Self::unwrap_string(raw)),
            Rule::identifier_array => MdlValueType::FlagArray(
                p.into_inner().into_iter().map(|p| p.as_str().s()).collect(),
            ),
            Rule::number_array => {
                let inner = p.into_inner();
                let mut fv = Vec::<f32>::with_capacity(inner.len());
                let mut iv = Vec::<i32>::with_capacity(fv.capacity());
                for p in inner {
                    let s = p.as_str();
                    if p.as_rule() == Rule::float {
                        fv.push(s.parse()?);
                    } else {
                        // 19: number of digits in i64:MAX
                        let i = if s.len() < 19 {
                            s.parse::<i64>()?
                        } else {
                            yesno!(s.starts_with('-'), i64::MIN, i64::MAX)
                        };
                        if i >= i32::MIN as i64 && i <= i32::MAX as i64 {
                            iv.push(i as i32);
                        }
                        fv.push(s.parse()?);
                    }
                }
                yesno!(
                    iv.len() == fv.len(),
                    MdlValueType::IntegerArray(iv),
                    MdlValueType::FloatArray(fv)
                )
            },
            _impossible => MdlValueType::default(),
        };
        return Ok(this);
    }

    pub fn to<T: FromMdlValue>(&self) -> Result<T, MyError> {
        T::from(&self)
    }
    pub fn to_ivec(&self, n: u32) -> Result<Vec<i32>, MyError> {
        let v = self.to::<Vec<i32>>()?;
        if v.len() as u32 == n {
            Ok(v)
        } else {
            let want = F!("{n} integer{}", yesno!(n > 1, "s", ""));
            self.expect_but(&want, v.len().s().as_str())
        }
    }
    pub fn to_fvec(&self, n: u32) -> Result<Vec<f32>, MyError> {
        let v = self.to::<Vec<f32>>()?;
        if v.len() as u32 == n {
            Ok(v)
        } else {
            let want = F!("{n} number{}", yesno!(n > 1, "s", ""));
            self.expect_but(&want, v.len().s().as_str())
        }
    }

    pub fn unwrap_string(s: &str) -> String {
        let s = &s[1..s.len() - 1]; // remove quotes
        s.unescape()
    }

    #[allow(unused)]
    pub fn as_str(&self) -> &str {
        match &self.typ {
            MdlValueType::String(s) => s,
            MdlValueType::Flag(s) => s,
            _ => "",
        }
    }

    pub fn is_empty(&self) -> bool {
        self.typ == MdlValueType::None
    }

    pub fn expect<T>(&self, s: &str) -> Result<T, MyError> {
        let forname = yesno!(self.name.is_empty(), "".s(), F!(" for {:?}", self.name));
        let inscope = yesno!(self.scope.is_empty(), "".s(), F!(" (in {})", self.scope));
        let gottype = yesno!(self.is_empty(), "".s(), F!(", got {:?}", self.typ));
        ERR!("Expecting {s}{forname}{inscope} at line {}{gottype}", self.line)
    }
    pub fn expect_but<T>(&self, s: &str, got: &str) -> Result<T, MyError> {
        let forname = yesno!(self.name.is_empty(), "".s(), F!(" for {:?}", self.name));
        let inscope = yesno!(self.scope.is_empty(), "".s(), F!(" (in {})", self.scope));
        ERR!("Expecting {s}{forname}{inscope} at line {}, got {got}", self.line)
    }
    pub fn unexpect<T>(&self) -> Result<T, MyError> {
        let forname = yesno!(self.name.is_empty(), "".s(), F!(" for {:?}", self.name));
        let inscope = yesno!(self.scope.is_empty(), "".s(), F!(" (in {})", self.scope));
        ERR!("Unexpected {:?}{forname}{inscope} at line {}", self.raw, self.line)
    }
}

// endregion
// region: trait: FromMdlValue

pub trait FromMdlValue: Sized {
    fn from(v: &MdlValue) -> Result<Self, MyError>;
}

macro_rules! impl_FromMdlValue_int {
    ($($ty:ty),*) => {
        $(
            impl FromMdlValue for $ty {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    match &v.typ {
                        MdlValueType::Integer(i) => Ok(*i as $ty),
                        MdlValueType::Flag(f) if f.eq_icase("None") || f.eq_icase("Multiple") => Ok(-1),
                        _ => v.expect("an integer"),
                    }
                }
            }
            impl FromMdlValue for Vec<$ty> {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    if let MdlValueType::IntegerArray(iv) = &v.typ {
                        Ok(iv.convert(|v| *v as $ty))
                    } else {
                        v.expect("integer array")
                    }
                }
            }
        )*
    };
}
macro_rules! impl_FromMdlValue_uint {
    ($($ty:ty),*) => {
        $(
            impl FromMdlValue for $ty {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    if let MdlValueType::Integer(i) = &v.typ {
                        Ok(yesno!(*i < 0, 0, *i as $ty))
                    } else {
                        v.expect("an integer")
                    }
                }
            }
            impl FromMdlValue for Vec<$ty> {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    if let MdlValueType::IntegerArray(iv) = &v.typ {
                        Ok(iv.convert(|v| yesno!(*v < 0, 0, *v as $ty)))
                    } else {
                        v.expect("integer array")
                    }
                }
            }
        )*
    };
}
macro_rules! impl_FromMdlValue_float {
    ($($ty:ty),*) => {
        $(
            impl FromMdlValue for $ty {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    match &v.typ {
                        MdlValueType::Float(f) => Ok(*f),
                        MdlValueType::Integer(i) => Ok(*i as $ty),
                        _ => v.expect("a number"),
                    }
                }
            }
            impl FromMdlValue for Vec<$ty> {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    match &v.typ {
                        MdlValueType::FloatArray(fv) => Ok(fv.clone()),
                        MdlValueType::IntegerArray(iv) => Ok(iv.convert(|v| *v as $ty)),
                        _ => v.expect("number array"),
                    }
                }
            }
        )*
    };
}
macro_rules! impl_FromMdlValue_vecN {
    ($($a:tt),*) => {
        $(paste! {
            impl FromMdlValue for [<Vec $a>] {
                fn from(v: &MdlValue) -> Result<Self, MyError> {
                    const LEN: usize = $a;
                    let vs = match &v.typ {
                        MdlValueType::FloatArray(fv) => fv.to_vec(),
                        MdlValueType::IntegerArray(iv) => iv.convert(|v| *v as f32),
                        _ => vec![],
                    };
                    if vs.len() == LEN {
                        Ok(Self::from_slice(vs.as_slice()))
                    } else {
                        v.expect(&F!("{LEN} numbers"))
                    }
                }
            }
        })*
    };
}

impl_FromMdlValue_int!(i8, i16, i32);
impl_FromMdlValue_uint!(u8, u16, u32);
impl_FromMdlValue_float!(f32);
impl_FromMdlValue_vecN!(2, 3, 4);

impl FromMdlValue for String {
    fn from(v: &MdlValue) -> Result<Self, MyError> {
        match &v.typ {
            MdlValueType::String(s) => Ok(s.clone()),
            MdlValueType::Flag(s) => Ok(s.clone()),
            _ => v.expect("string"),
        }
    }
}
impl FromMdlValue for Vec<String> {
    fn from(v: &MdlValue) -> Result<Self, MyError> {
        if let MdlValueType::FlagArray(sv) = &v.typ {
            Ok(sv.clone())
        } else {
            v.expect("flag array")
        }
    }
}

pub trait _ExtendMdlFieldArray {
    fn to<T: FromMdlFieldArray>(&self, name: &str) -> Result<T, MyError>;
}
impl _ExtendMdlFieldArray for Vec<MdlField> {
    fn to<T: FromMdlFieldArray>(&self, name: &str) -> Result<T, MyError> {
        T::from(&self, name)
    }
}

// endregion
// region: trait: FromMdlFieldArray

pub trait FromMdlFieldArray: Sized {
    fn from(v: &Vec<MdlField>, name: &str) -> Result<Self, MyError>;
    fn len(&self) -> u32;
}

macro_rules! impl_FromMdlFieldArray {
    ($($ty:ty),*) => {
        $(impl FromMdlFieldArray for Vec<$ty> {
            fn from(v: &Vec<MdlField>, name: &str) -> Result<Self, MyError> {
                v.try_convert(|f| yesno!(f.name.eq_icase(name), FromMdlValue::from(&f.value), f.unexpect()))
            }
            fn len(&self) -> u32 {
                self.len() as u32
            }
        })*
    };
}

impl_FromMdlFieldArray!(i8, u8, i16, u16, i32, u32, f32, Vec2, Vec3, Vec4);

// endregion
