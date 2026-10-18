---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/preview_manager.cpp"
ref: HEAD
---

#include "preview_manager.h"
#include "ui_main.h"
#include "utils.h"
#include "image_io.h"
#include "blp_api.h"
#include "batch_convert.h"

#include <algorithm>
#include <cstdio>
#include <cstring>
#include <filesystem>
#include <fstream>

namespace {

std::vector<BlpMipEntry> read_blp_mip_entries(const std::vector<uint8_t>& bytes) {
    std::vector<BlpMipEntry> entries;
    if (bytes.size() < 148) return entries;

    const uint8_t* data    = bytes.data();
    const bool     isBlp1  = (memcmp(data, "BLP1", 4) == 0);
    const bool     isBlp2  = (memcmp(data, "BLP2", 4) == 0);
    if (!isBlp1 && !isBlp2) return entries;

    const int headerSize    = isBlp1 ? 156 : 148;
    if (static_cast<int>(bytes.size()) < headerSize) return entries;

    const int offsetsOffset = isBlp1 ? 28 : 20;
    const int sizesOffset   = offsetsOffset + 16 * 4;

    for (int i = 0; i < 16; ++i) {
        uint32_t offset = 0, size = 0;
        memcpy(&offset, data + offsetsOffset + i * 4, 4);
        memcpy(&size,   data + sizesOffset   + i * 4, 4);
        if (offset != 0 && size != 0) {
            entries.push_back({i, offset, size});
        }
    }
    return entries;
}

} // namespace

std::string single_save_format_str(const AppState& state) {
    if (state.outputFormat == OUTPUT_FORMAT_ORIGINAL && !state.currentMeta.format.empty()) {
        return normalize_format(state.currentMeta.format);
    }
    return normalized_format_str(state.outputFormat);
}

void update_preview(AppState& state, const std::string& path) {
    state.currentPreviewPath = path;
    state.currentBlpBytes.clear();
    state.originalFileBytes.clear();
    state.currentMeta     = ImageMeta();
    state.originalMeta    = ImageMeta();
    state.currentIsBlp    = false;
    state.currentMipIndex = 0;
    state.hasOriginalBackup = false;
    state.previewAdjusted   = false;
    state.previewOriginalRGBA.clear();
    state.previewAdjustedRGBA.clear();
    state.mipEntries.clear();
    state.selectedMipIndex = 0;

    namespace fs = std::filesystem;
    const fs::path fsInputPath = fs_path_from_utf8(path);
    std::string ext = fsInputPath.extension().string();
    if (!ext.empty() && ext[0] == '.') ext = ext.substr(1);
    ext = normalize_format(ext);

    std::ifstream file(fsInputPath, std::ios::binary | std::ios::ate);
    if (!file.is_open()) {
        state.logMessages.push_back("打开失败：" + path);
        state.imageViewer.clear_image();
        return;
    }
    const auto fileSize = file.tellg();
    file.seekg(0);
    std::vector<uint8_t> bytes(static_cast<size_t>(fileSize));
    file.read(reinterpret_cast<char*>(bytes.data()), fileSize);
    file.close();

    if (bytes.empty()) {
        state.logMessages.push_back("文件为空：" + path);
        state.imageViewer.clear_image();
        return;
    }

    state.originalFileBytes = bytes;

    RgbaImage  image;
    ImageMeta  meta;
    std::string error;

    if (ext == "blp") {
        state.currentIsBlp    = true;
        state.currentBlpBytes = bytes;

        blpcodec::RawImage blpImage;
        if (!state.blpApi.load_from_buffer(bytes, &blpImage, &error)) {
            state.logMessages.push_back("BLP 解码失败：" + path +
                                        (error.empty() ? "" : "（" + error + "）"));
            state.imageViewer.clear_image();
            return;
        }

        image.width  = static_cast<int>(blpImage.width);
        image.height = static_cast<int>(blpImage.height);
        image.pixels = std::move(blpImage.rgba);

        meta.width    = image.width;
        meta.height   = image.height;
        meta.format   = "blp";
        meta.fileSize = static_cast<uint64_t>(fileSize);

        state.mipEntries = read_blp_mip_entries(bytes);
    } else {
        if (!load_image_file(path, &image, &meta, &error, &state.blpApi)) {
            state.logMessages.push_back("预览失败：" + path + "（" + error + "）");
            state.imageViewer.clear_image();
            return;
        }
    }

    state.currentMeta       = meta;
    state.originalMeta      = meta;
    state.hasOriginalBackup = !state.originalFileBytes.empty();
    state.previewOriginalRGBA = image.pixels;
    state.previewOrigW        = image.width;
    state.previewOrigH        = image.height;

    std::vector<uint8_t> displayPixels = image.pixels;
    if (state.currentIsBlp && !state.previewUseAlpha) {
        for (size_t i = 3; i < displayPixels.size(); i += 4) {
            displayPixels[i] = 255;
        }
    }

    state.imageViewer.set_image(state.device, displayPixels.data(), image.width, image.height);
    state.imageViewer.fitMode = true;

    state.resizeWidth  = image.width;
    state.resizeHeight = image.height;
}

