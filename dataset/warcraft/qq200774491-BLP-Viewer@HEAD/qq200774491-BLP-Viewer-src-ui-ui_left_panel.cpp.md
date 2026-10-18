---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_left_panel.cpp"
ref: HEAD
---

#include "ui_left_panel.h"
#include "ui_main.h"
#include "batch_convert.h"
#include "ui_compose_tab.h"
#include "file_dialogs.h"
#include "style.h"
#include "utils.h"
#include "imgui.h"

#include <algorithm>
#include <cctype>
#include <cstdio>
#include <cstring>
#include <filesystem>

namespace {

std::string to_lower_ascii(std::string text) {
    std::transform(text.begin(), text.end(), text.begin(),
                   [](unsigned char c) { return static_cast<char>(std::tolower(c)); });
    return text;
}

} // namespace

void render_left_panel(AppState& state) {
    ImGui::PushStyleColor(ImGuiCol_ChildBg, ImVec4(1, 1, 1, 1));
    ImGui::TextUnformatted("资源中心");
    ImGui::Separator();

    if (ImGui::BeginTabBar("LeftWorkspaceTabs", ImGuiTabBarFlags_FittingPolicyResizeDown)) {
        if (ImGui::BeginTabItem("资源列表")) {
            static char searchBuf[128] = {};

            ImGui::PushStyleColor(ImGuiCol_Text, ImVec4(0.357f, 0.392f, 0.447f, 1.0f));
            ImGui::TextWrapped("拖拽、按钮添加或从菜单栏导入。列表用于预览与批量转换。");
            ImGui::PopStyleColor();

            ImGui::PushItemWidth(-1);
            ImGui::InputTextWithHint("##FileSearch", "搜索文件名...", searchBuf, sizeof(searchBuf));
            ImGui::PopItemWidth();

            const float gap   = ImGui::GetStyle().ItemSpacing.x;
            const float halfW = (ImGui::GetContentRegionAvail().x - gap) * 0.5f;

            if (ImGui::Button("添加文件...", ImVec2(halfW, 0))) {
                auto files = open_file_dialog(state.hwnd, L"*.png;*.jpg;*.jpeg;*.bmp;*.tga;*.blp;*.dds", true);
                std::vector<std::string> paths;
                for (const auto& f : files)
                    paths.push_back(fs_path_to_utf8(std::filesystem::path(f)));
                add_files(state, paths);
            }
            ImGui::SameLine();
            if (ImGui::Button("添加文件夹...", ImVec2(-1, 0))) {
                auto folder = open_folder_dialog(state.hwnd);
                if (!folder.empty()) {
                    const std::string folderUtf8 = fs_path_to_utf8(std::filesystem::path(folder));
                    std::strncpy(state.inputDirBuf, folderUtf8.c_str(), sizeof(state.inputDirBuf) - 1);
                    add_folder_files(state, folderUtf8, state.recursive);
                }
            }
            if (ImGui::Button("移除选中", ImVec2(halfW, 0))) {
                if (state.selectedFileIndex >= 0 &&
                    state.selectedFileIndex < static_cast<int>(state.fileList.size())) {
                    const std::string path = state.fileList[state.selectedFileIndex];
                    const bool removedCurrent = (path == state.currentPreviewPath);
                    state.thumbCache.invalidate(path);
                    state.fileSet.erase(path);
                    state.relativePathMap.erase(path);
                    state.fileList.erase(state.fileList.begin() + state.selectedFileIndex);
                    if (state.fileList.empty()) {
                        state.selectedFileIndex = -1;
                        state.imageViewer.clear_image();
                        state.currentPreviewPath.clear();
                        state.currentBlpBytes.clear();
                        state.previewOriginalRGBA.clear();
                        state.previewAdjustedRGBA.clear();
                        state.currentMeta = ImageMeta();
                    } else if (state.selectedFileIndex >= static_cast<int>(state.fileList.size())) {
                        state.selectedFileIndex = static_cast<int>(state.fileList.size()) - 1;
                        if (removedCurrent) state.currentPreviewPath.clear();
                    } else if (removedCurrent) {
                        state.currentPreviewPath.clear();
                    }
                }
            }
            ImGui::SameLine();
            if (ImGui::Button("清空列表", ImVec2(-1, 0))) {
                state.thumbCache.clear();
                state.fileList.clear();
                state.fileSet.clear();
                state.relativePathMap.clear();
                state.selectedFileIndex = -1;
                state.imageViewer.clear_image();
                state.currentPreviewPath.clear();
                state.currentBlpBytes.clear();
                state.previewOriginalRGBA.clear();
                state.previewAdjustedRGBA.clear();
                state.currentMeta = ImageMeta();
            }

            ImGui::Separator();
            draw_output_format_selector(state);
            ImGui::PushStyleColor(ImGuiCol_Text, ImVec4(0.357f, 0.392f, 0.447f, 1.0f));
            ImGui::TextWrapped("用于预览图“保存尺寸/对齐”的保存格式，与批量转换共用此设置。");
            ImGui::PopStyleColor();
            ImGui::Separator();

            const std::string keyword = to_lower_ascii(searchBuf);
            float listH = ImGui::GetContentRegionAvail().y - 28.0f * state.dpiScale;
            if (listH < 120.0f * state.dpiScale) listH = 120.0f * state.dpiScale;
            ImGui::BeginChild("FileList", ImVec2(-1, listH), ImGuiChildFlags_Borders);
            for (int i = 0; i < static_cast<int>(state.fileList.size()); ++i) {
                const std::string displayName =
                    fs_path_to_utf8(fs_path_from_utf8(state.fileList[i]).filename());
                if (!keyword.empty()) {
                    const std::string nameLower = to_lower_ascii(displayName);
                    const std::string pathLower = to_lower_ascii(state.fileList[i]);
                    if (nameLower.find(keyword) == std::string::npos &&
                        pathLower.find(keyword) == std::string::npos) {
                        continue;
                    }
                }
                const bool isSelected = (i == state.selectedFileIndex);
                if (ImGui::Selectable(displayName.c_str(), isSelected)) {
                    state.selectedFileIndex = i;
                    state.rightViewMode = 1;
                }
                if (ImGui::IsItemHovered()) {
                    ImGui::SetTooltip("%s", state.fileList[i].c_str());
                }
            }
            ImGui::EndChild();

            char brief[96];
            std::snprintf(brief, sizeof(brief), "总文件数：%zu | 当前选中：%d",
                          state.fileList.size(),
                          state.selectedFileIndex >= 0 ? state.selectedFileIndex + 1 : 0);
            ImGui::TextUnformatted(brief);

            ImGui::EndTabItem();
        }

        if (ImGui::BeginTabItem("批量转换")) {
            render_batch_convert_tab(state);
            ImGui::EndTabItem();
        }

        if (ImGui::BeginTabItem("图层合成")) {
            render_compose_tab(state);
            ImGui::EndTabItem();
        }

        ImGui::EndTabBar();
    }

    ImGui::PopStyleColor(); // ChildBg
}
