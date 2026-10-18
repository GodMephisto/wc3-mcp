---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_right_panel.h"
ref: HEAD
---

#pragma once

struct AppState;

/**
 * @brief  Render the right panel: thumbnail grid and single-image preview/save toolbar.
 * @param  state  Application state.
 */
void render_right_panel(AppState& state);