void refresh_preview_display(AppState& state) {
    const auto& rgba = (state.previewAdjusted && !state.previewAdjustedRGBA.empty())
                           ? state.previewAdjustedRGBA
                           : state.previewOriginalRGBA;
    const int w = state.previewAdjusted ? state.previewAdjW : state.previewOrigW;
    const int h = state.previewAdjusted ? state.previewAdjH : state.previewOrigH;

    if (rgba.empty()) return;

    std::vector<uint8_t> displayPixels = rgba;
    if (state.currentIsBlp && !state.previewUseAlpha) {
        for (size_t i = 3; i < displayPixels.size(); i += 4) {
            displayPixels[i] = 255;
        }
    }

    state.imageViewer.set_image(state.device, displayPixels.data(), w, h);
}

bool save_aligned_to_source(AppState& state, const std::vector<uint8_t>& rgba,
                             int w, int h, std::string* outError) {
    if (state.currentPreviewPath.empty()) {
        if (outError) *outError = "未选择文件";
        return false;
    }

    namespace fs = std::filesystem;
    const std::string format   = single_save_format_str(state);
    fs::path          outFsPath = fs_path_from_utf8(state.currentPreviewPath);
    outFsPath.replace_extension("." + format);
    const std::string outPath      = fs_path_to_utf8(outFsPath);
    const bool        formatChanged = (outPath != state.currentPreviewPath);

    RgbaImage img;
    img.width  = w;
    img.height = h;
    img.pixels = rgba;

    const int mipCount = (format == "blp") ? auto_mip_count(w, h) : 1;
    if (!write_image_file(outPath, format, img, state.quality, mipCount, outError, &state.blpApi)) {
        return false;
    }
    state.thumbCache.invalidate(outPath);

    const std::string oldPath = state.currentPreviewPath;
    state.currentPreviewPath  = outPath;
    if (formatChanged) {
        state.convertedFromPath  = oldPath;
        state.convertedFromBytes = state.originalFileBytes;
        state.convertedToPath    = outPath;

        std::error_code removeEc;
        if (!fs::remove(fs_path_from_utf8(oldPath), removeEc) || removeEc) {
            state.logMessages.push_back("原文件删除失败：" + oldPath);
        }
        state.thumbCache.invalidate(oldPath);

        const bool newAlreadyListed = state.fileSet.count(outPath) != 0;
        auto oldIt = std::find(state.fileList.begin(), state.fileList.end(), oldPath);
        if (oldIt != state.fileList.end()) {
            if (newAlreadyListed) {
                state.fileList.erase(oldIt);
            } else {
                *oldIt = outPath;
            }
            state.fileSet.erase(oldPath);
            auto relIt = state.relativePathMap.find(oldPath);
            if (relIt != state.relativePathMap.end()) {
                fs::path relPath = fs_path_from_utf8(relIt->second);
                relPath.replace_extension("." + format);
                state.relativePathMap.erase(relIt);
                state.relativePathMap[outPath] = fs_path_to_utf8(relPath);
            }
        } else if (!newAlreadyListed) {
            state.fileList.push_back(outPath);
        }
        if (!newAlreadyListed) {
            state.fileSet.insert(outPath);
        }
        auto it = std::find(state.fileList.begin(), state.fileList.end(), outPath);
        if (it != state.fileList.end()) {
            state.selectedFileIndex = static_cast<int>(it - state.fileList.begin());
        }
    }

    state.currentMeta.width  = w;
    state.currentMeta.height = h;
    state.currentMeta.format = format;
    try {
        state.currentMeta.fileSize = static_cast<uint64_t>(
            fs::file_size(fs_path_from_utf8(state.currentPreviewPath)));
    } catch (...) {}

    state.currentIsBlp    = (format == "blp");
    state.currentMipIndex = 0;

    if (state.currentIsBlp) {
        std::ifstream f(fs_path_from_utf8(state.currentPreviewPath),
                        std::ios::binary | std::ios::ate);
        if (f.is_open()) {
            const auto sz = f.tellg();
            f.seekg(0);
            state.currentBlpBytes.resize(static_cast<size_t>(sz));
            f.read(reinterpret_cast<char*>(state.currentBlpBytes.data()), sz);
            state.mipEntries = read_blp_mip_entries(state.currentBlpBytes);
        }
    } else {
        state.currentBlpBytes.clear();
        state.mipEntries.clear();
    }

    state.previewAdjustedRGBA = rgba;
    state.previewAdjW         = w;
    state.previewAdjH         = h;
    state.previewAdjusted     = true;
    state.resizeWidth         = w;
    state.resizeHeight        = h;

    refresh_preview_display(state);
    return true;
}
