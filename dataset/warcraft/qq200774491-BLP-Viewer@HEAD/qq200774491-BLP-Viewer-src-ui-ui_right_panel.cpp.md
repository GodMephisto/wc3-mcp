---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_right_panel.cpp"
ref: HEAD
---

#include "ui_right_panel.h"
#include "ui_main.h"
#include "preview_manager.h"
#include "thumbnail_grid.h"
#include "style.h"
#include "utils.h"
#include "image_ops.h"

#include <algorithm>
#include <cstdio>
#include <cstring>
#include <filesystem>
#include <fstream>

#include "stb_image_resize2.h"

namespace {

bool write_file_bytes(const std::string& path, const std::vector<uint8_t>& bytes,
                      std::string* outError) {
    std::ofstream file(fs_path_from_utf8(path), std::ios::binary | std::ios::trunc);
    if (!file.is_open()) {
        if (outError) *outError = "写入文件失败";
        return false;
    }
    file.write(reinterpret_cast<const char*>(bytes.data()),
               static_cast<std::streamsize>(bytes.size()));
    if (!file.good()) {
        if (outError) *outError = "写入文件不完整";
        return false;
    }
    return true;
}

void log_msg(AppState& state, const std::string& msg) {
    state.logMessages.push_back(msg);
}

} // namespace

