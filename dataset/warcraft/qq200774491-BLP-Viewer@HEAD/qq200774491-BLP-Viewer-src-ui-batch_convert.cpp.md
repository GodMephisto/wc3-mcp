---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/batch_convert.cpp"
ref: HEAD
---

#include "batch_convert.h"
#include "ui_main.h"
#include "file_dialogs.h"
#include "style.h"
#include "utils.h"
#include "image_io.h"
#include "image_ops.h"
#include "imgui.h"

#include <algorithm>
#include <cctype>
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

bool resolve_batch_target(const AppState& state, int* tw, int* th) {
    switch (state.batchSizeMode) {
        case 1: *tw = 64;  *th = 64;  return true;
        case 2: *tw = 128; *th = 128; return true;
        case 3: *tw = std::clamp(state.batchWidth,  1, 16384);
                *th = std::clamp(state.batchHeight, 1, 16384); return true;
        default: return false;
    }
}

void run_convert_for_paths_internal(AppState& state, const std::vector<std::string>& inputPaths) {
    if (state.converting.load()) {
        log_message(state, "已有转换任务正在执行");
        return;
    }
    if (inputPaths.empty()) {
        log_message(state, "没有可转换的文件");
        return;
    }
    if (!ensure_output_dir(state, inputPaths)) return;

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
        ConvertItemResult item;
        item.inputPath = inputPath;

        RgbaImage image;
        ImageMeta meta;
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

        apply_batch_output_size(state, image);

        const std::string format = preserveOriginalFormat ? meta.format : selectedFormat;
        const bool formatIsBlp   = (format == "blp");
        std::string outPath  = build_output_path(state, inputPath, format, state.overwrite);
        item.outputPath      = outPath;
        const int mipCount   = formatIsBlp ? auto_mip_count(image.width, image.height) : 1;
        if (!write_image_file(outPath, format, image, state.quality, mipCount, &error, &state.blpApi)) {
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
    std::snprintf(msg, sizeof(msg), "已转换 %d / %d 个文件（失败 %d）",
                  successCount, total, failedCount);
    log_message(state, msg);
    state.converting = false;
}

void do_convert_all(AppState& state) {
    if (state.fileList.empty()) {
        const std::string inputDir = state.inputDirBuf;
        if (!inputDir.empty()) {
            add_folder_files(state, inputDir, state.recursive);
        }
    }
    run_convert_for_paths_internal(state, state.fileList);
}

} // namespace

// ---------------------------------------------------------------------------
// File scanning
// ---------------------------------------------------------------------------

void add_folder_files(AppState& state, const std::string& folder, bool recursive) {
    if (folder.empty()) return;

    namespace fs = std::filesystem;
    const fs::path folderPath = fs_path_from_utf8(folder);
    auto exts = supported_extensions();

    std::vector<std::string> paths;
    std::unordered_map<std::string, std::string> relMap;

    auto process_entry = [&](const fs::directory_entry& entry) {
        if (!entry.is_regular_file()) return;
        std::string ext = entry.path().extension().string();
        if (!ext.empty() && ext[0] == '.') ext = ext.substr(1);
        const std::string normExt = normalize_format(ext);
        bool supported = false;
        for (const auto& e : exts) {
            if (normExt == e) { supported = true; break; }
        }
        if (!supported) return;
        const std::string fullPath = fs_path_to_utf8(entry.path());
        paths.push_back(fullPath);
        const std::string relPath = fs_path_to_utf8(fs::relative(entry.path(), folderPath));
        if (!relPath.empty()) {
            relMap[fullPath] = relPath;
        }
    };

    try {
        if (recursive) {
            for (const auto& entry : fs::recursive_directory_iterator(folderPath))
                process_entry(entry);
        } else {
            for (const auto& entry : fs::directory_iterator(folderPath))
                process_entry(entry);
        }
    } catch (...) {}

    for (const auto& [k, v] : relMap) {
        state.relativePathMap[k] = v;
    }

    const int beforeCount = static_cast<int>(state.fileList.size());
    add_files(state, paths);
    if (static_cast<int>(state.fileList.size()) > beforeCount) {
        state.rightViewMode = 0;
    }
}

void add_files(AppState& state, const std::vector<std::string>& paths) {
    const int startCount = static_cast<int>(state.fileList.size());
    bool addedViaFolder  = false;

    for (const auto& path : paths) {
        namespace fs = std::filesystem;
        try {
            fs::path fsPath = fs_path_from_utf8(path);
            if (!fs::exists(fsPath)) continue;
            if (fs::is_directory(fsPath)) {
                addedViaFolder = true;
                add_folder_files(state, path, state.recursive);
                continue;
            }
            if (!is_supported_file(path)) continue;

            const std::string fullPath = fs_path_to_utf8(fs::absolute(fsPath));
            if (state.fileSet.count(fullPath)) continue;

            state.fileList.push_back(fullPath);
            state.fileSet.insert(fullPath);
        } catch (...) {}
    }

    const int added = static_cast<int>(state.fileList.size()) - startCount;
    if (added > 0) {
        state.selectedFileIndex = static_cast<int>(state.fileList.size()) - 1;
        state.rightViewMode     = (addedViaFolder || added > 1) ? 0 : 1;
    } else if (!state.fileList.empty() && state.selectedFileIndex < 0) {
        state.selectedFileIndex = 0;
    }
}

