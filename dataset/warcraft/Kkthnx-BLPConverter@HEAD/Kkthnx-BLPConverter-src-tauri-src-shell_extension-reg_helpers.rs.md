---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/reg_helpers.rs"
ref: HEAD
---

use std::ffi::OsStr;
use std::io;
use std::path::Path;

use winreg::enums::RegType;
use winreg::types::FromRegValue;
use winreg::{RegKey, RegValue};

pub(crate) fn notify_shell_assoc(_reason: &str) {
    use windows::Win32::UI::Shell::{
        SHChangeNotify, SHCNE_ASSOCCHANGED, SHCNE_UPDATEIMAGE, SHCNF_IDLIST,
    };
    unsafe {
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, None, None);
        SHChangeNotify(SHCNE_UPDATEIMAGE, SHCNF_IDLIST, None, None);
    }
}

pub(crate) struct RegKeyHelper {
    key: RegKey,
}

pub(in crate::shell_extension) enum RegVal {
    Sz(std::ffi::OsString),
    Dword(u32),
}

pub(in crate::shell_extension) trait IntoRegVal {
    fn into_reg_val(self) -> RegVal;
}

impl IntoRegVal for &str {
    fn into_reg_val(self) -> RegVal {
        RegVal::Sz(self.into())
    }
}

impl IntoRegVal for &OsStr {
    fn into_reg_val(self) -> RegVal {
        RegVal::Sz(self.to_os_string())
    }
}

impl IntoRegVal for &Path {
    fn into_reg_val(self) -> RegVal {
        RegVal::Sz(self.as_os_str().to_os_string())
    }
}

impl IntoRegVal for u32 {
    fn into_reg_val(self) -> RegVal {
        RegVal::Dword(self)
    }
}

impl RegKeyHelper {
    pub fn open(root: &RegKey, path: impl AsRef<str>) -> io::Result<Self> {
        let (key, _) = root.create_subkey(path.as_ref())?;
        Ok(Self { key })
    }

    pub fn sub(&self, suffix: &str) -> io::Result<Self> {
        let (key, _) = self.key.create_subkey(suffix)?;
        Ok(Self { key })
    }

    pub fn set<V: IntoRegVal>(&self, name: &str, value: V) -> io::Result<()> {
        match value.into_reg_val() {
            RegVal::Sz(os) => self.key.set_value(name, &os),
            RegVal::Dword(d) => self.key.set_value(name, &d),
        }
    }

    pub fn set_default<V: IntoRegVal>(&self, value: V) -> io::Result<()> {
        self.set("", value)
    }

    #[allow(dead_code)]
    pub fn get<T: FromRegValue>(&self, name: &str) -> io::Result<T> {
        self.key.get_value(name)
    }

    #[allow(dead_code)]
    pub fn set_binary(&self, name: &str, bytes: &[u8]) -> io::Result<()> {
        let value = RegValue {
            vtype: RegType::REG_BINARY,
            bytes: bytes.to_vec(),
        };
        self.key.set_raw_value(name, &value)
    }
}
