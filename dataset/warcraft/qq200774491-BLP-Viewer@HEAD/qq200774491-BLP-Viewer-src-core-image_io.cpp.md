---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_io.cpp"
ref: HEAD
---

#include "image_io.h"

#include <algorithm>
#include <cstring>
#include <filesystem>
#include <fstream>
#include <memory>

#include "blp_api.h"
#include "dds_codec.h"
#include "utils.h"

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"
#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image_write.h"

namespace {

constexpr size_t BLP_MAGIC_BYTES = 4;
constexpr int    RGBA_CHANNELS   = 4;
constexpr int    RGB_CHANNELS    = 3;

std::string extension_from_path(const std::string& path) {
    std::string ext = fs_path_from_utf8(path).extension().string();
    if (!ext.empty() && ext[0] == '.') ext = ext.substr(1);
    std::transform(ext.begin(), ext.end(), ext.begin(),
                   [](unsigned char c) { return static_cast<char>(std::tolower(c)); });
    return ext;
}

void append_bytes(void* context, void* data, int size) {
    auto* out       = reinterpret_cast<std::vector<uint8_t>*>(context);
    const auto* src = reinterpret_cast<const uint8_t*>(data);
    out->insert(out->end(), src, src + size);
}

std::vector<uint8_t> rgba_to_rgb_bytes(const RgbaImage& image) {
    std::vector<uint8_t> rgb;
    if (image.width <= 0 || image.height <= 0) return rgb;

    const int pixelCount = image.width * image.height;
    rgb.resize(static_cast<size_t>(pixelCount) * RGB_CHANNELS);

    const uint8_t* src = image.pixels.data();
    uint8_t*       dst = rgb.data();

    for (int i = 0; i < pixelCount; ++i) {
        dst[i * RGB_CHANNELS + 0] = src[i * RGBA_CHANNELS + 0];
        dst[i * RGB_CHANNELS + 1] = src[i * RGBA_CHANNELS + 1];
        dst[i * RGB_CHANNELS + 2] = src[i * RGBA_CHANNELS + 2];
    }

    return rgb;
}

bool encode_to_bytes(const RgbaImage& image,
                     const std::string& format,
                     int quality,
                     std::vector<uint8_t>* outBytes,
                     std::string* outError) {
    if (!outBytes) {
        if (outError) *outError = "输出缓冲区为空";
        return false;
    }

    outBytes->clear();

    if (image.width <= 0 || image.height <= 0 || image.pixels.empty()) {
        if (outError) *outError = "无效的图像数据";
        return false;
    }

    const std::string fmt = normalize_format(format);

    if (fmt == "png") {
        const int stride = image.width * RGBA_CHANNELS;
        int ok = stbi_write_png_to_func(append_bytes, outBytes,
                                        image.width, image.height, RGBA_CHANNELS,
                                        image.pixels.data(), stride);
        if (!ok && outError) *outError = "PNG 编码失败";
        return ok != 0;
    }

    if (fmt == "tga") {
        int ok = stbi_write_tga_to_func(append_bytes, outBytes,
                                        image.width, image.height, RGBA_CHANNELS,
                                        image.pixels.data());
        if (!ok && outError) *outError = "TGA 编码失败";
        return ok != 0;
    }

    if (fmt == "bmp") {
        const std::vector<uint8_t> rgb = rgba_to_rgb_bytes(image);
        int ok = stbi_write_bmp_to_func(append_bytes, outBytes,
                                        image.width, image.height, RGB_CHANNELS,
                                        rgb.data());
        if (!ok && outError) *outError = "BMP 编码失败";
        return ok != 0;
    }

    if (fmt == "jpg") {
        const std::vector<uint8_t> rgb = rgba_to_rgb_bytes(image);
        const int clampedQuality = std::clamp(quality, 1, 100);
        int ok = stbi_write_jpg_to_func(append_bytes, outBytes,
                                        image.width, image.height, RGB_CHANNELS,
                                        rgb.data(), clampedQuality);
        if (!ok && outError) *outError = "JPG 编码失败";
        return ok != 0;
    }

    if (outError) *outError = "不支持的输出格式";
    return false;
}

} // namespace

