---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/lib.rs"
ref: HEAD
---

mod class_factory;
mod decode;
mod dll_export;
mod registry;
mod thumbnail_provider;
mod utils;

pub use registry::*;

use std::sync::atomic::AtomicU32;
use windows_core::HRESULT;

pub(crate) const CLASS_E_CLASSNOTAVAILABLE: HRESULT = HRESULT(0x80040111u32 as i32);
pub(crate) const E_FAIL: HRESULT = HRESULT(0x80004005u32 as i32);

/// Run a COM method body, turning any panic into an error instead of letting it
/// unwind across the FFI boundary into Explorer. AssertUnwindSafe is fine here:
/// a panic while the state mutex is held just poisons it, and every lock site
/// already maps a poisoned lock to an error.
pub(crate) fn catch_com<T>(
    f: impl FnOnce() -> windows_core::Result<T>,
) -> windows_core::Result<T> {
    std::panic::catch_unwind(std::panic::AssertUnwindSafe(f))
        .unwrap_or_else(|_| Err(windows_core::Error::from(E_FAIL)))
}

/// Same guard for the raw exported entry points that return an HRESULT directly.
pub(crate) fn catch_hr(f: impl FnOnce() -> HRESULT) -> HRESULT {
    std::panic::catch_unwind(std::panic::AssertUnwindSafe(f)).unwrap_or(E_FAIL)
}

pub(crate) static DLL_LOCK_COUNT: AtomicU32 = AtomicU32::new(0);

#[derive(Default)]
pub(crate) struct ProviderState {
    pub path_utf8: Option<String>,
    pub stream_data: Option<std::sync::Arc<[u8]>>,
}
