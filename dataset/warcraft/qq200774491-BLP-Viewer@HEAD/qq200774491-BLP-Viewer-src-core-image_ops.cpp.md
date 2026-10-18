---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_ops.cpp"
ref: HEAD
---

#include "image_ops.h"

#include <algorithm>
#include <cmath>
#include <cstring>

// stb_image_resize2 is implemented (STB_IMAGE_RESIZE2_IMPLEMENTATION) once in
// ui_right_panel.cpp; here we only declare/use it (external linkage).
#include "stb_image_resize2.h"

namespace {

constexpr int RGBA_CHANNELS = 4;  // bytes per pixel throughout this file

// Bilinear resize src -> dst dimensions, RGBA8.
std::vector<uint8_t> resize_rgba(
    const uint8_t* src,
    int sw,
    int sh, 
    int dw, 
    int dh) { 
        std::vector<uint8_t> dst(static_cast<size_t>(dw) * dh * RGBA_CHANNELS );
        stbir_resize_uint8_linear(
            src, 
            sw, 
            sh,
            sw * RGBA_CHANNELS,
            dst.data(),
            dw, 
            dh,
            dw * RGBA_CHANNELS, 
            STBIR_RGBA
        );
    return dst;
}

// Center-copy a (cw x ch) RGBA block into a transparent (tw x th) canvas.
RgbaImage center_on_transparent(const uint8_t* block, int cw, int ch, int tw, int th) {
    RgbaImage out;
    out.width  = tw;
    out.height = th;
    out.pixels.assign(static_cast<size_t>(tw) * th * RGBA_CHANNELS, 0);  // transparent
    const int offX  = std::max(0, (tw - cw) / 2);
    const int offY  = std::max(0, (th - ch) / 2);
    const int copyW = std::min(cw, tw - offX);
    const int copyH = std::min(ch, th - offY);
    for (int y = 0; y < copyH; ++y) {
        std::memcpy(out.pixels.data() + (static_cast<size_t>(y + offY) * tw + offX) * RGBA_CHANNELS,
                    block + static_cast<size_t>(y) * cw * RGBA_CHANNELS,
                    static_cast<size_t>(copyW) * RGBA_CHANNELS);
    }
    return out;
}

}  // namespace

RgbaImage transform_to_target(const uint8_t* srcPixels, int srcW, int srcH,
                               int targetW, int targetH, ResizeMethod method) {
    RgbaImage out;
    if (!srcPixels || srcW <= 0 || srcH <= 0 || targetW <= 0 || targetH <= 0) {
        return out;  // empty
    }

    if (method == ResizeMethod::Stretch) {
        out.width  = targetW;
        out.height = targetH;
        out.pixels = resize_rgba(srcPixels, srcW, srcH, targetW, targetH);
        return out;
    }

    // CenterTransparent: fit inside the target, scaling down only if too big.
    const double s = std::min(1.0, std::min(static_cast<double>(targetW) / srcW,
                                            static_cast<double>(targetH) / srcH));
    if (s < 1.0) {
        const int scaledW = std::max(1, std::min(targetW, static_cast<int>(std::lround(srcW * s))));
        const int scaledH = std::max(1, std::min(targetH, static_cast<int>(std::lround(srcH * s))));
        std::vector<uint8_t> scaled = resize_rgba(srcPixels, srcW, srcH, scaledW, scaledH);
        return center_on_transparent(scaled.data(), scaledW, scaledH, targetW, targetH);
    }
    return center_on_transparent(srcPixels, srcW, srcH, targetW, targetH);
}