bool load_image_file(const std::string& path,
                     RgbaImage* outImage,
                     ImageMeta* outMeta,
                     std::string* outError,
                     BlpApi* blpApi) {
    if (!outImage) {
        if (outError) *outError = "输出图像为空";
        return false;
    }

    const std::filesystem::path fsPath = fs_path_from_utf8(path);
    std::ifstream file(fsPath, std::ios::binary | std::ios::ate);
    if (!file.is_open()) {
        if (outError) *outError = "打开文件失败";
        return false;
    }

    const std::streamsize fileSize = file.tellg();
    if (fileSize <= 0) {
        if (outError) *outError = "文件为空";
        return false;
    }
    file.seekg(0, std::ios::beg);
    std::vector<uint8_t> bytes(static_cast<size_t>(fileSize));
    file.read(reinterpret_cast<char*>(bytes.data()), fileSize);
    if (!file.good() && !file.eof()) {
        if (outError) *outError = "读取文件失败";
        return false;
    }
    file.close();

    if (bytes.empty()) {
        if (outError) *outError = "文件为空";
        return false;
    }

    const std::string ext          = normalize_format(extension_from_path(path));
    const bool        looksLikeBlp = bytes.size() >= BLP_MAGIC_BYTES &&
                                    (std::memcmp(bytes.data(), "BLP1", BLP_MAGIC_BYTES) == 0 ||
                                     std::memcmp(bytes.data(), "BLP2", BLP_MAGIC_BYTES) == 0);
    const bool        looksLikeDds = ddscodec::is_dds(bytes.data(), bytes.size());

    if (looksLikeBlp || (ext == "blp" && !looksLikeDds)) {
        std::string decodeError;
        auto decoded = blpcodec::decode(bytes.data(), bytes.size(), &decodeError);
        if (!decoded) {
            if (outError) {
                *outError = "BLP 解码失败" + (decodeError.empty() ? "" : "：" + decodeError);
            }
            return false;
        }

        outImage->width  = static_cast<int>(decoded->width);
        outImage->height = static_cast<int>(decoded->height);
        outImage->pixels = std::move(decoded->rgba);
    } else if (ext == "dds" || looksLikeDds) {
        std::string decodeError;
        auto decoded = ddscodec::decode(bytes.data(), bytes.size(), &decodeError);
        if (!decoded) {
            if (outError) {
                *outError = "DDS 解码失败" + (decodeError.empty() ? "" : "：" + decodeError);
            }
            return false;
        }

        outImage->width  = static_cast<int>(decoded->width);
        outImage->height = static_cast<int>(decoded->height);
        outImage->pixels = std::move(decoded->rgba);
    } else {
        int width = 0, height = 0, comp = 0;
        auto pixels = std::unique_ptr<stbi_uc, decltype(&stbi_image_free)>(
            stbi_load_from_memory(bytes.data(), static_cast<int>(bytes.size()),
                                  &width, &height, &comp, RGBA_CHANNELS),
            stbi_image_free);
        if (!pixels) {
            if (outError) {
                const char* reason = stbi_failure_reason();
                *outError = std::string("图像解码失败") +
                            (reason ? std::string("：") + reason : "");
            }
            return false;
        }

        outImage->width  = width;
        outImage->height = height;
        outImage->pixels.assign(pixels.get(),
                                pixels.get() + width * height * RGBA_CHANNELS);
    }

    if (outMeta) {
        outMeta->width    = outImage->width;
        outMeta->height   = outImage->height;
        outMeta->format   = looksLikeBlp ? "blp" : (looksLikeDds ? "dds" : ext);
        outMeta->fileSize = static_cast<uint64_t>(fileSize);
    }

    return true;
}

bool write_image_file(const std::string& outputPath,
                      const std::string& format,
                      const RgbaImage& image,
                      int quality,
                      int mipCount,
                      std::string* outError,
                      BlpApi* blpApi) {
    const std::string fmt = normalize_format(format);
    namespace fs = std::filesystem;
    const fs::path outFsPath = fs_path_from_utf8(outputPath);
    const fs::path outParent = outFsPath.parent_path();
    if (!outParent.empty()) {
        fs::create_directories(outParent);
    }

    std::vector<uint8_t> encoded;

    if (fmt == "blp") {
        if (image.width <= 0 || image.height <= 0 || image.pixels.empty()) {
            if (outError) *outError = "无效的图像数据";
            return false;
        }
        if (!blpcodec::encode_jpeg_blp1(image.pixels.data(),
                                        static_cast<uint32_t>(image.width),
                                        static_cast<uint32_t>(image.height),
                                        quality, mipCount, encoded, outError)) {
            return false;
        }
    } else if (fmt == "dds") {
        if (image.width <= 0 || image.height <= 0 || image.pixels.empty()) {
            if (outError) *outError = "无效的图像数据";
            return false;
        }
        if (!ddscodec::encode_bgra8(image.pixels.data(),
                                    static_cast<uint32_t>(image.width),
                                    static_cast<uint32_t>(image.height),
                                    encoded, outError)) {
            return false;
        }
    } else if (!encode_to_bytes(image, fmt, quality, &encoded, outError)) {
        return false;
    }

    std::ofstream file(outFsPath, std::ios::binary | std::ios::trunc);
    if (!file.is_open()) {
        if (outError) *outError = "打开输出文件失败";
        return false;
    }

    file.write(reinterpret_cast<const char*>(encoded.data()),
               static_cast<std::streamsize>(encoded.size()));
    if (!file.good()) {
        if (outError) *outError = "写入输出文件失败";
        return false;
    }

    return true;
}
