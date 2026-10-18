---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_texture.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <d3d11.h>
#include <wrl/client.h>

using Microsoft::WRL::ComPtr;

/// Default tile size in pixels used when creating a checkerboard texture.
constexpr int DEFAULT_CHECKER_TILE_PX = 16;

/// GPU texture paired with its shader-resource view, ready for ImGui rendering.
struct ImageTexture {
    ComPtr<ID3D11ShaderResourceView> srv; ///< Shader resource view; null when no texture is loaded
    int width  = 0;                       ///< Texture width in pixels
    int height = 0;                       ///< Texture height in pixels

    /**
     * @brief  Upload RGBA pixel data to the GPU and create a shader resource view.
     * @param  device  D3D11 device used for resource creation.
     * @param  rgba    Tightly-packed RGBA8 pixel buffer of size w * h * 4 bytes.
     * @param  w       Image width in pixels; must be > 0.
     * @param  h       Image height in pixels; must be > 0.
     * @return true on success; false if any argument is invalid or D3D11 calls fail.
     */
    bool create_from_rgba(ID3D11Device* device, const uint8_t* rgba, int w, int h);

    /**
     * @brief  Release the GPU texture and reset width/height to zero.
     */
    void release();

    /**
     * @brief  Check whether a valid texture is currently loaded.
     * @return true if srv is non-null; false otherwise.
     */
    bool is_valid() const;

    /**
     * @brief  Create a grey checkerboard texture suitable for transparent-background display.
     * @param  device    D3D11 device used for resource creation.
     * @param  tileSize  Side length of each checker square in pixels (default: DEFAULT_CHECKER_TILE_PX).
     * @return ImageTexture with srv populated; srv is null if device creation fails.
     */
    static ImageTexture create_checkerboard(ID3D11Device* device, int tileSize = DEFAULT_CHECKER_TILE_PX);
};
