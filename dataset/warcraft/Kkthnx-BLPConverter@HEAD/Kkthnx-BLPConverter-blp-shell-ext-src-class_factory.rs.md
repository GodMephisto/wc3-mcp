---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/class_factory.rs"
ref: HEAD
---

use std::ffi::c_void;
use std::ptr::null_mut;
use std::sync::atomic::Ordering;

use windows::Win32::Foundation::{E_NOINTERFACE, E_POINTER};
use windows::Win32::System::Com::IClassFactory_Impl;
use windows::Win32::UI::Shell::PropertiesSystem::{IInitializeWithFile, IInitializeWithStream};
use windows::Win32::UI::Shell::{IInitializeWithItem, IThumbnailProvider};
use windows_core::{BOOL, GUID, IUnknown, Interface};
use windows_implement::implement;

use crate::DLL_LOCK_COUNT;
use crate::thumbnail_provider::BlpThumbProvider;

#[implement(windows::Win32::System::Com::IClassFactory)]
pub struct BlpClassFactory;

impl BlpClassFactory {
    pub fn new() -> Self {
        Self
    }
}

impl IClassFactory_Impl for BlpClassFactory_Impl {
    fn CreateInstance(
        &self,
        _outer: windows::core::Ref<'_, IUnknown>,
        riid: *const GUID,
        ppv: *mut *mut c_void,
    ) -> windows::core::Result<()> {
        crate::catch_com(|| {
        if ppv.is_null() || riid.is_null() {
            return Err(windows::core::Error::from(E_POINTER));
        }

        unsafe {
            *ppv = null_mut();
        }

        let provider: IUnknown = BlpThumbProvider::new().into();
        let requested = unsafe { *riid };

        unsafe {
            if requested == IThumbnailProvider::IID {
                *ppv = provider.cast::<IThumbnailProvider>()?.into_raw();
                return Ok(());
            }
            if requested == IInitializeWithItem::IID {
                *ppv = provider.cast::<IInitializeWithItem>()?.into_raw();
                return Ok(());
            }
            if requested == IInitializeWithStream::IID {
                *ppv = provider.cast::<IInitializeWithStream>()?.into_raw();
                return Ok(());
            }
            if requested == IInitializeWithFile::IID {
                *ppv = provider.cast::<IInitializeWithFile>()?.into_raw();
                return Ok(());
            }
            if requested == IUnknown::IID {
                *ppv = provider.into_raw();
                return Ok(());
            }
        }

        Err(windows::core::Error::from(E_NOINTERFACE))
        })
    }

    fn LockServer(&self, f_lock: BOOL) -> windows::core::Result<()> {
        if f_lock.as_bool() {
            DLL_LOCK_COUNT.fetch_add(1, Ordering::SeqCst);
        } else {
            DLL_LOCK_COUNT.fetch_sub(1, Ordering::SeqCst);
        }
        Ok(())
    }
}
