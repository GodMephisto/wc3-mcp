---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/app.cpp"
ref: HEAD
---

#include "app.h"

#include <dxgi.h>

namespace {

constexpr UINT SWAP_CHAIN_BUFFER_COUNT  = 2;
constexpr UINT REFRESH_RATE_NUMERATOR   = 60;
constexpr UINT REFRESH_RATE_DENOMINATOR = 1;

} // namespace

bool create_device_d3d(HWND hWnd, DX11State* state) {
    DXGI_SWAP_CHAIN_DESC sd               = {};
    sd.BufferCount                        = SWAP_CHAIN_BUFFER_COUNT;
    sd.BufferDesc.Width                   = 0;
    sd.BufferDesc.Height                  = 0;
    sd.BufferDesc.Format                  = DXGI_FORMAT_R8G8B8A8_UNORM;
    sd.BufferDesc.RefreshRate.Numerator   = REFRESH_RATE_NUMERATOR;
    sd.BufferDesc.RefreshRate.Denominator = REFRESH_RATE_DENOMINATOR;
    sd.Flags                              = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
    sd.BufferUsage                        = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    sd.OutputWindow                       = hWnd;
    sd.SampleDesc.Count                   = 1;
    sd.SampleDesc.Quality                 = 0;
    sd.Windowed                           = TRUE;
    sd.SwapEffect                         = DXGI_SWAP_EFFECT_DISCARD;

    constexpr UINT createDeviceFlags = 0;
    D3D_FEATURE_LEVEL featureLevel;
    constexpr D3D_FEATURE_LEVEL featureLevelArray[2] = {
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_0,
    };

    HRESULT hr = D3D11CreateDeviceAndSwapChain(
        nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, createDeviceFlags,
        featureLevelArray, 2, D3D11_SDK_VERSION, &sd,
        state->swapChain.ReleaseAndGetAddressOf(),
        state->device.ReleaseAndGetAddressOf(),
        &featureLevel,
        state->deviceContext.ReleaseAndGetAddressOf());

    if (hr == DXGI_ERROR_UNSUPPORTED) {
        hr = D3D11CreateDeviceAndSwapChain(
            nullptr, D3D_DRIVER_TYPE_WARP, nullptr, createDeviceFlags,
            featureLevelArray, 2, D3D11_SDK_VERSION, &sd,
            state->swapChain.ReleaseAndGetAddressOf(),
            state->device.ReleaseAndGetAddressOf(),
            &featureLevel,
            state->deviceContext.ReleaseAndGetAddressOf());
    }

    if (FAILED(hr)) return false;

    create_render_target(state);
    return true;
}

void cleanup_device_d3d(DX11State* state) {
    cleanup_render_target(state);
    state->swapChain.Reset();
    state->deviceContext.Reset();
    state->device.Reset();
}

void create_render_target(DX11State* state) {
    ComPtr<ID3D11Texture2D> backBuffer;
    state->swapChain->GetBuffer(0, IID_PPV_ARGS(backBuffer.GetAddressOf()));
    if (backBuffer) {
        state->device->CreateRenderTargetView(
            backBuffer.Get(), nullptr,
            state->renderTargetView.ReleaseAndGetAddressOf());
    }
}

void cleanup_render_target(DX11State* state) {
    state->renderTargetView.Reset();
}
