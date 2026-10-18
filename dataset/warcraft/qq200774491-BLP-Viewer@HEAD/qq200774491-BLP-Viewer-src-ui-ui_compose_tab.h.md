---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_compose_tab.h"
ref: HEAD
---

#pragma once

struct AppState;

/**
 * @brief  Render the "图层合成" tab body inside the left-panel TabBar.
 * @param  state  Application state.
 */
void render_compose_tab(AppState& state);

/**
 * @brief  Run batch layer-compose or border operation on all files in the resource list.
 * @param  state  Application state.
 */
void run_compose_batch_from_ui(AppState& state);
