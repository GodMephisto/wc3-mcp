---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_left_panel.h"
ref: HEAD
---

#pragma once

struct AppState;

/**
 * @brief  Render the left panel: resource list, batch-convert, and compose tabs.
 * @param  state  Application state.
 */
void render_left_panel(AppState& state);