RgbaImage composite_overlay(const RgbaImage& base,const RgbaImage& overlay,Anchor9 anchor,int marginPx, int scalePct, float opacity) {
    RgbaImage out = base;  // copy
    if (base.width <= 0 || base.height <= 0 || overlay.width <= 0 || overlay.height <= 0) {
        return out;
    }
    // Optionally scale the overlay.
    const int pct = std::max(1, scalePct);
    int ow = std::max(1, overlay.width * pct / 100);
    int oh = std::max(1, overlay.height * pct / 100);
    std::vector<uint8_t> scaledStorage;
    const uint8_t* op = overlay.pixels.data();
    if (ow != overlay.width || oh != overlay.height) {
        scaledStorage = resize_rgba(overlay.pixels.data(), overlay.width, overlay.height, ow, oh);
        op = scaledStorage.data();
    }

    const int bw     = base.width;
    const int bh     = base.height;
    const int margin = std::max(0, marginPx);

    // Resolve top-left placement from the 9-anchor grid.
    const int col = static_cast<int>(anchor) % 3;  // 0=left, 1=center, 2=right
    const int row = static_cast<int>(anchor) / 3;  // 0=top,  1=mid,    2=bottom
    int px;
    if (col == 0)       px = margin;
    else if (col == 1)  px = (bw - ow) / 2;
    else                px = bw - ow - margin;
    int py;
    if (row == 0)       py = margin;
    else if (row == 1)  py = (bh - oh) / 2;
    else                py = bh - oh - margin;

    const float op01 = std::clamp(opacity, 0.0f, 1.0f);

    for (int j = 0; j < oh; ++j) {
        const int by = py + j;
        if (by < 0 || by >= bh) continue;
        for (int i = 0; i < ow; ++i) {
            const int bx = px + i;
            if (bx < 0 || bx >= bw) continue;

            const uint8_t* s = op + (static_cast<size_t>(j) * ow + i) * RGBA_CHANNELS;
            const float as = (s[3] / 255.0f) * op01;  // effective source alpha
            if (as <= 0.0f) continue;

            uint8_t* d = out.pixels.data() + (static_cast<size_t>(by) * bw + bx) * RGBA_CHANNELS;
            const float ad   = d[3] / 255.0f;
            const float outA = as + ad * (1.0f - as);
            const float inv  = (outA > 0.0f) ? (1.0f / outA) : 0.0f;
            for (int c = 0; c < 3; ++c) {
                const float v = (s[c] * as + d[c] * ad * (1.0f - as)) * inv;
                d[c] = static_cast<uint8_t>(std::clamp(v + 0.5f, 0.0f, 255.0f));  // +0.5f: round-to-nearest
            }
            d[3] = static_cast<uint8_t>(std::clamp(outA * 255.0f + 0.5f, 0.0f, 255.0f));  // +0.5f: round-to-nearest
        }
    }
    return out;
}

RgbaImage add_border(const RgbaImage& src, int thicknessPx,
                     uint8_t r, uint8_t g, uint8_t b, uint8_t a, bool expandCanvas) {
    const int t = std::max(0, thicknessPx);
    if (src.width <= 0 || src.height <= 0 || t == 0) {
        return src;  // nothing to do
    }

    const uint8_t color[RGBA_CHANNELS] = {r, g, b, a};

    if (expandCanvas) {
        const int nw = src.width  + 2 * t;
        const int nh = src.height + 2 * t;
        RgbaImage out;
        out.width  = nw;
        out.height = nh;
        out.pixels.resize(static_cast<size_t>(nw) * nh * RGBA_CHANNELS);
        // Fill everything with the border color.
        for (size_t p = 0; p < out.pixels.size(); p += RGBA_CHANNELS) {
            std::memcpy(out.pixels.data() + p, color, RGBA_CHANNELS);
        }
        // Copy original into the center.
        for (int y = 0; y < src.height; ++y) {
            std::memcpy(out.pixels.data() + (static_cast<size_t>(y + t) * nw + t) * RGBA_CHANNELS,
                        src.pixels.data() + static_cast<size_t>(y) * src.width * RGBA_CHANNELS,
                        static_cast<size_t>(src.width) * RGBA_CHANNELS);
        }
        return out;
    }

    // Draw inside the existing bounds.
    RgbaImage out = src;
    const int w  = out.width;
    const int h  = out.height;
    const int tt = std::min(t, std::min(w, h));  // clamp so it can't overflow
    auto setPixel = [&](int x, int y) {
        std::memcpy(out.pixels.data() + (static_cast<size_t>(y) * w + x) * RGBA_CHANNELS,
                    color, RGBA_CHANNELS);
    };
    for (int y = 0; y < h; ++y) {
        const bool edgeRow = (y < tt) || (y >= h - tt);
        for (int x = 0; x < w; ++x) {
            if (edgeRow || x < tt || x >= w - tt) {
                setPixel(x, y);
            }
        }
    }
    return out;
}
