---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/settings.cpp"
ref: HEAD
---

#include "settings.h"
#include "utils.h"

#include <algorithm>
#include <charconv>
#include <cstdio>
#include <filesystem>
#include <fstream>

#include <Windows.h>
#include <ShlObj.h>

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

namespace {

/// Parse a decimal integer from a string_view; leaves result unchanged on failure.
void parse_int(const std::string& s, int& result) {
    std::from_chars(s.data(), s.data() + s.size(), result);
}

/// Parse a float from a string; leaves result unchanged on failure.
void parse_float(const std::string& s, float& result) {
    float tmp = result;
    auto [ptr, ec] = std::from_chars(s.data(), s.data() + s.size(), tmp);
    if (ec == std::errc{}) result = tmp;
}

/// Trim leading and trailing ASCII whitespace in-place.
void trim_inplace(std::string& s) {
    while (!s.empty() && std::isspace(static_cast<unsigned char>(s.back())))
        s.pop_back();
    while (!s.empty() && std::isspace(static_cast<unsigned char>(s.front())))
        s.erase(s.begin());
}

} // namespace

// ---------------------------------------------------------------------------
// AppSettings implementation
// ---------------------------------------------------------------------------

std::string AppSettings::settings_path() {
    wchar_t* appData = nullptr;
    if (SUCCEEDED(SHGetKnownFolderPath(FOLDERID_RoamingAppData, 0, nullptr, &appData))) {
        char buf[MAX_PATH * 3] = {};
        WideCharToMultiByte(CP_UTF8, 0, appData, -1, buf, sizeof(buf), nullptr, nullptr);
        CoTaskMemFree(appData);
        std::filesystem::path dir = fs_path_from_utf8(buf) / "blp_viewer";
        std::filesystem::create_directories(dir);
        return fs_path_to_utf8(dir / "settings.ini");
    }
    return "settings.ini";
}

void AppSettings::load() {
    std::ifstream file(fs_path_from_utf8(settings_path()));
    if (!file.is_open()) return;

    std::string line;
    while (std::getline(file, line)) {
        if (line.empty() || line[0] == '#' || line[0] == '[') continue;

        const auto eq = line.find('=');
        if (eq == std::string::npos) continue;

        std::string key   = line.substr(0, eq);
        std::string value = line.substr(eq + 1);

        // Trim trailing whitespace from key, leading whitespace from value.
        while (!key.empty() && std::isspace(static_cast<unsigned char>(key.back())))
            key.pop_back();
        while (!value.empty() && std::isspace(static_cast<unsigned char>(value.front())))
            value.erase(value.begin());

        if      (key == "windowX")          parse_int(value, windowX);
        else if (key == "windowY")          parse_int(value, windowY);
        else if (key == "windowW")          parse_int(value, windowW);
        else if (key == "windowH")          parse_int(value, windowH);
        else if (key == "splitterPos")      parse_float(value, splitterPos);
        else if (key == "outputFormat") {
            parse_int(value, outputFormat);
            outputFormat = std::clamp(outputFormat, 0, OUTPUT_FORMAT_MAX);
        }
        else if (key == "quality")          parse_int(value, quality);
        else if (key == "overwrite")        overwrite        = (value == "1");
        else if (key == "recursive")        recursive        = (value == "1");
        else if (key == "batchSizeMode") {
            parse_int(value, batchSizeMode);
            batchSizeMode = std::clamp(batchSizeMode, 0, 3);
        } else if (key == "batchWidth") {
            parse_int(value, batchWidth);
            batchWidth = std::clamp(batchWidth, 1, 16384);
        } else if (key == "batchHeight") {
            parse_int(value, batchHeight);
            batchHeight = std::clamp(batchHeight, 1, 16384);
        } else if (key == "batchLockAspect")   batchLockAspect   = (value == "1");
        else if (key == "batchResizeMethod") {
            parse_int(value, batchResizeMethod);
            batchResizeMethod = std::clamp(batchResizeMethod, 0, 1);
        } else if (key == "lastInputDir")   lastInputDir  = value;
        else if (key == "lastOutputDir")    lastOutputDir = value;
    }
}

void AppSettings::save() const {
    const std::string path = settings_path();
    const std::filesystem::path fsPath = fs_path_from_utf8(path);
    std::filesystem::create_directories(fsPath.parent_path());

    std::ofstream file(fsPath, std::ios::trunc);
    if (!file.is_open()) return;

    file << "[window]\n";
    file << "windowX="     << windowX     << "\n";
    file << "windowY="     << windowY     << "\n";
    file << "windowW="     << windowW     << "\n";
    file << "windowH="     << windowH     << "\n";
    file << "splitterPos=" << splitterPos << "\n";

    file << "\n[convert]\n";
    file << "outputFormat=" << outputFormat                << "\n";
    file << "quality="      << quality                     << "\n";
    file << "overwrite="    << (overwrite  ? "1" : "0")   << "\n";
    file << "recursive="    << (recursive  ? "1" : "0")   << "\n";

    file << "\n[batch]\n";
    file << "batchSizeMode="     << batchSizeMode                       << "\n";
    file << "batchWidth="        << batchWidth                          << "\n";
    file << "batchHeight="       << batchHeight                         << "\n";
    file << "batchLockAspect="   << (batchLockAspect ? "1" : "0")      << "\n";
    file << "batchResizeMethod=" << batchResizeMethod                   << "\n";

    file << "\n[paths]\n";
    file << "lastInputDir="  << lastInputDir  << "\n";
    file << "lastOutputDir=" << lastOutputDir << "\n";
}
