---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/macros.rs"
ref: HEAD
---

// region: conditional operator

#[macro_export]
macro_rules! yesno {
    ($cond:expr, $y:expr, $n:expr) => {{ if $cond { $y } else { $n } }};
    ($cond:expr, $y:stmt, $n:stmt) => {{ if $cond { $y } else { $n } }};
    ($var:ident = $expr:expr, $y:expr, $n:expr) => {{ if let $var = $expr { $y } else { $n } }};
    ($var:ident = $expr:expr, $y:stmt, $n:stmt) => {{ if let $var = $expr { $y } else { $n } }};
    ($var:ident = $expr:expr, $cond:expr, $y:stmt, $n:stmt) => {{
        let $var = $expr;
        if $cond { $y } else { $n }
    }};
}

#[macro_export]
macro_rules! yes {
    ($cond:expr, $y:expr) => {{
        if $cond {
            _ = $y;
        }
    }};
    ($cond:expr, $y:stmt) => {{
        if $cond {
            $y
        }
    }};
    ($var:ident = $expr:expr, $cond:expr, $y:expr) => {{
        let $var = $expr;
        if $cond {
            _ = $y;
        }
    }};
    ($var:ident = $expr:expr, $cond:expr, $y:stmt) => {{
        let $var = $expr;
        if $cond {
            $y
        }
    }};
}

#[macro_export]
macro_rules! no {
    ($cond:expr, $y:expr) => {{
        if !$cond {
            _ = $y;
        }
    }};
    ($cond:expr, $y:stmt) => {{
        if !$cond {
            $y
        }
    }};
    ($var:ident = $expr:expr, $cond:expr, $y:stmt) => {{
        let $var = $expr;
        if !$cond {
            $y
        }
    }};
    ($var:ident = $expr:expr, $cond:expr, $y:expr) => {{
        let $var = $expr;
        if !$cond {
            _ = $y;
        }
    }};
}

// endregion
// region: match

#[macro_export]
macro_rules! match_istr {
    ($s:expr, $( $($left:literal)|+ => $right:expr ),+ $(, $_def:ident => $def:expr )? $(,)?) => {
        match $s {
            $(
                s if $(s.eq_ignore_ascii_case($left))||+ => $right,
            )+
            $( $_def => $def )?
        }
    }
}

// endregion
// region: build default struct

#[macro_export]
macro_rules! Build {
    (Self $(, $field:ident : $value:expr )* $(,)?) => {
        Self {
            $($field: $value,)*
            ..Default::default()
        }
    };
    ( $($field:ident : $value:expr),* $(,)? ) => { // omit Self
        Self {
            $( $field: $value, )*
            ..Default::default()
        }
    };
    ($ty:path $(, $field:ident : $value:expr )* $(,)?) => {
        {
            let mut ret = <$ty>::default();
            $(ret.$field = $value;)*
            ret
        }
    };
}

// endregion
// region: EXIT

#[macro_export]
macro_rules! EXIT {
    () => {{ return Ok(()); }};
    ($($arg:tt)*) => {{ log!($($arg)*); EXIT!(); }};
}

#[macro_export]
macro_rules! EXIT1 {
    () => {{ return Ok(()); }};
    ($($arg:tt)*) => {{ return ERR!($($arg)*); }};
}

// endregion
