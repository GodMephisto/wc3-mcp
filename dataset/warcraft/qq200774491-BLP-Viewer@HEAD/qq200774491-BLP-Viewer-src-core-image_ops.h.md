---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_ops.h"
ref: HEAD
---

#pragma once

#include <cstdint>

#include "image_io.h"  // RgbaImage

// How a source image is fit into a fixed target size.
enum class ResizeMethod {
    Stretch,            // bilinear stretch to fill target (distorts aspect)
    CenterTransparent,  // fit inside target (scale down if larger) + center, alpha=0 padding
};

// 9-anchor placement grid for overlay compositing.
enum class Anchor9 {
    TopLeft, TopCenter, TopRight,
    MidLeft, Center, MidRight,
    BottomLeft, BottomCenter, BottomRight,
};

/**
 * @brief  Produce a target-sized RGBA8 image from a raw pixel block.
 * @param  srcPixels  Pointer to the source RGBA8 pixel data; must not be null.
 * @param  srcW       Source width in pixels; must be > 0.
 * @param  srcH       Source height in pixels; must be > 0.
 * @param  targetW    Output width in pixels; must be > 0.
 * @param  targetH    Output height in pixels; must be > 0.
 * @param  method     Stretch: bilinear resize to (targetW, targetH).
 *                    CenterTransparent: scale down only if src exceeds the target,
 *                    preserving aspect ratio, then center; padding is alpha=0.
 * @return Filled RgbaImage on success; empty image (width == 0) for degenerate inputs.
 */
RgbaImage transform_to_target(
    const uint8_t* srcPixels,
    int srcW, 
    int srcH,
    int targetW, int targetH,
    ResizeMethod method
);

/**
 * @brief  Src-over alpha composite of `overlay` onto a copy of `base`.
 * @param  base      Background image; returned unchanged when either image is degenerate.
 * @param  overlay   Foreground image; scaled, placed and alpha-blended over base.
 * @param  anchor    One of the nine anchor positions in Anchor9 that determines
 *                   where the overlay's top-left corner is placed.
 * @param  marginPx  Inset in pixels from the chosen edge(s); clamped to >= 0.
 * @param  scalePct  Overlay scale as a percentage of its original size [1..∞]; 100
 *                   means 1:1; clamped to >= 1.
 * @param  opacity   Global opacity multiplier applied to the overlay's alpha [0..1].
 * @return New RgbaImage with the overlay composited; overlay pixels outside base
 *         bounds are silently clipped.
 */
RgbaImage composite_overlay(
    const RgbaImage& base,
    const RgbaImage& overlay,
    Anchor9 anchor, 
    int marginPx, 
    int scalePct, 
    float opacity
);

/**
 * @brief  Return a copy of `src` with a solid-color RGBA border.
 * @param  src          Source image; returned unchanged when degenerate or t == 0.
 * @param  thicknessPx  Border thickness in pixels; clamped to >= 0.
 * @param  r            Border red channel [0..255].
 * @param  g            Border green channel [0..255].
 * @param  b            Border blue channel [0..255].
 * @param  a            Border alpha channel [0..255].
 * @param  expandCanvas false: draw the border inside src's existing bounds (pixels
 *                      at the edge are overwritten).
 *                      true: grow the canvas by thicknessPx on every side and fill
 *                      the new margin with the color; the original is centered.
 * @return New RgbaImage with the border applied.
 */
RgbaImage add_border(
    const RgbaImage& src,
    int thicknessPx,
    uint8_t r,
    uint8_t g, 
    uint8_t b,
    uint8_t a, 
    bool expandCanvas
);
