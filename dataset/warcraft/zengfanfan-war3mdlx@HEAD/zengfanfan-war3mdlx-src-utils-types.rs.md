---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/types.rs"
ref: HEAD
---

use crate::*;

// region: typename

#[macro_export]
macro_rules! TNAMEL {
    () => {
        std::any::type_name::<Self>()
    };
    ($e:expr) => {
        crate::types::tnamel($e)
    };
    ($t:ty) => {
        std::any::type_name::<$t>()
    };
}

#[macro_export]
macro_rules! TNAME {
    () => {
        crate::types::tname_last_seg_trim::<Self>(1)
    };
    ($e:expr) => {
        crate::types::tname($e)
    };
    ($t:ty) => {
        crate::types::tname_last_seg_trim::<$t>(1)
    };
}

#[allow(dead_code)]
pub fn tnamel<T>(_: &T) -> &str {
    std::any::type_name::<T>()
}
#[allow(dead_code)]
pub fn tname<T>(_: &T) -> String {
    tname_last_seg_trim::<T>(1)
}

pub fn tname_last_seg_trim<T>(n: u32) -> String {
    let n = n as usize;
    let full = std::any::type_name::<T>();
    let parts: Vec<&str> = full.split("::").collect();
    let len = parts.len();
    if n >= len { full.to_string() } else { parts[len - n..].join("::") }
}

// endregion
// region: trait: CheckDefault, CheckValue

pub trait _CheckDefault {
    fn isdef(&self) -> bool; //=::default()
}
impl<T: Default + PartialEq> _CheckDefault for T {
    fn isdef(&self) -> bool {
        *self == T::default()
    }
}

#[allow(dead_code)]
pub trait CheckValue {
    fn is0(&self) -> bool; //=0
    fn is1(&self) -> bool; //=1
    fn isneg1(&self) -> bool; //=-1
}

macro_rules! impl_CheckValue {
    ($($t:ty),*) => {
        $(
            impl CheckValue for $t {
                fn is0(&self) -> bool {
                    *self == 0 as Self
                }
                fn is1(&self) -> bool {
                    *self == 1 as Self
                }
                fn isneg1(&self) -> bool {
                    *self == -1i8 as Self
                }
            }
            impl CheckValue for &[$t] {
                fn is0(&self) -> bool {
                    self.iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.iter().all(|x| x.isneg1())
                }
            }
            impl CheckValue for &Vec<$t> {
                fn is0(&self) -> bool {
                    self.iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.iter().all(|x| x.isneg1())
                }
            }
            impl CheckValue for Vec<$t> {
                fn is0(&self) -> bool {
                    self.iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.iter().all(|x| x.isneg1())
                }
            }
        )*
    };
}
macro_rules! impl_CheckValue_vecN {
    ($($t:ty),*) => {
        $(
            impl CheckValue for $t {
                fn is0(&self) -> bool {
                    self.to_array().iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.to_array().iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.to_array().iter().all(|x| x.isneg1())
                }
            }
            impl CheckValue for &[$t] {
                fn is0(&self) -> bool {
                    self.iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.iter().all(|x| x.isneg1())
                }
            }
            impl CheckValue for &Vec<$t> {
                fn is0(&self) -> bool {
                    self.iter().all(|x| x.is0())
                }
                fn is1(&self) -> bool {
                    self.iter().all(|x| x.is1())
                }
                fn isneg1(&self) -> bool {
                    self.iter().all(|x| x.isneg1())
                }
            }
        )*
    };
}

impl_CheckValue!(i8, u8, i16, u16, i32, u32, f32);
impl_CheckValue_vecN!(Vec2, Vec3, Vec4);

impl CheckValue for bool {
    fn is0(&self) -> bool {
        !*self
    }
    fn is1(&self) -> bool {
        *self
    }
    fn isneg1(&self) -> bool {
        false
    }
}
impl CheckValue for String {
    fn is0(&self) -> bool {
        self.is_empty()
    }
    fn is1(&self) -> bool {
        false
    }
    fn isneg1(&self) -> bool {
        false
    }
}
impl CheckValue for str {
    fn is0(&self) -> bool {
        self.is_empty()
    }
    fn is1(&self) -> bool {
        false
    }
    fn isneg1(&self) -> bool {
        false
    }
}

// endregion
