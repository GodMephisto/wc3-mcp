---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_compose_tab.cpp"
ref: HEAD
---

#include "ui_compose_tab.h"
#include "ui_main.h"
#include "batch_convert.h"
#include "file_dialogs.h"
#include "style.h"
#include "utils.h"
#include "image_io.h"
#include "image_ops.h"
#include "imgui.h"

#include <algorithm>
#include <chrono>
#include <cstdio>
#include <cstring>
#include <ctime>
#include <filesystem>

namespace {

void log_message(AppState& state, const std::string& msg) {
    auto now = std::chrono::system_clock::now();
    std::time_t t = std::chrono::system_clock::to_time_t(now);
    struct tm tm_buf;
    localtime_s(&tm_buf, &t);
    char timeBuf[32];
    std::strftime(timeBuf, sizeof(timeBuf), "%H:%M:%S", &tm_buf);
    char buf[512];
    std::snprintf(buf, sizeof(buf), "[%s] %s", timeBuf, msg.c_str());
    state.logMessages.push_back(buf);
}

} // namespace

void render_compose_tab(AppState& state) {
    ImGui::PushStyleColor(ImGuiCol_Text, ImVec4(0.357f, 0.392f, 0.447f, 1.0f));
    ImGui::TextWrapped("对“资源列表”中的每张图执行图层叠加或加边框，批量输出。");
    ImGui::PopStyleColor();

    ImGui::RadioButton("图层叠加", &state.composeOpMode, 0);
    ImGui::SameLine();
    ImGui::RadioButton("边框", &state.composeOpMode, 1);
    ImGui::Separator();

    if (state.composeOpMode == 0) {
        if (ImGui::Button("选择叠加图...")) {
            auto files = open_file_dialog(state.hwnd, L"*.png;*.jpg;*.jpeg;*.bmp;*.tga;*.blp;*.dds", false);
            if (!files.empty()) {
                state.overlayImagePath = fs_path_to_utf8(std::filesystem::path(files.front()));
            }
        }
        ImGui::SameLine();
        if (state.overlayImagePath.empty()) {
            ImGui::TextDisabled("未选择");
        } else {
            const std::string name = fs_path_to_utf8(fs_path_from_utf8(state.overlayImagePath).filename());
            ImGui::TextUnformatted(name.c_str());
            if (ImGui::IsItemHovered()) ImGui::SetTooltip("%s", state.overlayImagePath.c_str());
        }

        ImGui::TextUnformatted("锚点");
        const char* anchorLabels[9] = {"左上", "上", "右上",
                                       "左",   "中", "右",
                                       "左下", "下", "右下"};
        const float anchorBtnW = std::max(1.0f,
            (ImGui::GetContentRegionAvail().x - ImGui::GetStyle().ItemSpacing.x * 2.0f) / 3.0f);
        for (int a = 0; a < 9; ++a) {
            if (state.overlayAnchor == a) push_primary_button_style();
            else push_secondary_button_style();
            char id[32];
            std::snprintf(id, sizeof(id), "%s##anchor%d", anchorLabels[a], a);
            if (ImGui::Button(id, ImVec2(anchorBtnW, 0))) state.overlayAnchor = a;
            pop_button_style();
            if (a % 3 != 2) ImGui::SameLine();
        }

        ImGui::SliderInt("边距(px)", &state.overlayMarginPx, 0, 256);
        ImGui::SliderInt("缩放(%)", &state.overlayScalePct, 1, 400);
        ImGui::SliderFloat("不透明度", &state.overlayOpacity, 0.0f, 1.0f, "%.2f");
    } else {
        ImGui::SliderInt("边框厚度(px)", &state.borderThicknessPx, 0, 256);
        ImGui::ColorEdit4("边框颜色", state.borderColor);
        ImGui::Checkbox("扩展画布(否则内描边)", &state.borderExpandCanvas);
    }

    ImGui::Separator();
    ImGui::TextUnformatted("输出设置");
    ImGui::PushItemWidth(-84);
    ImGui::InputText("##ComposeOutputDir", state.outputDirBuf, sizeof(state.outputDirBuf));
    ImGui::PopItemWidth();
    ImGui::SameLine();
    if (ImGui::Button("浏览##ComposeOutputBrowse")) {
        auto folder = open_folder_dialog(state.hwnd);
        if (!folder.empty()) {
            std::strncpy(state.outputDirBuf,
                         fs_path_to_utf8(std::filesystem::path(folder)).c_str(),
                         sizeof(state.outputDirBuf) - 1);
        }
    }
    draw_output_format_selector(state);
    if (state.outputFormat == 0 || state.outputFormat == OUTPUT_FORMAT_ORIGINAL) {
        const char* qualityLabel = (state.outputFormat == OUTPUT_FORMAT_ORIGINAL)
            ? "BLP / JPG 质量##compose" : "BLP 质量##compose";
        ImGui::SliderInt(qualityLabel, &state.quality, 0, 100);
    }
    draw_output_size_selector(state);
    ImGui::Checkbox("覆盖已存在文件##compose", &state.overwrite);

    ImGui::Spacing();
    ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(10, 8));
    if (state.converting.load()) {
        ImGui::BeginDisabled();
        ImGui::Button("处理中...", ImVec2(-1, 0));
        ImGui::EndDisabled();
    } else {
        push_primary_button_style();
        if (ImGui::Button("开始处理", ImVec2(-1, 0))) {
            run_compose_batch_from_ui(state);
        }
        pop_button_style();
    }
    ImGui::PopStyleVar();
    ImGui::ProgressBar(state.convertProgress.load(), ImVec2(-1, 0));
}

