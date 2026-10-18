---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/image_view.h"
ref: HEAD
---

#pragma once

#include "image_texture.h"
#include "imgui.h"

/// Pan-and-zoom image viewer backed by a D3D11 shader-resource view.
struct ImageViewer {
    ImageTexture texture;                              ///< Currently loaded image texture
    ImageTexture checkerboard;                         ///< Tiled grey checkerboard for transparent backgrounds
    float zoom            = 1.0f;                      ///< Current zoom factor (1.0 = 100 %)
    float panX            = 0.0f;                      ///< Horizontal pan offset in pixels
    float panY            = 0.0f;                      ///< Vertical pan offset in pixels
    bool  fitMode         = true;                      ///< true while the view is in fit-to-window mode
    bool  hasImage        = false;                     ///< true when a valid image is loaded
    int   imageWidth      = 0;                         ///< Loaded image width in pixels
    int   imageHeight     = 0;                         ///< Loaded image height in pixels
    bool  useCheckerboard = true;                      ///< Draw checkerboard behind transparent images
    float bgColor[3]      = {0.878f, 0.894f, 0.922f}; ///< Solid background colour (linear RGB, used when useCheckerboard is false)

    /**
     * @brief  Create the shared checkerboard texture; call once after device creation.
     * @param  device  D3D11 device used for GPU resource creation.
     */
    void init(ID3D11Device* device);

    /**
     * @brief  Upload and display a new RGBA image, resetting pan and zoom to defaults.
     * @param  device  D3D11 device used for texture upload.
     * @param  rgba    Tightly-packed RGBA8 pixel buffer of size w * h * 4 bytes.
     * @param  w       Image width in pixels; must be > 0.
     * @param  h       Image height in pixels; must be > 0.
     */
    void set_image(ID3D11Device* device, const uint8_t* rgba, int w, int h);

    /**
     * @brief  Release the current image texture and return the viewer to its empty state.
     */
    void clear_image();

    /**
     * @brief  Compute zoom so the entire image fits within the given view dimensions.
     * @param  viewW  Available view width in pixels.
     * @param  viewH  Available view height in pixels.
     */
    void fit_to_view(float viewW, float viewH);

    /**
     * @brief  Set an explicit zoom level, clamped to [ZOOM_MIN, ZOOM_MAX].
     * @param  value   Desired zoom factor.
     * @param  pivotX  Normalised horizontal pivot (0–1); reserved for future use.
     * @param  pivotY  Normalised vertical pivot (0–1); reserved for future use.
     */
    void set_zoom(float value, float pivotX = 0.5f, float pivotY = 0.5f);

    /**
     * @brief  Draw the viewer into the current ImGui window and handle mouse input.
     * @param  viewW  Width of the rendering region in pixels.
     * @param  viewH  Height of the rendering region in pixels.
     */
    void render(float viewW, float viewH);

    /**
     * @brief  Release all GPU resources owned by this viewer.
     */
    void release();
};
