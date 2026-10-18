---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/app.h"
ref: HEAD
---

#pragma once

#include <d3d11.h>
#include <wrl/client.h>

using Microsoft::WRL::ComPtr;

/// Direct3D 11 device and swap-chain state owned by the application window.
struct DX11State {
    ComPtr<ID3D11Device>            device;          ///< D3D11 logical device
    ComPtr<ID3D11DeviceContext>     deviceContext;   ///< Immediate device context
    ComPtr<IDXGISwapChain>          swapChain;       ///< DXGI swap chain tied to the window
    ComPtr<ID3D11RenderTargetView>  renderTargetView; ///< Back-buffer render target view
};

/**
 * @brief  Create the D3D11 device, device context, and swap chain for a window.
 * @param  hWnd   Window handle that the swap chain presents to.
 * @param  state  Output state struct; all members are populated on success.
 * @return true on success; false if both hardware and WARP drivers fail.
 */
bool create_device_d3d(HWND hWnd, DX11State* state);

/**
 * @brief  Release the render-target view, swap chain, device context, and device.
 * @param  state  State struct whose COM objects are reset to null.
 */
void cleanup_device_d3d(DX11State* state);

/**
 * @brief  Create (or re-create) the render-target view from the swap-chain back buffer.
 * @param  state  Must contain a valid device and swap chain.
 */
void create_render_target(DX11State* state);

/**
 * @brief  Release the render-target view so the swap chain can be resized.
 * @param  state  State struct whose renderTargetView is reset to null.
 */
void cleanup_render_target(DX11State* state);
