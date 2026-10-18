---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/extends/vector.rs"
ref: HEAD
---

use crate::*;

// region: trait: ExtendVector

pub trait _ExtendVector {
    fn to_or_string(&self) -> String;
}

impl<T: Display> _ExtendVector for Vec<T> {
    fn to_or_string(&self) -> String {
        self.as_slice().to_or_string()
    }
}

impl<T: Display> _ExtendVector for [T] {
    fn to_or_string(&self) -> String {
        match self.len() {
            0 => String::new(),
            1 => self[0].s(),
            2 => F!("{} or {}", self[0], self[1]),
            len => {
                let left = self[..len - 1].convert(|a| a.s()).join(", ");
                let right = &self[len - 1];
                F!("{} or {}", left, right)
            },
        }
    }
}

// endregion
// region: trait: ExtendVectorReverse

pub trait _ExtendVectorReverse {
    fn reverse(&self) -> Self;
}

impl<T: Clone + Copy> _ExtendVectorReverse for Vec<T> {
    fn reverse(&self) -> Self {
        let mut v: Self = Self::with_capacity(self.len());
        for i in 0..self.len() {
            v.push(self[self.len() - i - 1]);
        }
        return v;
    }
}

impl _ExtendVectorReverse for Vec2 {
    fn reverse(&self) -> Self {
        Self::new(self.y, self.x)
    }
}

impl _ExtendVectorReverse for Vec3 {
    fn reverse(&self) -> Self {
        Self::new(self.z, self.y, self.x)
    }
}

impl _ExtendVectorReverse for Vec4 {
    fn reverse(&self) -> Self {
        Self::new(self.w, self.z, self.y, self.x)
    }
}

// endregion
// region: convert

pub trait _ConvertVec<A> {
    fn convert<B, F>(&self, f: F) -> Vec<B>
    where
        F: Fn(&A) -> B;
    fn try_convert<B, F>(&self, f: F) -> Result<Vec<B>, MyError>
    where
        F: Fn(&A) -> Result<B, MyError>;
}

impl<A> _ConvertVec<A> for Vec<A> {
    fn convert<B, F>(&self, f: F) -> Vec<B>
    where
        F: Fn(&A) -> B,
    {
        self.into_iter().map(f).collect()
    }

    fn try_convert<B, F>(&self, f: F) -> Result<Vec<B>, MyError>
    where
        F: Fn(&A) -> Result<B, MyError>,
    {
        let mut ret = Vec::with_capacity(self.len());
        for a in self.iter() {
            ret.push(f(a)?);
        }
        return Ok(ret);
    }
}

impl<A> _ConvertVec<A> for [A] {
    fn convert<B, F>(&self, f: F) -> Vec<B>
    where
        F: Fn(&A) -> B,
    {
        self.into_iter().map(f).collect()
    }

    fn try_convert<B, F>(&self, f: F) -> Result<Vec<B>, MyError>
    where
        F: Fn(&A) -> Result<B, MyError>,
    {
        let mut ret = Vec::with_capacity(self.len());
        for a in self.iter() {
            ret.push(f(a)?);
        }
        return Ok(ret);
    }
}

// endregion
