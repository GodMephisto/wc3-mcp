---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/registry.rs"
ref: HEAD
---

use windows_core::GUID;

/// Shell Thumbnail Provider category GUID.
pub const SHELL_THUMB_HANDLER_CATID: GUID =
    GUID::from_u128(0xE357FCCD_A995_4576_B01F_234630154E96);

/// Shell Preview Handler category GUID.
pub const SHELL_PREVIEW_HANDLER_CATID: GUID =
    GUID::from_u128(0x8895B1C6_B41F_4C1C_A562_0D564250836F);

/// CLSID for the BLPView thumbnail provider COM class.
pub const CLSID_BLP_THUMB: GUID =
    GUID::from_u128(0xA7F3C2E1_5B4D_4E89_9C0A_1B2C3D4E5F60);

pub const DEFAULT_PROGID: &str = "BLPConverter.BLPView";
pub const DEFAULT_EXT: &str = ".blp";
pub const FRIENDLY_NAME: &str = "BLPView Thumbnail Provider";
pub const INSTALL_FOLDER: &str = "BLPConverter\\BLPView";
pub const DLL_FILENAME: &str = "blpview_thumb.dll";

pub trait GuidExt {
    fn to_braced_upper(&self) -> String;
}

impl GuidExt for GUID {
    fn to_braced_upper(&self) -> String {
        format!(
            "{{{:08X}-{:04X}-{:04X}-{:02X}{:02X}-{:02X}{:02X}{:02X}{:02X}{:02X}{:02X}}}",
            self.data1,
            self.data2,
            self.data3,
            self.data4[0],
            self.data4[1],
            self.data4[2],
            self.data4[3],
            self.data4[4],
            self.data4[5],
            self.data4[6],
            self.data4[7],
        )
    }
}
