---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/thumbnail_cache.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <string>
#include <unordered_map>

#include "image_texture.h"

class BlpApi;

/// Decoded thumbnail state for one file path.
struct ThumbnailEntry {
    ImageTexture texture;      ///< Uploaded GPU texture; valid only when loaded && !failed.
    bool     loaded   = false; ///< true once a load attempt has finished (success or failure).
    bool     failed   = false; ///< true when the load attempt produced no usable pixels.
    int      srcW     = 0;     ///< Source image width in pixels.
    int      srcH     = 0;     ///< Source image height in pixels.
    uint64_t fileSize = 0;     ///< Source file size in bytes.
    std::string extLabel;      ///< Uppercase extension shown as placeholder on failure.
};

/// LRU-free cache of decoded thumbnails keyed by absolute UTF-8 file path.
struct ThumbnailCache {
    std::unordered_map<std::string, ThumbnailEntry> entries;
    int generatedThumbPx = 0; ///< Pixel size at which entries were generated (DPI guard).

    /**
     * @brief  Look up an existing entry without triggering a load.
     * @param  path  Absolute UTF-8 file path.
     * @return Pointer to the entry, or nullptr if not yet cached.
     */
    ThumbnailEntry* find(const std::string& path);

    /**
     * @brief  Synchronously decode, downscale, and upload a thumbnail; budget per frame.
     * @param  device   D3D11 device used for texture upload.
     * @param  blpApi   Codec context for BLP decoding.
     * @param  path     Absolute UTF-8 file path.
     * @param  thumbPx  Target thumbnail size in pixels (width == height).
     * @return Reference to the (possibly failed) cache entry.
     */
    ThumbnailEntry& load_now(ID3D11Device* device, BlpApi* blpApi,
                             const std::string& path, int thumbPx);

    /**
     * @brief  Release and remove one entry from the cache.
     * @param  path  Absolute UTF-8 file path to evict.
     */
    void invalidate(const std::string& path);

    /** @brief Release all textures and clear the cache. */
    void clear();
};