// ---------------------------------------------------------------------------
// Batch output helpers
// ---------------------------------------------------------------------------

std::string normalized_format_str(int formatIndex) {
    switch (formatIndex) {
        case 0: return "blp";
        case 1: return "png";
        case 2: return "jpg";
        case 3: return "bmp";
        case 4: return "tga";
        case OUTPUT_FORMAT_DDS: return "dds";
        default: return "blp";
    }
}

std::string build_output_path(const AppState& state, const std::string& inputPath,
                               const std::string& format, bool overwrite) {
    namespace fs = std::filesystem;
    const std::string outputDir     = state.outputDirBuf;
    const fs::path    outputDirPath = fs_path_from_utf8(outputDir);

    auto it = state.relativePathMap.find(inputPath);
    std::string baseName;
    std::string relativeDir;
    if (it != state.relativePathMap.end()) {
        fs::path relPath = fs_path_from_utf8(it->second);
        baseName    = fs_path_to_utf8(relPath.stem());
        relativeDir = fs_path_to_utf8(relPath.parent_path());
        if (relativeDir == ".") relativeDir.clear();
    } else {
        baseName = fs_path_to_utf8(fs_path_from_utf8(inputPath).stem());
    }

    const std::string ext        = normalize_format(format);
    const fs::path    targetDir  = relativeDir.empty()
                                       ? outputDirPath
                                       : (outputDirPath / fs_path_from_utf8(relativeDir));
    fs::path candidatePath = targetDir / fs_path_from_utf8(baseName + "." + ext);
    std::string candidate  = fs_path_to_utf8(candidatePath);
    if (overwrite || !fs::exists(candidatePath)) return candidate;

    for (int index = 1; fs::exists(candidatePath); ++index) {
        char buf[32];
        std::snprintf(buf, sizeof(buf), "_%d.", index);
        candidatePath = targetDir / fs_path_from_utf8(baseName + buf + ext);
        candidate     = fs_path_to_utf8(candidatePath);
    }
    return candidate;
}

bool ensure_output_dir(AppState& state, const std::vector<std::string>& inputPaths) {
    const std::string outputDir = state.outputDirBuf;
    if (!outputDir.empty()) return true;

    if (inputPaths.size() == 1) {
        const std::string dir = fs_path_to_utf8(fs_path_from_utf8(inputPaths[0]).parent_path());
        std::strncpy(state.outputDirBuf, dir.c_str(), sizeof(state.outputDirBuf) - 1);
        log_message(state, "未设置输出目录，已使用源文件目录");
        return !dir.empty();
    }

    log_message(state, "请先设置输出目录");
    return false;
}

void apply_batch_output_size(const AppState& state, RgbaImage& image) {
    int tw = 0, th = 0;
    if (!resolve_batch_target(state, &tw, &th)) return;
    if (tw == image.width && th == image.height) return;
    const ResizeMethod method = (state.batchResizeMethod == 0)
        ? ResizeMethod::Stretch : ResizeMethod::CenterTransparent;
    RgbaImage resized = transform_to_target(image.pixels.data(), image.width, image.height,
                                            tw, th, method);
    if (resized.width > 0) image = std::move(resized);
}

// ---------------------------------------------------------------------------
// Shared UI selectors
// ---------------------------------------------------------------------------

void draw_output_format_selector(AppState& state) {
    ImGui::TextUnformatted("输出格式");
    auto draw_format_button = [&](const char* label, int formatId, float width) {
        if (state.outputFormat == formatId) push_primary_button_style();
        else push_secondary_button_style();
        const bool clicked = ImGui::Button(label, ImVec2(width, 0));
        pop_button_style();
        if (clicked) state.outputFormat = formatId;
    };
    const float formatSpacing = ImGui::GetStyle().ItemSpacing.x;
    const float formatAvail   = ImGui::GetContentRegionAvail().x;
    const float formatButtonW = std::max(1.0f, (formatAvail - formatSpacing * 2.0f) / 3.0f);
    draw_format_button("原始格式", OUTPUT_FORMAT_ORIGINAL, formatButtonW);
    ImGui::SameLine();
    draw_format_button("BLP", 0, formatButtonW);
    ImGui::SameLine();
    draw_format_button("PNG", 1, formatButtonW);
    draw_format_button("JPG", 2, formatButtonW);
    ImGui::SameLine();
    draw_format_button("BMP", 3, formatButtonW);
    ImGui::SameLine();
    draw_format_button("TGA", 4, formatButtonW);
    draw_format_button("DDS", OUTPUT_FORMAT_DDS, formatButtonW);
}

