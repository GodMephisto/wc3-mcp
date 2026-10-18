---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/image_view.cpp"
ref: HEAD
---

#include "image_view.h"

#include <algorithm>
#include <cmath>

namespace {

constexpr float ZOOM_MIN         = 0.05f;
constexpr float ZOOM_MAX         = 20.0f;
constexpr float ZOOM_STEP_FACTOR = 1.15f;
constexpr float CHECKER_TILE_SIZE = 10.0f;

void draw_checkerboard(ImDrawList* drawList, const ImVec2& minPos, const ImVec2& maxPos, float cellSize) {
    if (!drawList || cellSize <= 1.0f) return;
    if (maxPos.x <= minPos.x || maxPos.y <= minPos.y) return;

    constexpr ImU32 light = IM_COL32(236, 240, 246, 255);
    constexpr ImU32 dark  = IM_COL32(208, 215, 226, 255);

    int row = 0;
    for (float y = minPos.y; y < maxPos.y; y += cellSize, ++row) {
        int col = 0;
        const float y2 = std::min(y + cellSize, maxPos.y);
        for (float x = minPos.x; x < maxPos.x; x += cellSize, ++col) {
            const float x2      = std::min(x + cellSize, maxPos.x);
            const ImU32 color   = ((row + col) & 1) ? dark : light;
            drawList->AddRectFilled(ImVec2(x, y), ImVec2(x2, y2), color);
        }
    }
}

} // namespace

void ImageViewer::init(ID3D11Device* device) {
    checkerboard = ImageTexture::create_checkerboard(device);
}

void ImageViewer::set_image(ID3D11Device* device, const uint8_t* rgba, int w, int h) {
    texture.release();
    texture.create_from_rgba(device, rgba, w, h);
    imageWidth  = w;
    imageHeight = h;
    hasImage    = true;
    zoom        = 1.0f;
    panX        = 0.0f;
    panY        = 0.0f;
    fitMode     = false;
}

void ImageViewer::clear_image() {
    texture.release();
    hasImage    = false;
    imageWidth  = 0;
    imageHeight = 0;
    zoom        = 1.0f;
    panX        = 0.0f;
    panY        = 0.0f;
    fitMode     = true;
}

void ImageViewer::fit_to_view(float viewW, float viewH) {
    if (!hasImage || imageWidth <= 0 || imageHeight <= 0) return;
    const float scaleX = viewW / static_cast<float>(imageWidth);
    const float scaleY = viewH / static_cast<float>(imageHeight);
    zoom    = std::min(scaleX, scaleY);
    panX    = 0.0f;
    panY    = 0.0f;
    fitMode = true;
}

void ImageViewer::set_zoom(float value, float /*pivotX*/, float /*pivotY*/) {
    zoom    = std::clamp(value, ZOOM_MIN, ZOOM_MAX);
    fitMode = false;
}

void ImageViewer::render(float viewW, float viewH) {
    if (!hasImage || !texture.is_valid()) {
        const ImVec2 textSize  = ImGui::CalcTextSize("拖放图片到这里预览");
        const ImVec2 cursorPos = ImGui::GetCursorScreenPos();
        const float  textX     = cursorPos.x + (viewW - textSize.x) * 0.5f;
        const float  textY     = cursorPos.y + (viewH - textSize.y) * 0.5f;
        ImGui::GetWindowDrawList()->AddText(
            ImVec2(textX, textY),
            IM_COL32(90, 96, 110, 255),
            "拖放图片到这里预览");
        return;
    }

    const float displayW = static_cast<float>(imageWidth)  * zoom;
    const float displayH = static_cast<float>(imageHeight) * zoom;
    const float offsetX  = (viewW - displayW) * 0.5f + panX;
    const float offsetY  = (viewH - displayH) * 0.5f + panY;

    const ImVec2 cursorPos = ImGui::GetCursorScreenPos();
    ImDrawList*  drawList  = ImGui::GetWindowDrawList();

    const ImVec2 regionMin = cursorPos;
    const ImVec2 regionMax = ImVec2(cursorPos.x + viewW, cursorPos.y + viewH);
    const ImVec2 bgMin     = ImVec2(cursorPos.x + offsetX, cursorPos.y + offsetY);
    const ImVec2 bgMax     = ImVec2(bgMin.x + displayW, bgMin.y + displayH);

    if (useCheckerboard) {
        const ImVec2 clipMin(std::max(bgMin.x, regionMin.x), std::max(bgMin.y, regionMin.y));
        const ImVec2 clipMax(std::min(bgMax.x, regionMax.x), std::min(bgMax.y, regionMax.y));
        draw_checkerboard(drawList, clipMin, clipMax, CHECKER_TILE_SIZE);
    } else {
        drawList->AddRectFilled(bgMin, bgMax,
            IM_COL32(static_cast<int>(bgColor[0] * 255),
                     static_cast<int>(bgColor[1] * 255),
                     static_cast<int>(bgColor[2] * 255), 255));
    }

    drawList->AddImage(reinterpret_cast<ImTextureID>(texture.srv.Get()), bgMin, bgMax);

    ImGui::InvisibleButton("##imageview", ImVec2(viewW, viewH));

    if (ImGui::IsItemHovered()) {
        const float wheel = ImGui::GetIO().MouseWheel;
        if (wheel != 0.0f) {
            const float factor  = wheel > 0.0f ? ZOOM_STEP_FACTOR : (1.0f / ZOOM_STEP_FACTOR);
            const float oldZoom = zoom;
            zoom    = std::clamp(zoom * factor, ZOOM_MIN, ZOOM_MAX);
            fitMode = false;

            const ImVec2 mousePos = ImGui::GetIO().MousePos;
            const float  relX     = mousePos.x - cursorPos.x - viewW * 0.5f - panX;
            const float  relY     = mousePos.y - cursorPos.y - viewH * 0.5f - panY;
            const float  scale    = zoom / oldZoom;
            panX -= relX * (scale - 1.0f);
            panY -= relY * (scale - 1.0f);
        }
    }

    if (ImGui::IsItemActive() && ImGui::IsMouseDragging(ImGuiMouseButton_Left)) {
        const ImVec2 delta = ImGui::GetIO().MouseDelta;
        panX   += delta.x;
        panY   += delta.y;
        fitMode = false;
    }
}

void ImageViewer::release() {
    texture.release();
    checkerboard.release();
}
