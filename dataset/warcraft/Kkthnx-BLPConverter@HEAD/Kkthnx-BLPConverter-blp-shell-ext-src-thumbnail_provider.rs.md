---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/thumbnail_provider.rs"
ref: HEAD
---

use std::sync::{Arc, Mutex};

use windows_implement::implement;

use crate::decode::decode_blp_rgba;
use crate::utils::{create_hbitmap_bgra, resize_fit_rgba, rgba_to_bgra};
use crate::{ProviderState, DLL_LOCK_COUNT};

use windows::Win32::Foundation::{E_FAIL, E_POINTER};
use windows::Win32::System::Com::CoTaskMemFree;
use windows::Win32::Graphics::Gdi::HBITMAP;
use windows::Win32::System::Com::{ISequentialStream, IStream, STREAM_SEEK_SET};
use windows::Win32::UI::Shell::PropertiesSystem::{
    IInitializeWithFile_Impl, IInitializeWithStream_Impl,
};
use windows::Win32::UI::Shell::{
    IInitializeWithItem_Impl, IShellItem, IThumbnailProvider_Impl, SIGDN_FILESYSPATH,
    WTS_ALPHATYPE, WTSAT_ARGB,
};
use windows_core::{Interface, PCWSTR, PWSTR};

const MAX_STREAM_BYTES: usize = 64 * 1024 * 1024;

#[implement(
    windows::Win32::UI::Shell::IThumbnailProvider,
    windows::Win32::UI::Shell::IInitializeWithItem,
    windows::Win32::UI::Shell::PropertiesSystem::IInitializeWithStream,
    windows::Win32::UI::Shell::PropertiesSystem::IInitializeWithFile
)]
pub struct BlpThumbProvider {
    state: Mutex<ProviderState>,
}

impl BlpThumbProvider {
    pub fn new() -> Self {
        DLL_LOCK_COUNT.fetch_add(1, std::sync::atomic::Ordering::SeqCst);
        Self {
            state: Mutex::new(ProviderState::default()),
        }
    }

    fn lock_state(&self) -> windows::core::Result<std::sync::MutexGuard<'_, ProviderState>> {
        self.state
            .lock()
            .map_err(|_| windows::core::Error::from(E_FAIL))
    }
}

impl Drop for BlpThumbProvider {
    fn drop(&mut self) {
        DLL_LOCK_COUNT.fetch_sub(1, std::sync::atomic::Ordering::SeqCst);
    }
}

impl IInitializeWithItem_Impl for BlpThumbProvider_Impl {
    fn Initialize(
        &self,
        psi: windows::core::Ref<'_, IShellItem>,
        _grf_mode: u32,
    ) -> windows::core::Result<()> {
        crate::catch_com(|| {
            unsafe {
                let item = psi.ok()?;
                let pw: PWSTR = item.GetDisplayName(SIGDN_FILESYSPATH)?;
                if pw.is_null() {
                    return Err(windows::core::Error::from(E_FAIL));
                }
                let path = widestring::U16CStr::from_ptr_str(pw.0).to_string_lossy();
                CoTaskMemFree(Some(pw.0 as *const _));
                let mut state = self.lock_state()?;
                state.path_utf8 = Some(path);
                state.stream_data = None;
            }
            Ok(())
        })
    }
}

impl IInitializeWithFile_Impl for BlpThumbProvider_Impl {
    fn Initialize(
        &self,
        psz_file_path: &PCWSTR,
        _grf_mode: u32,
    ) -> windows::core::Result<()> {
        crate::catch_com(|| {
            if psz_file_path.is_null() || psz_file_path.0.is_null() {
                return Err(windows::core::Error::from(E_FAIL));
            }

            let path = unsafe {
                widestring::U16CStr::from_ptr_str(psz_file_path.0).to_string_lossy()
            };
            let mut state = self.lock_state()?;
            state.path_utf8 = Some(path);
            state.stream_data = None;
            Ok(())
        })
    }
}

impl IInitializeWithStream_Impl for BlpThumbProvider_Impl {
    fn Initialize(
        &self,
        pstream: windows::core::Ref<'_, IStream>,
        _grf_mode: u32,
    ) -> windows::core::Result<()> {
        use windows::Win32::Foundation::S_FALSE;

        crate::catch_com(|| {
            let stream = pstream.ok()?;
            unsafe {
                stream.Seek(0, STREAM_SEEK_SET, None)?;
            }

            let mut data = Vec::new();
            let seq: ISequentialStream = stream.cast()?;
            let mut buf = [0u8; 8192];

            loop {
                let mut read = 0u32;
                let hr = unsafe {
                    seq.Read(
                        buf.as_mut_ptr() as *mut _,
                        buf.len() as u32,
                        Some(&mut read),
                    )
                };

                if hr.is_err() {
                    return Err(windows::core::Error::from(hr));
                }

                if read > 0 {
                    if data.len() + read as usize > MAX_STREAM_BYTES {
                        return Err(windows::core::Error::from(E_FAIL));
                    }
                    data.extend_from_slice(&buf[..read as usize]);
                }

                if hr == S_FALSE || read == 0 {
                    break;
                }
            }

            if data.is_empty() {
                return Err(windows::core::Error::from(E_FAIL));
            }

            let mut state = self.lock_state()?;
            state.path_utf8 = None;
            state.stream_data = Some(Arc::from(data));
            Ok(())
        })
    }
}

impl IThumbnailProvider_Impl for BlpThumbProvider_Impl {
    fn GetThumbnail(
        &self,
        cx: u32,
        phbmp: *mut HBITMAP,
        pdwalpha: *mut WTS_ALPHATYPE,
    ) -> windows::core::Result<()> {
        crate::catch_com(|| {
            if phbmp.is_null() || pdwalpha.is_null() {
                return Err(windows::core::Error::from(E_POINTER));
            }

            let (data_arc, path_opt) = {
                let state = self.lock_state()?;
                (state.stream_data.clone(), state.path_utf8.clone())
            };

            let data: Arc<[u8]> = if let Some(buffer) = data_arc {
                buffer
            } else {
                let path = path_opt.ok_or_else(|| windows::core::Error::from(E_FAIL))?;
                Arc::from(
                    std::fs::read(&path)
                        .map_err(|_| windows::core::Error::from(E_FAIL))?,
                )
            };

            let (width, height, rgba) =
                decode_blp_rgba(&data).map_err(|_| windows::core::Error::from(E_FAIL))?;

            let (target_w, target_h, rgba_fit) = {
                let max_dim = if cx > 0 { cx } else { 256 };
                if width.max(height) > max_dim {
                    resize_fit_rgba(&rgba, width, height, max_dim)
                } else {
                    (width, height, rgba)
                }
            };

            let bgra = rgba_to_bgra(&rgba_fit);
            let hbmp = unsafe {
                create_hbitmap_bgra(target_w as i32, target_h as i32, &bgra)?
            };

            unsafe {
                *phbmp = hbmp;
                *pdwalpha = WTSAT_ARGB;
            }

            Ok(())
        })
    }
}