void run_compose_batch_from_ui(AppState& state) {
    if (state.converting.load()) {
        log_message(state, "已有任务正在执行");
        return;
    }
    const std::vector<std::string>& inputPaths = state.fileList;
    if (inputPaths.empty()) {
        log_message(state, "资源列表为空，无可处理文件");
        return;
    }
    if (!ensure_output_dir(state, inputPaths)) return;

    RgbaImage overlayImg;
    if (state.composeOpMode == 0) {
        if (state.overlayImagePath.empty()) {
            log_message(state, "请先选择叠加图");
            return;
        }
        ImageMeta   om;
        std::string oerr;
        if (!load_image_file(state.overlayImagePath, &overlayImg, &om, &oerr, &state.blpApi)) {
            log_message(state, std::string("叠加图读取失败：") + oerr);
            return;
        }
    }

    const bool preserveOriginalFormat = (state.outputFormat == OUTPUT_FORMAT_ORIGINAL);
    const std::string selectedFormat = preserveOriginalFormat
        ? std::string() : normalized_format_str(state.outputFormat);
    const int total = static_cast<int>(inputPaths.size());
    int successCount = 0;
    int failedCount  = 0;

    state.lastConvertResults.clear();
    state.lastConvertResults.reserve(inputPaths.size());
    state.lastConvertTotal   = total;
    state.lastConvertSuccess = 0;
    state.lastConvertFailed  = 0;
    state.converting         = true;
    state.convertProgress    = 0.0f;

    for (int i = 0; i < total; ++i) {
        const std::string& inputPath = inputPaths[i];
        ConvertItemResult  item;
        item.inputPath = inputPath;

        RgbaImage   image;
        ImageMeta   meta;
        std::string error;
        if (!load_image_file(inputPath, &image, &meta, &error, &state.blpApi)) {
            item.success = false;
            item.error   = error;
            char msg[512];
            std::snprintf(msg, sizeof(msg), "读取失败：%s（%s）", inputPath.c_str(), error.c_str());
            log_message(state, msg);
            ++failedCount;
            state.lastConvertResults.push_back(std::move(item));
            state.convertProgress = static_cast<float>(i + 1) / total;
            continue;
        }

        RgbaImage out;
        if (state.composeOpMode == 0) {
            out = composite_overlay(image, overlayImg, static_cast<Anchor9>(state.overlayAnchor),
                                    state.overlayMarginPx, state.overlayScalePct, state.overlayOpacity);
        } else {
            auto to8 = [](float v) {
                return static_cast<uint8_t>(std::clamp(v * 255.0f + 0.5f, 0.0f, 255.0f));
            };
            out = add_border(image, state.borderThicknessPx,
                             to8(state.borderColor[0]), to8(state.borderColor[1]),
                             to8(state.borderColor[2]), to8(state.borderColor[3]),
                             state.borderExpandCanvas);
        }
        if (out.width <= 0) out = image;

        apply_batch_output_size(state, out);

        const std::string format = preserveOriginalFormat ? meta.format : selectedFormat;
        const bool formatIsBlp   = (format == "blp");
        const std::string outPath  = build_output_path(state, inputPath, format, state.overwrite);
        item.outputPath            = outPath;
        const int mipCount         = formatIsBlp ? auto_mip_count(out.width, out.height) : 1;
        if (!write_image_file(outPath, format, out, state.quality, mipCount, &error, &state.blpApi)) {
            item.success = false;
            item.error   = error;
            char msg[512];
            std::snprintf(msg, sizeof(msg), "写入失败：%s（%s）", outPath.c_str(), error.c_str());
            log_message(state, msg);
            ++failedCount;
            state.lastConvertResults.push_back(std::move(item));
            state.convertProgress = static_cast<float>(i + 1) / total;
            continue;
        }

        item.success = true;
        if (state.fileSet.count(outPath)) state.thumbCache.invalidate(outPath);
        ++successCount;
        state.lastConvertResults.push_back(std::move(item));
        state.convertProgress = static_cast<float>(i + 1) / total;
    }

    state.lastConvertSuccess = successCount;
    state.lastConvertFailed  = failedCount;
    char msg[160];
    std::snprintf(msg, sizeof(msg), "已处理 %d / %d 个文件（失败 %d）",
                  successCount, total, failedCount);
    log_message(state, msg);
    state.converting = false;
}
