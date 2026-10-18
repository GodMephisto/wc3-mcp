---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/dll_export.rs"
ref: HEAD
---

use std::ffi::c_void;
use std::ptr::null_mut;
use std::sync::atomic::Ordering;

use windows::Win32::Foundation::{E_NOINTERFACE, E_POINTER, S_FALSE, S_OK};
use windows::Win32::System::Com::IClassFactory;
use windows_core::{GUID, HRESULT, IUnknown, Interface};

use crate::class_factory::BlpClassFactory;
use crate::{CLSID_BLP_THUMB, CLASS_E_CLASSNOTAVAILABLE, DLL_LOCK_COUNT};

#[no_mangle]
#[allow(non_snake_case)]
pub extern "system" fn DllGetClassObject(
    rclsid: *const GUID,
    riid: *const GUID,
    ppv: *mut *mut c_void,
) -> HRESULT {
    crate::catch_hr(|| {
        if ppv.is_null() || rclsid.is_null() || riid.is_null() {
            return E_POINTER;
        }

        unsafe {
            *ppv = null_mut();
        }

        let requested = unsafe { *rclsid };
        if requested != CLSID_BLP_THUMB {
            return CLASS_E_CLASSNOTAVAILABLE;
        }

        let factory = BlpClassFactory::new();
        let class_factory: IClassFactory = factory.into();
        let requested_iid = unsafe { *riid };

        if requested_iid == IClassFactory::IID || requested_iid == IUnknown::IID {
            unsafe {
                *ppv = class_factory.into_raw();
            }
            S_OK
        } else {
            E_NOINTERFACE
        }
    })
}

#[no_mangle]
#[allow(non_snake_case)]
pub extern "system" fn DllCanUnloadNow() -> HRESULT {
    if DLL_LOCK_COUNT.load(Ordering::SeqCst) == 0 {
        S_OK
    } else {
        S_FALSE
    }
}
