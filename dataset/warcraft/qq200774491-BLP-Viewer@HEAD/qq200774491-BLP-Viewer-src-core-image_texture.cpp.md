---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_texture.cpp"
ref: HEAD
---

#include "image_texture.h"

#include <vector>

/// Number of tile repetitions per axis when generating the checkerboard texture.
constexpr int CHECKER_TILE_REPEAT = 16;

bool ImageTexture::create_from_rgba(ID3D11Device* device, const uint8_t* rgba, int w, int h) {
    release();

    if (!device || !rgba || w <= 0 || h <= 0) return false;

    D3D11_TEXTURE2D_DESC desc   = {};
    desc.Width                  = static_cast<UINT>(w);
    desc.Height                 = static_cast<UINT>(h);
    desc.MipLevels              = 1;
    desc.ArraySize              = 1;
    desc.Format                 = DXGI_FORMAT_R8G8B8A8_UNORM;
    desc.SampleDesc.Count       = 1;
    desc.Usage                  = D3D11_USAGE_DEFAULT;
    desc.BindFlags              = D3D11_BIND_SHADER_RESOURCE;

    D3D11_SUBRESOURCE_DATA initData = {};
    initData.pSysMem                = rgba;
    initData.SysMemPitch            = static_cast<UINT>(w * 4);

    ComPtr<ID3D11Texture2D> texture;
    HRESULT hr = device->CreateTexture2D(&desc, &initData, texture.GetAddressOf());
    if (FAILED(hr)) return false;

    D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc    = {};
    srvDesc.Format                             = desc.Format;
    srvDesc.ViewDimension                      = D3D11_SRV_DIMENSION_TEXTURE2D;
    srvDesc.Texture2D.MipLevels                = 1;

    hr = device->CreateShaderResourceView(texture.Get(), &srvDesc, srv.ReleaseAndGetAddressOf());
    if (FAILED(hr)) return false;

    width  = w;
    height = h;
    return true;
}

void ImageTexture::release() {
    srv.Reset();
    width  = 0;
    height = 0;
}

bool ImageTexture::is_valid() const {
    return srv != nullptr;
}

ImageTexture ImageTexture::create_checkerboard(ID3D11Device* device, int tileSize) {
    const int texSize = tileSize * CHECKER_TILE_REPEAT;
    std::vector<uint8_t> pixels(static_cast<size_t>(texSize * texSize * 4));

    constexpr uint8_t lightColor[4] = {224, 228, 235, 255};
    constexpr uint8_t darkColor[4]  = {195, 198, 204, 255};

    for (int y = 0; y < texSize; ++y) {
        for (int x = 0; x < texSize; ++x) {
            const int tileX        = x / tileSize;
            const int tileY        = y / tileSize;
            const bool isDark      = ((tileX + tileY) % 2) == 0;
            const uint8_t* color   = isDark ? darkColor : lightColor;
            const int idx          = (y * texSize + x) * 4;
            pixels[idx + 0]        = color[0];
            pixels[idx + 1]        = color[1];
            pixels[idx + 2]        = color[2];
            pixels[idx + 3]        = color[3];
        }
    }

    ImageTexture tex;
    tex.create_from_rgba(device, pixels.data(), texSize, texSize);
    return tex;
}
