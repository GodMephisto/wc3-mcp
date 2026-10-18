---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/thumbnail_grid.h"
ref: HEAD
---

#pragma once

struct AppState;

/**
 * @brief  Render the thumbnail-grid view of all files in the resource list.
 * @param  state  Application state; thumbCache is populated lazily each frame.
 */
void render_thumbnail_grid(AppState& state);
