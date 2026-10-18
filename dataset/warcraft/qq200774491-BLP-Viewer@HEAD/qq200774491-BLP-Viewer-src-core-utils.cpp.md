---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/utils.cpp"
ref: HEAD
---

#include "utils.h"

#include <algorithm>
#include <cstdio>
#include <filesystem>

namespace {
    constexpr int MAX_POW2          = 1 << 30;
    constexpr int MAX_MIPMAP_COUNT  = 16;
} // namespace

std::filesystem::path fs_path_from_utf8(const std::string& utf8Path) {
    try {
        return std::filesystem::u8path(utf8Path);
    } catch (...) {
        return std::filesystem::path(utf8Path);
    }
}

std::string fs_path_to_utf8(const std::filesystem::path& path) {
    try {
        return path.u8string();
    } catch (...) {
        return path.string();
    }
}

int nearest_power_of_two(int value) {
    if (value <= 0) return 1;
    if (is_power_of_two(value)) return value;

    int upper = 1;
    while (upper < value && upper < MAX_POW2) upper <<= 1;
    int lower = upper >> 1;
    if (value - lower < upper - value) return lower;
    if (value - lower > upper - value) return upper;
    return upper;
}

int next_power_of_two(int value) {
    if (value <= 0) return 1;
    int upper = 1;
    while (upper < value && upper < MAX_POW2) upper <<= 1;
    return upper;
}

int auto_mip_count(int width, int height) {
    int maxDim = (width > height) ? width : height;
    int count = 1;
    while (maxDim > 1 && count < MAX_MIPMAP_COUNT) {
        maxDim /= 2;
        ++count;
    }
    return count;
}

std::string format_file_size(uint64_t bytes) {
    char buf[64];
    const double KB = 1024.0;
    const double MB = KB * 1024.0;
    const double GB = MB * 1024.0;

    if (bytes >= static_cast<uint64_t>(GB)) {
        std::snprintf(buf, sizeof(buf), "%.2f GB", static_cast<double>(bytes) / GB);
    } else if (bytes >= static_cast<uint64_t>(MB)) {
        std::snprintf(buf, sizeof(buf), "%.2f MB", static_cast<double>(bytes) / MB);
    } else if (bytes >= static_cast<uint64_t>(KB)) {
        std::snprintf(buf, sizeof(buf), "%.2f KB", static_cast<double>(bytes) / KB);
    } else {
        std::snprintf(buf, sizeof(buf), "%llu B", static_cast<unsigned long long>(bytes));
    }
    return buf;
}

std::string normalize_format(const std::string& format) {
    std::string fmt = format;
    while (!fmt.empty() && std::isspace(static_cast<unsigned char>(fmt.front())))
        fmt.erase(fmt.begin());
    while (!fmt.empty() && std::isspace(static_cast<unsigned char>(fmt.back())))
        fmt.pop_back();
    std::transform(fmt.begin(), fmt.end(), fmt.begin(),
                   [](unsigned char c) { return static_cast<char>(std::tolower(c)); });
    if (fmt == "jpeg") return "jpg";
    return fmt;
}

std::vector<std::string> supported_extensions() {
    return {"blp", "dds", "png", "jpg", "jpeg", "bmp", "tga"};
}

bool is_supported_file(const std::string& path) {
    std::string ext = fs_path_from_utf8(path).extension().string();
    if (!ext.empty() && ext[0] == '.') ext = ext.substr(1);
    const std::string normalized = normalize_format(ext);
    for (const auto& supported : supported_extensions()) {
        if (normalized == supported) return true;
    }
    return false;
}
