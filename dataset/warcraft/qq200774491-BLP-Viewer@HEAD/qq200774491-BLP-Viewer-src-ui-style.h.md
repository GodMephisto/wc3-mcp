---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/style.h"
ref: HEAD
---

#pragma once

#include "imgui.h"

/**
 * @brief  Apply the application's custom Dear ImGui style, scaled for DPI.
 * @param  dpiScale  Physical-to-logical pixel ratio (1.0 on 96-DPI displays).
 */
void apply_imgui_style(float dpiScale);

/**
 * @brief  Load application fonts at the given DPI scale; rebuilds the font atlas.
 * @param  dpiScale  Physical-to-logical pixel ratio (1.0 on 96-DPI displays).
 */
void load_fonts(float dpiScale);

/** @brief Returns the success highlight colour (green). */
ImVec4 ui_color_success();
/** @brief Returns the warning highlight colour (orange). */
ImVec4 ui_color_warning();
/** @brief Returns the error highlight colour (red). */
ImVec4 ui_color_error();

/** @brief Push the primary (blue) button style onto the ImGui colour stack. */
void push_primary_button_style();
/** @brief Push the secondary (grey) button style onto the ImGui colour stack. */
void push_secondary_button_style();
/** @brief Push the danger (red) button style onto the ImGui colour stack. */
void push_danger_button_style();
/** @brief Pop three colour entries pushed by push_*_button_style(). */
void pop_button_style();