void draw_output_size_selector(AppState& state) {
    ImGui::TextUnformatted("输出尺寸");
    const float spacing = ImGui::GetStyle().ItemSpacing.x;
    const float halfW   = std::max(1.0f, (ImGui::GetContentRegionAvail().x - spacing) / 2.0f);

    auto size_mode_btn = [&](const char* label, int mode) {
        if (state.batchSizeMode == mode) push_primary_button_style();
        else push_secondary_button_style();
        const bool clicked = ImGui::Button(label, ImVec2(halfW, 0));
        pop_button_style();
        if (clicked) state.batchSizeMode = mode;
    };
    size_mode_btn("原始大小", 0);
    ImGui::SameLine();
    size_mode_btn("64×64", 1);
    size_mode_btn("128×128", 2);
    ImGui::SameLine();
    size_mode_btn("自定义", 3);

    if (state.batchSizeMode == 3) {
        ImGui::SetNextItemWidth(78.0f * state.dpiScale);
        if (ImGui::InputInt("宽##BatchW", &state.batchWidth, 0, 0)) {
            state.batchWidth = std::clamp(state.batchWidth, 1, 16384);
            if (state.batchLockAspect && state.batchAspect > 0.0f) {
                state.batchHeight = std::clamp(
                    static_cast<int>(state.batchWidth / state.batchAspect + 0.5f), 1, 16384);
            }
        }
        ImGui::SameLine();
        ImGui::SetNextItemWidth(78.0f * state.dpiScale);
        if (ImGui::InputInt("高##BatchH", &state.batchHeight, 0, 0)) {
            state.batchHeight = std::clamp(state.batchHeight, 1, 16384);
            if (state.batchLockAspect && state.batchAspect > 0.0f) {
                state.batchWidth = std::clamp(
                    static_cast<int>(state.batchHeight * state.batchAspect + 0.5f), 1, 16384);
            }
        }
        ImGui::SameLine();
        if (ImGui::Checkbox("锁比例##Batch", &state.batchLockAspect) && state.batchLockAspect) {
            state.batchAspect = static_cast<float>(std::max(1, state.batchWidth)) /
                                static_cast<float>(std::max(1, state.batchHeight));
        }
    }

    if (state.batchSizeMode != 0) {
        ImGui::TextUnformatted("处理方式");
        auto method_btn = [&](const char* label, int method) {
            if (state.batchResizeMethod == method) push_primary_button_style();
            else push_secondary_button_style();
            const bool clicked = ImGui::Button(label, ImVec2(halfW, 0));
            pop_button_style();
            if (clicked) state.batchResizeMethod = method;
        };
        method_btn("拉伸##BatchMethod", 0);
        ImGui::SameLine();
        method_btn("居中透明##BatchMethod", 1);
    }
}

// ---------------------------------------------------------------------------
// Batch-convert tab UI
// ---------------------------------------------------------------------------