void render_right_panel(AppState& state) {
    static int         lastSelectedIndex = -2;
    static std::string lastSelectedPath;
    std::string        selectedPath;
    if (state.selectedFileIndex >= 0 &&
        state.selectedFileIndex < static_cast<int>(state.fileList.size())) {
        selectedPath = state.fileList[state.selectedFileIndex];
    }

    if (state.selectedFileIndex != lastSelectedIndex || selectedPath != lastSelectedPath) {
        lastSelectedIndex = state.selectedFileIndex;
        lastSelectedPath  = selectedPath;
        if (state.selectedFileIndex >= 0 &&
            state.selectedFileIndex < static_cast<int>(state.fileList.size())) {
            update_preview(state, selectedPath);
        } else {
            state.imageViewer.clear_image();
            state.currentPreviewPath.clear();
            state.currentMeta = ImageMeta();
        }
    }

    {
        auto viewModeBtn = [&](const char* label, int mode) {
            if (state.rightViewMode == mode) push_primary_button_style();
            else push_secondary_button_style();
            const bool clicked = ImGui::Button(label);
            pop_button_style();
            if (clicked) state.rightViewMode = mode;
        };
        viewModeBtn("网格", 0);
        ImGui::SameLine();
        viewModeBtn("预览", 1);
        ImGui::SameLine();
        if (state.rightViewMode == 0) {
            ImGui::TextDisabled("共 %d 张 · 双击缩略图进入预览",
                                static_cast<int>(state.fileList.size()));
        } else if (!state.currentPreviewPath.empty()) {
            const std::string name =
                fs_path_to_utf8(fs_path_from_utf8(state.currentPreviewPath).filename());
            ImGui::TextDisabled("%s", name.c_str());
        } else {
            ImGui::TextDisabled("未加载图像");
        }
        ImGui::Separator();
    }

    if (state.rightViewMode == 0) {
        render_thumbnail_grid(state);
        return;
    }

    const bool hasImage = state.currentMeta.width > 0;
    const bool isPot    = is_power_of_two(state.currentMeta.width) &&
                          is_power_of_two(state.currentMeta.height);

    auto getActiveSource = [&](const std::vector<uint8_t>** outPixels, int* outW, int* outH) -> bool {
        if (!outPixels || !outW || !outH) return false;
        if (state.previewAdjusted && !state.previewAdjustedRGBA.empty()) {
            *outPixels = &state.previewAdjustedRGBA;
            *outW      = state.previewAdjW;
            *outH      = state.previewAdjH;
        } else {
            *outPixels = &state.previewOriginalRGBA;
            *outW      = state.previewOrigW;
            *outH      = state.previewOrigH;
        }
        return *outPixels && !(*outPixels)->empty() && *outW > 0 && *outH > 0;
    };

    auto resizeAndSave = [&](int targetW, int targetH, const char* successPrefix) {
        const std::vector<uint8_t>* source = nullptr;
        int srcW = 0, srcH = 0;
        if (!getActiveSource(&source, &srcW, &srcH)) return;

        const bool sizeUnchanged  = (srcW == targetW && srcH == targetH);
        const bool formatChanged  = (state.currentMeta.format != single_save_format_str(state));
        if (sizeUnchanged && !formatChanged) {
            log_msg(state, "尺寸与输出格式均未变化，无需保存");
            return;
        }

        std::string error;
        bool saved = false;
        if (sizeUnchanged) {
            saved = save_aligned_to_source(state, *source, srcW, srcH, &error);
        } else {
            std::vector<uint8_t> resized(static_cast<size_t>(targetW) * targetH * 4);
            stbir_resize_uint8_linear(source->data(), srcW, srcH, srcW * 4,
                                      resized.data(), targetW, targetH, targetW * 4, STBIR_RGBA);
            saved = save_aligned_to_source(state, resized, targetW, targetH, &error);
        }
        if (saved) {
            char msg[128];
            std::snprintf(msg, sizeof(msg), "%s：%d x %d", successPrefix, targetW, targetH);
            log_msg(state, msg);
        } else {
            log_msg(state, "保存失败：" + error);
        }
    };

    {
        const float remainingH = ImGui::GetContentRegionAvail().y;
        const float previewH   = std::max(120.0f * state.dpiScale, remainingH);

        ImGui::PushStyleColor(
            ImGuiCol_ChildBg,
            ImVec4(state.previewCanvasBg[0], state.previewCanvasBg[1], state.previewCanvasBg[2], 1.0f));
        ImGui::BeginChild("PreviewArea", ImVec2(-1, previewH), ImGuiChildFlags_Borders);

        if (hasImage) {
            char info[256];
            std::snprintf(info, sizeof(info), "%d x %d 像素 | %s | %s",
                          state.currentMeta.width, state.currentMeta.height,
                          format_file_size(state.currentMeta.fileSize).c_str(),
                          state.currentMeta.format.c_str());
            std::string infoStr = info;
            if (state.currentIsBlp && state.currentMipIndex >= 0) {
                char mipBuf[32];
                std::snprintf(mipBuf, sizeof(mipBuf), " | 层级 %d", state.currentMipIndex + 1);
                infoStr += mipBuf;
            }
            if (!isPot) infoStr += " | 非 2 次幂";

            if (!isPot) ImGui::PushStyleColor(ImGuiCol_Text, ui_color_warning());
            ImGui::TextUnformatted(infoStr.c_str());
            if (!isPot) ImGui::PopStyleColor();

            if (state.currentIsBlp) {
                if (ImGui::Checkbox("显示透明通道", &state.previewUseAlpha)) {
                    refresh_preview_display(state);
                }
                ImGui::SameLine();
            }
            if (ImGui::Button("背景颜色")) {
                ImGui::OpenPopup("BgColorPicker");
            }
            if (ImGui::BeginPopup("BgColorPicker")) {
                ImGui::ColorPicker3("##bgpicker", state.previewCanvasBg);
                if (ImGui::Button("恢复默认")) {
                    state.previewCanvasBg[0] = 0.878f;
                    state.previewCanvasBg[1] = 0.894f;
                    state.previewCanvasBg[2] = 0.922f;
                    ImGui::CloseCurrentPopup();
                }
                ImGui::EndPopup();
            }
            ImGui::SameLine();
            if (ImGui::Button("适应窗口")) {
                state.imageViewer.fitMode = true;
            }
            ImGui::SameLine();
            if (ImGui::Button("原始大小")) {
                state.imageViewer.set_zoom(1.0f);
                state.imageViewer.fitMode = false;
            }
            ImGui::SameLine();
            char zoomText[32];
            std::snprintf(zoomText, sizeof(zoomText), "缩放：%d%%",
                          std::max(1, static_cast<int>(state.imageViewer.zoom * 100.0f + 0.5f)));
            ImGui::TextUnformatted(zoomText);

            auto resolveTargetDims = [&](int* tw, int* th) {
                switch (state.targetSizeMode) {
                    case 1: *tw = 64;  *th = 64;  break;
                    case 2: *tw = 128; *th = 128; break;
                    default: *tw = std::max(1, state.resizeWidth);
                             *th = std::max(1, state.resizeHeight); break;
                }
            };
            auto saveWithMethod = [&](ResizeMethod method, const char* okMsg) {
                const std::vector<uint8_t>* src = nullptr;
                int sw = 0, sh = 0;
                if (!getActiveSource(&src, &sw, &sh)) return;
                int tw = 0, th = 0;
                resolveTargetDims(&tw, &th);
                RgbaImage r = transform_to_target(src->data(), sw, sh, tw, th, method);
                if (r.width <= 0) { log_msg(state, "尺寸处理失败"); return; }
                std::string err;
                if (save_aligned_to_source(state, r.pixels, r.width, r.height, &err)) {
                    char msg[128];
                    std::snprintf(msg, sizeof(msg), "%s：%d x %d", okMsg, r.width, r.height);
                    log_msg(state, msg);
                } else {
                    log_msg(state, std::string("保存失败：") + err);
                }
            };

            ImGui::SameLine();
            push_primary_button_style();
            if (ImGui::Button("拉伸尺寸保存"))
                saveWithMethod(ResizeMethod::Stretch, "已拉伸尺寸保存");
            pop_button_style();
            ImGui::SameLine();
            push_primary_button_style();
            if (ImGui::Button("居中透明保存"))
                saveWithMethod(ResizeMethod::CenterTransparent, "已居中透明保存");
            pop_button_style();

            const bool originalIsPot = is_power_of_two(state.previewOrigW) &&
                                       is_power_of_two(state.previewOrigH);
            const bool canAlign = !originalIsPot && !state.previewAdjusted;
            ImGui::SameLine();
            if (!canAlign) ImGui::BeginDisabled();
            if (ImGui::Button("拉伸2次幂")) {
                const int targetW = nearest_power_of_two(state.previewOrigW);
                const int targetH = nearest_power_of_two(state.previewOrigH);
                if (targetW != state.previewOrigW || targetH != state.previewOrigH) {
                    std::vector<uint8_t> resized(static_cast<size_t>(targetW) * targetH * 4);
                    stbir_resize_uint8_linear(
                        state.previewOriginalRGBA.data(), state.previewOrigW, state.previewOrigH,
                        state.previewOrigW * 4, resized.data(), targetW, targetH, targetW * 4,
                        STBIR_RGBA);
                    std::string error;
                    if (save_aligned_to_source(state, resized, targetW, targetH, &error)) {
                        log_msg(state, "已拉伸对齐 2 次幂并保存");
                    } else {
                        log_msg(state, "对齐失败：" + error);
                    }
                }
            }
            ImGui::SameLine();
            if (ImGui::Button("居中2次幂")) {
                const int targetW = next_power_of_two(state.previewOrigW);
                const int targetH = next_power_of_two(state.previewOrigH);
                if (targetW != state.previewOrigW || targetH != state.previewOrigH) {
                    std::vector<uint8_t> centered(static_cast<size_t>(targetW) * targetH * 4, 0);
                    const int offX = (targetW - state.previewOrigW) / 2;
                    const int offY = (targetH - state.previewOrigH) / 2;
                    for (int y = 0; y < state.previewOrigH; ++y) {
                        memcpy(centered.data() + (static_cast<size_t>(y + offY) * targetW + offX) * 4,
                               state.previewOriginalRGBA.data() +
                               static_cast<size_t>(y) * state.previewOrigW * 4,
                               static_cast<size_t>(state.previewOrigW) * 4);
                    }
                    std::string error;
                    if (save_aligned_to_source(state, centered, targetW, targetH, &error)) {
                        log_msg(state, "已居中对齐 2 次幂并保存");
                    } else {
                        log_msg(state, "居中对齐失败：" + error);
                    }
                }
            }
            if (!canAlign) ImGui::EndDisabled();

            auto sizeModeBtn = [&](const char* label, int mode) {
                if (state.targetSizeMode == mode) push_primary_button_style();
                else push_secondary_button_style();
                const bool clicked = ImGui::Button(label);
                pop_button_style();
                if (clicked) state.targetSizeMode = mode;
            };
            sizeModeBtn("默认尺寸", 0);
            ImGui::SameLine();
            sizeModeBtn("64×64", 1);
            ImGui::SameLine();
            sizeModeBtn("128×128", 2);
            ImGui::SameLine();

            ImGui::SetNextItemWidth(78.0f * state.dpiScale);
            if (ImGui::InputInt("宽##ToolbarResizeWidth", &state.resizeWidth, 0, 0)) {
                state.resizeWidth = std::clamp(state.resizeWidth, 1, 16384);
                if (state.resizeLockAspect && state.previewOrigW > 0 && state.previewOrigH > 0) {
                    state.resizeHeight = std::max(1, static_cast<int>(
                        (static_cast<int64_t>(state.resizeWidth) * state.previewOrigH +
                         state.previewOrigW / 2) / state.previewOrigW));
                }
            }
            ImGui::SameLine();
            ImGui::SetNextItemWidth(78.0f * state.dpiScale);
            if (ImGui::InputInt("高##ToolbarResizeHeight", &state.resizeHeight, 0, 0)) {
                state.resizeHeight = std::clamp(state.resizeHeight, 1, 16384);
                if (state.resizeLockAspect && state.previewOrigW > 0 && state.previewOrigH > 0) {
                    state.resizeWidth = std::max(1, static_cast<int>(
                        (static_cast<int64_t>(state.resizeHeight) * state.previewOrigW +
                         state.previewOrigH / 2) / state.previewOrigH));
                }
            }
            ImGui::SameLine();
            ImGui::Checkbox("锁比例", &state.resizeLockAspect);
            ImGui::SameLine();
            push_primary_button_style();
            if (ImGui::Button("保存尺寸")) {
                resizeAndSave(std::max(1, state.resizeWidth),
                              std::max(1, state.resizeHeight), "已调整图像尺寸并保存");
            }
            pop_button_style();
            ImGui::SameLine();

            const bool convertedRestore = !state.convertedFromBytes.empty() &&
                                          state.currentPreviewPath == state.convertedToPath;
            const bool canRestore = (state.previewAdjusted && state.hasOriginalBackup) ||
                                    convertedRestore;
            if (!canRestore) ImGui::BeginDisabled();
            push_danger_button_style();
            if (ImGui::Button("恢复原图")) {
                std::string error;
                if (convertedRestore) {
                    if (write_file_bytes(state.convertedFromPath, state.convertedFromBytes, &error)) {
                        namespace fs = std::filesystem;
                        std::error_code ec;
                        fs::remove(fs_path_from_utf8(state.convertedToPath), ec);
                        state.thumbCache.invalidate(state.convertedToPath);
                        state.thumbCache.invalidate(state.convertedFromPath);

                        auto       it         = std::find(state.fileList.begin(), state.fileList.end(),
                                                          state.convertedToPath);
                        const bool origListed = state.fileSet.count(state.convertedFromPath) != 0;
                        if (it != state.fileList.end()) {
                            if (origListed) {
                                state.fileList.erase(it);
                            } else {
                                *it = state.convertedFromPath;
                            }
                            state.fileSet.erase(state.convertedToPath);
                            state.relativePathMap.erase(state.convertedToPath);
                        }
                        if (!origListed) state.fileSet.insert(state.convertedFromPath);
                        auto nit = std::find(state.fileList.begin(), state.fileList.end(),
                                             state.convertedFromPath);
                        if (nit != state.fileList.end()) {
                            state.selectedFileIndex =
                                static_cast<int>(nit - state.fileList.begin());
                        }
                        state.currentPreviewPath = state.convertedFromPath;
                        state.convertedFromPath.clear();
                        state.convertedFromBytes.clear();
                        state.convertedToPath.clear();
                        update_preview(state, state.currentPreviewPath);
                        log_msg(state, "已恢复原始文件");
                    } else {
                        log_msg(state, "恢复失败：" + error);
                    }
                } else if (write_file_bytes(state.currentPreviewPath, state.originalFileBytes, &error)) {
                    state.thumbCache.invalidate(state.currentPreviewPath);
                    state.currentMeta    = state.originalMeta;
                    state.previewAdjusted = false;
                    state.previewAdjustedRGBA.clear();
                    update_preview(state, state.currentPreviewPath);
                    log_msg(state, "已恢复原始文件");
                } else {
                    log_msg(state, "恢复失败：" + error);
                }
            }
            pop_button_style();
            if (!canRestore) ImGui::EndDisabled();
        } else {
            ImGui::TextUnformatted("未加载图像");
        }

        ImGui::Separator();

        const ImVec2 stagePos = ImGui::GetCursorPos();
        ImVec2       avail    = ImGui::GetContentRegionAvail();
        state.imageViewer.render(avail.x, avail.y);

        if (state.imageViewer.fitMode && state.imageViewer.hasImage) {
            state.imageViewer.fit_to_view(avail.x, avail.y);
        }

        state.zoomPercent = std::max(1, static_cast<int>(state.imageViewer.zoom * 100.0f + 0.5f));

        if (hasImage && state.currentIsBlp && !state.mipEntries.empty()) {
            const float pad        = 8.0f * state.dpiScale;
            const float panelW     = std::min(220.0f * state.dpiScale,
                                              std::max(150.0f * state.dpiScale, avail.x * 0.28f));
            const float lineH      = ImGui::GetTextLineHeightWithSpacing();
            const int   maxByH     = std::max(1, static_cast<int>(
                                         (avail.y - 56.0f * state.dpiScale) / lineH));
            const int   maxLines   = std::min(6, maxByH);
            const int   shownCount = std::min(static_cast<int>(state.mipEntries.size()), maxLines);
            const int   restCount  = static_cast<int>(state.mipEntries.size()) - shownCount;
            float panelH = lineH + shownCount * lineH +
                           (restCount > 0 ? lineH : 0.0f) + 8.0f * state.dpiScale;
            panelH = std::min(panelH, std::max(80.0f * state.dpiScale, avail.y - 2.0f * pad));

            const float panelX = stagePos.x + avail.x - panelW - pad;
            const float panelY = std::max(stagePos.y + pad,
                                          stagePos.y + avail.y - panelH - pad);

            ImGui::SetCursorPos(ImVec2(panelX, panelY));
            ImGui::PushStyleColor(ImGuiCol_ChildBg, ImVec4(0.0f, 0.0f, 0.0f, 0.22f));
            ImGui::PushStyleColor(ImGuiCol_Text, ImVec4(0.961f, 0.973f, 1.000f, 0.95f));
            ImGui::BeginChild(
                "CanvasMipOverlay", ImVec2(panelW, panelH),
                ImGuiChildFlags_Borders | ImGuiChildFlags_AlwaysUseWindowPadding,
                ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoScrollWithMouse);

            ImGui::TextUnformatted("BLP 层级");
            const int baseW = std::max(1, state.currentMeta.width);
            const int baseH = std::max(1, state.currentMeta.height);
            for (int i = 0; i < shownCount; ++i) {
                const BlpMipEntry& entry = state.mipEntries[i];
                const int mipW = std::max(1, baseW >> entry.index);
                const int mipH = std::max(1, baseH >> entry.index);
                ImGui::Text("第%d层 %d x %d | %s",
                            entry.index + 1, mipW, mipH,
                            format_file_size(entry.size).c_str());
            }
            if (restCount > 0) ImGui::Text("其余 %d 层...", restCount);

            ImGui::EndChild();
            ImGui::PopStyleColor(2);
        }

        ImGui::EndChild();
        ImGui::PopStyleColor();
    }
}
