---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/cli/blp_cli.cpp"
ref: HEAD
---

#include <filesystem>
#include <iostream>
#include <string>
#include <vector>

#ifdef _WIN32
#include <windows.h>
#include <shellapi.h>
#endif

#include "core/blp_api.h"
#include "core/image_io.h"
#include "core/utils.h"

namespace {

std::vector<std::string> command_line_args(int argc, char** argv) {
#ifdef _WIN32
    int wideArgc = 0;
    LPWSTR* wideArgv = CommandLineToArgvW(GetCommandLineW(), &wideArgc);
    if (!wideArgv) {
        return std::vector<std::string>(argv, argv + argc);
    }

    std::vector<std::string> args;
    args.reserve(static_cast<size_t>(wideArgc));
    for (int i = 0; i < wideArgc; ++i) {
        const int needed = WideCharToMultiByte(CP_UTF8, 0, wideArgv[i], -1, nullptr, 0, nullptr, nullptr);
        if (needed <= 0) {
            args.emplace_back();
            continue;
        }

        std::string utf8(static_cast<size_t>(needed - 1), '\0');
        WideCharToMultiByte(CP_UTF8, 0, wideArgv[i], -1, utf8.data(), needed, nullptr, nullptr);
        args.push_back(std::move(utf8));
    }
    LocalFree(wideArgv);
    return args;
#else
    return std::vector<std::string>(argv, argv + argc);
#endif
}

void print_usage() {
    std::cerr << "Usage:\n"
              << "  blp_cli <input.blp> <output.png>\n"
              << "  blp_cli --to-png <input.blp> <output.png>\n";
}

} // namespace

int main(int argc, char** argv) {
    const std::vector<std::string> args = command_line_args(argc, argv);
    if (args.size() != 3 && args.size() != 4) {
        print_usage();
        return 2;
    }

    size_t inputIndex = 1;
    if (args.size() == 4) {
        if (args[1] != "--to-png") {
            print_usage();
            return 2;
        }
        inputIndex = 2;
    }

    const std::string inputPath = args[inputIndex];
    const std::string outputPath = args[inputIndex + 1];

    BlpApi blpApi;
    RgbaImage image;
    ImageMeta meta;
    std::string error;
    if (!load_image_file(inputPath, &image, &meta, &error, &blpApi)) {
        std::cerr << "decode failed: " << error << "\n";
        return 1;
    }

    if (!write_image_file(outputPath, "png", image, 90, 1, &error, &blpApi)) {
        std::cerr << "write failed: " << error << "\n";
        return 1;
    }

    std::cout << "converted " << inputPath << " -> " << outputPath
              << " (" << image.width << "x" << image.height << ")\n";
    return 0;
}