void render_batch_convert_tab(AppState& state) {
    ImGui::TextUnformatted("输入来源");
    ImGui::PushItemWidth(-84);
    ImGui::InputText("##InputDir", state.inputDirBuf, sizeof(state.inputDirBuf));
    ImGui::PopItemWidth();
    ImGui::SameLine();
    if (ImGui::Button("浏览##InputBrowse")) {
        auto folder = open_folder_dialog(state.hwnd);
        if (!folder.empty()) {
            std::strncpy(state.inputDirBuf,
                         fs_path_to_utf8(std::filesystem::path(folder)).c_str(),
                         sizeof(state.inputDirBuf) - 1);
        }
    }
    ImGui::Checkbox("包含子目录", &state.recursive);
    ImGui::SameLine();
    if (ImGui::Button("扫描并添加")) {
        const std::string dir = state.inputDirBuf;
        if (!dir.empty()) {
            namespace fs = std::filesystem;
            fs::path dirPath = fs_path_from_utf8(dir);
            if (fs::exists(dirPath) && fs::is_directory(dirPath)) {
                add_folder_files(state, dir, state.recursive);
            } else {
                log_message(state, "输入目录不存在");
            }
        } else {
            log_message(state, "请先设置输入目录");
        }
    }

    ImGui::Separator();

    ImGui::TextUnformatted("输出设置");
    ImGui::PushItemWidth(-84);
    ImGui::InputText("##OutputDir", state.outputDirBuf, sizeof(state.outputDirBuf));
    ImGui::PopItemWidth();
    ImGui::SameLine();
    if (ImGui::Button("浏览##OutputBrowse")) {
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
            ? "BLP / JPG 质量" : "BLP 质量";
        ImGui::SliderInt(qualityLabel, &state.quality, 0, 100);
    } else {
        ImGui::TextDisabled("非 BLP 输出质量：%d（可在“编辑”菜单调整）", state.quality);
    }

    ImGui::Separator();
    draw_output_size_selector(state);
    ImGui::Separator();

    ImGui::Checkbox("覆盖已存在文件", &state.overwrite);

    ImGui::Spacing();
    ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(10, 8));
    if (state.converting.load()) {
        ImGui::BeginDisabled();
        ImGui::Button("转换中...", ImVec2(-1, 0));
        ImGui::EndDisabled();
    } else {
        push_primary_button_style();
        if (ImGui::Button("开始转换", ImVec2(-1, 0))) {
            ImGui::OpenPopup("确认开始转换");
        }
        pop_button_style();
    }
    ImGui::PopStyleVar();

    if (ImGui::BeginPopupModal("确认开始转换", nullptr, ImGuiWindowFlags_AlwaysAutoResize)) {
        const bool hasOutputDir = state.outputDirBuf[0] != '\0';
        const bool hasFiles     = !state.fileList.empty();
        const bool hasInputDir  = state.inputDirBuf[0] != '\0';
        const char* formatNames[] = {"BLP", "PNG", "JPG", "BMP", "TGA", "原始格式", "DDS"};
        const char* formatName = formatNames[
            std::clamp(state.outputFormat, 0, OUTPUT_FORMAT_MAX)];

        if (hasFiles) {
            ImGui::Text("将转换 %d 个文件，输出格式：%s",
                        static_cast<int>(state.fileList.size()), formatName);
        } else if (hasInputDir) {
            ImGui::Text("资源列表为空，将先扫描输入目录再转换，输出格式：%s", formatName);
        } else {
            ImGui::PushStyleColor(ImGuiCol_Text, ui_color_error());
            ImGui::TextUnformatted("没有可转换的文件，请先添加文件或设置输入目录。");
            ImGui::PopStyleColor();
        }

        char sizeDesc[96];
        const char* methodName = (state.batchResizeMethod == 0) ? "拉伸" : "居中透明";
        switch (state.batchSizeMode) {
            case 1: std::snprintf(sizeDesc, sizeof(sizeDesc), "输出尺寸：64×64（%s）", methodName); break;
            case 2: std::snprintf(sizeDesc, sizeof(sizeDesc), "输出尺寸：128×128（%s）", methodName); break;
            case 3: std::snprintf(sizeDesc, sizeof(sizeDesc), "输出尺寸：%d×%d（%s）",
                                  state.batchWidth, state.batchHeight, methodName); break;
            default: std::snprintf(sizeDesc, sizeof(sizeDesc), "输出尺寸：原始大小"); break;
        }
        ImGui::TextUnformatted(sizeDesc);

        if (hasOutputDir) {
            ImGui::Text("输出目录：");
            ImGui::SameLine();
            ImGui::TextWrapped("%s", state.outputDirBuf);
        } else {
            ImGui::PushStyleColor(ImGuiCol_Text, ui_color_warning());
            ImGui::TextWrapped("尚未设置输出目录！转换结果将直接保存到各源文件所在目录。");
            ImGui::PopStyleColor();
            if (ImGui::Button("选择输出目录...")) {
                auto folder = open_folder_dialog(state.hwnd);
                if (!folder.empty()) {
                    std::strncpy(state.outputDirBuf,
                                 fs_path_to_utf8(std::filesystem::path(folder)).c_str(),
                                 sizeof(state.outputDirBuf) - 1);
                }
            }
        }

        ImGui::Separator();
        const float btnW = 120.0f * state.dpiScale;
        if (hasFiles || hasInputDir) {
            push_primary_button_style();
            if (ImGui::Button("确认转换", ImVec2(btnW, 0))) {
                ImGui::CloseCurrentPopup();
                run_convert_all_from_ui(state);
            }
            pop_button_style();
            ImGui::SameLine();
        }
        if (ImGui::Button("取消", ImVec2(btnW, 0))) {
            ImGui::CloseCurrentPopup();
        }
        ImGui::EndPopup();
    }

    ImGui::ProgressBar(state.convertProgress.load(), ImVec2(-1, 0));
}

// ---------------------------------------------------------------------------
// Public execution entry points
// ---------------------------------------------------------------------------

void run_convert_all_from_ui(AppState& state) {
    do_convert_all(state);
}

void run_convert_for_paths(AppState& state, const std::vector<std::string>& inputPaths) {
    run_convert_for_paths_internal(state, inputPaths);
}
