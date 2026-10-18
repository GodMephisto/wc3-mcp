---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/preview_manager.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <string>
#include <vector>

struct AppState;

/**
 * @brief  Resolve the shared output-format selection for the current preview.
 * @param  state  Application state containing the selection and current image metadata.
 * @return Canonical lowercase extension string, e.g. "blp".
 */
std::string single_save_format_str(const AppState& state);

/**
 * @brief  Load and decode a file, populate all preview-related AppState fields.
 * @param  state  Application state; preview fields, imageViewer and mipEntries are updated.
 * @param  path   Absolute UTF-8 path to the file to preview.
 */
void update_preview(AppState& state, const std::string& path);

/**
 * @brief  Re-upload the current preview pixels to the GPU (e.g. after alpha-toggle).
 * @param  state  Application state.
 */
void refresh_preview_display(AppState& state);

/**
 * @brief  Write RGBA pixels to disk aligned with the current source file location.
 * @param  state     Application state; currentPreviewPath, fileList, etc. are updated on success.
 * @param  rgba      Tightly-packed RGBA8 pixels.
 * @param  w         Image width in pixels.
 * @param  h         Image height in pixels.
 * @param  outError  Optional; receives a human-readable error on failure.
 * @return true on success; false on any I/O or encode error.
 */
bool save_aligned_to_source(AppState& state, const std::vector<uint8_t>& rgba,
                             int w, int h, std::string* outError);
