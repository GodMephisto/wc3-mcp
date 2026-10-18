---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/thumbnail_grid.cpp"
ref: HEAD
---

#include "thumbnail_grid.h"
#include "ui_main.h"
#include "thumbnail_cache.h"
#include "utils.h"
#include "imgui.h"

#include <algorithm>
#include <cstdio>

void render_thumbnail_grid(AppState& state) {
    const int thumbPx = static_cast<int>(96.0f * state.dpiScale + 0.5f);
    if (state.thumbCache.generatedThumbPx != thumbPx) {
        state.thumbCache.clear();
        state.thumbCache.generatedThumbPx = thumbPx;
    }

    ImGui::PushStyleColor(ImGuiCol_ChildBg, ImVec4(1, 1, 1, 1));
    ImGui::BeginChild("ThumbGrid", ImVec2(-1, -1), ImGuiChildFlags_Borders);

    if (state.fileList.empty()) {
        ImGui::TextDisabled("列表为空，请从左侧添加文件");
        ImGui::EndChild();
        ImGui::PopStyleColor();
        return;
    }

    const ImGuiStyle& style     = ImGui::GetStyle();
    const float       pad       = 6.0f * state.dpiScale;
    const float       textH     = ImGui::GetTextLineHeightWithSpacing();
    const float       cellW     = thumbPx + pad * 2.0f;
    const float       cellH     = thumbPx + pad * 2.0f + textH;
    const float       availW    = ImGui::GetContentRegionAvail().x;
    const int         columns   = std::max(1,
        static_cast<int>((availW + style.ItemSpacing.x) / (cellW + style.ItemSpacing.x)));
    const int         total     = static_cast<int>(state.fileList.size());
    const int         rows      = (total + columns - 1) / columns;

    int          loadBudget = 3;
    ImDrawList*  drawList   = ImGui::GetWindowDrawList();

    ImGuiListClipper clipper;
    clipper.Begin(rows, cellH + style.ItemSpacing.y);
    while (clipper.Step()) {
        for (int row = clipper.DisplayStart; row < clipper.DisplayEnd; ++row) {
            for (int col = 0; col < columns; ++col) {
                const int i = row * columns + col;
                if (i >= total) break;
                if (col > 0) ImGui::SameLine();

                const std::string& path = state.fileList[i];
                ImGui::PushID(i);

                const bool selected = (i == state.selectedFileIndex);
                if (ImGui::Selectable("##cell", selected,
                                      ImGuiSelectableFlags_AllowDoubleClick,
                                      ImVec2(cellW, cellH))) {
                    state.selectedFileIndex = i;
                    if (ImGui::IsMouseDoubleClicked(ImGuiMouseButton_Left)) {
                        state.rightViewMode = 1;
                    }
                }

                ThumbnailEntry* entry = state.thumbCache.find(path);
                if ((!entry || !entry->loaded) && loadBudget > 0) {
                    entry = &state.thumbCache.load_now(state.device, &state.blpApi, path, thumbPx);
                    --loadBudget;
                }

                if (ImGui::IsItemHovered()) {
                    if (entry && entry->loaded && !entry->failed) {
                        ImGui::SetTooltip("%s\n%d x %d | %s", path.c_str(),
                                          entry->srcW, entry->srcH,
                                          format_file_size(entry->fileSize).c_str());
                    } else {
                        ImGui::SetTooltip("%s", path.c_str());
                    }
                }

                const ImVec2 p0       = ImGui::GetItemRectMin();
                const ImVec2 thumbMin(p0.x + pad, p0.y + pad);
                const ImVec2 thumbMax(thumbMin.x + thumbPx, thumbMin.y + thumbPx);

                drawList->AddRectFilled(thumbMin, thumbMax,
                                        IM_COL32(244, 246, 250, 255), 3.0f * state.dpiScale);

                if (entry && entry->loaded && !entry->failed && entry->texture.is_valid()) {
                    drawList->AddImage(reinterpret_cast<ImTextureID>(entry->texture.srv.Get()),
                                       thumbMin, thumbMax);
                } else {
                    const char* label = "...";
                    if (entry && entry->loaded) {
                        label = entry->extLabel.empty() ? "?" : entry->extLabel.c_str();
                    }
                    const ImVec2 ts = ImGui::CalcTextSize(label);
                    drawList->AddText(ImVec2(thumbMin.x + (thumbPx - ts.x) * 0.5f,
                                             thumbMin.y + (thumbPx - ts.y) * 0.5f),
                                      IM_COL32(140, 150, 165, 255), label);
                }

                const std::string name     = fs_path_to_utf8(fs_path_from_utf8(path).filename());
                const ImVec2      nameSize = ImGui::CalcTextSize(name.c_str());
                const float       textX   = p0.x + std::max(pad * 0.5f, (cellW - nameSize.x) * 0.5f);
                const float       textY   = thumbMax.y + pad * 0.5f;
                const ImVec4      clipRect(p0.x, p0.y, p0.x + cellW, p0.y + cellH);
                drawList->AddText(nullptr, 0.0f, ImVec2(textX, textY),
                                  IM_COL32(60, 66, 77, 255), name.c_str(), nullptr,
                                  0.0f, &clipRect);

                ImGui::PopID();
            }
        }
    }

    ImGui::EndChild();
    ImGui::PopStyleColor();
}
