---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/blp/blp_codec.cpp"
ref: HEAD
---

// BLP1 codec implementation.
// Decode ported from ShadowEngine (src/parser/BlpDecoder.cpp) with corrected bounds checks;
// encode is a new implementation: JPEG-content BLP1 with each mip as a self-contained
// complete JPEG (jpegHeaderSize = 0).
#include "blp_codec.h"

#include <algorithm>
#include <cstring>

#include <turbojpeg.h>

#define STB_IMAGE_RESIZE_IMPLEMENTATION
#include "stb_image_resize2.h"

namespace blpcodec {

namespace {

constexpr uint32_t MAGIC_BLP1        = 0x31504C42; // 'BLP1'
constexpr uint32_t MAGIC_BLP2        = 0x32504C42; // 'BLP2'
constexpr size_t   HEADER_SIZE       = 156;         // fixed header + 16 mip offset/size pairs
constexpr uint32_t COMPRESSION_JPEG     = 0;
constexpr uint32_t COMPRESSION_PALETTED = 1;

// ---------------------------------------------------------------------------
// RAII wrappers for libturbojpeg handles and buffers
// ---------------------------------------------------------------------------

// Wraps a TurboJPEG compressor handle (new tj3 API).
struct TjCompressor {
    tjhandle h;
    explicit TjCompressor() : h(tj3Init(TJINIT_COMPRESS)) {}
    ~TjCompressor() { if (h) tj3Destroy(h); }
    operator tjhandle() const { return h; }
    bool ok() const { return h != nullptr; }
    TjCompressor(const TjCompressor&)            = delete;
    TjCompressor& operator=(const TjCompressor&) = delete;
};

// Wraps a TurboJPEG decompressor handle (classic API — tjInitDecompress/tjDestroy).
struct TjDecompressor {
    tjhandle h;
    explicit TjDecompressor() : h(tjInitDecompress()) {}
    ~TjDecompressor() { if (h) tjDestroy(h); }
    operator tjhandle() const { return h; }
    bool ok() const { return h != nullptr; }
    TjDecompressor(const TjDecompressor&)            = delete;
    TjDecompressor& operator=(const TjDecompressor&) = delete;
};

// Owns a buffer allocated by tj3Compress8 / tj3DecompressToYUVPlanes, etc.
struct TjBuffer {
    unsigned char* data = nullptr;
    size_t         size = 0;
    TjBuffer() = default;
    ~TjBuffer() { if (data) tj3Free(data); }
    TjBuffer(const TjBuffer&)            = delete;
    TjBuffer& operator=(const TjBuffer&) = delete;
};

// ---------------------------------------------------------------------------
// BLP header
// ---------------------------------------------------------------------------

struct BlpHeader {
    uint32_t magic       = 0;
    uint32_t compression = 0;
    uint32_t alphaBits   = 0;
    uint32_t width       = 0;
    uint32_t height      = 0;
    uint32_t extra       = 0;
    uint32_t hasMipmaps  = 0;
    uint32_t mipmapOffsets[MAX_MIPS] = {};
    uint32_t mipmapSizes[MAX_MIPS]   = {};
};

// ---------------------------------------------------------------------------
// Small utilities
// ---------------------------------------------------------------------------

void set_error(std::string* outError, const std::string& message) {
    if (outError) *outError = message;
}

uint32_t read_u32(const uint8_t* p) {
    uint32_t v;
    std::memcpy(&v, p, sizeof(v));
    return v;
}

void write_u32(std::vector<uint8_t>& out, size_t pos, uint32_t value) {
    std::memcpy(out.data() + pos, &value, sizeof(value));
}

// ---------------------------------------------------------------------------
// Header / segment parsing
// ---------------------------------------------------------------------------

bool parse_header(const uint8_t* data, size_t size,
                  BlpHeader& header, std::string* outError) {
    if (!data || size < HEADER_SIZE) {
        set_error(outError, "文件太小，不是有效的 BLP 文件");
        return false;
    }

    header.magic = read_u32(data);
    if (header.magic == MAGIC_BLP2) {
        set_error(outError, "不支持 BLP2 格式（仅支持 War3 的 BLP1）");
        return false;
    }
    if (header.magic != MAGIC_BLP1) {
        set_error(outError, "无效的 BLP 文件头");
        return false;
    }

    header.compression = read_u32(data + 4);
    header.alphaBits   = read_u32(data + 8);
    header.width       = read_u32(data + 12);
    header.height      = read_u32(data + 16);
    header.extra       = read_u32(data + 20);
    header.hasMipmaps  = read_u32(data + 24);

    for (int i = 0; i < MAX_MIPS; ++i) {
        header.mipmapOffsets[i] = read_u32(data + 28 + i * 4);
        header.mipmapSizes[i]   = read_u32(data + 28 + 64 + i * 4);
    }

    return true;
}

// Validates the mip data segment lies within the file and returns a pointer to it.
const uint8_t* mip_segment(const uint8_t* data, size_t size,
                            const BlpHeader& header, uint32_t mipLevel,
                            uint32_t* outMipSize, std::string* outError) {
    const uint32_t offset  = header.mipmapOffsets[mipLevel];
    const uint32_t mipSize = header.mipmapSizes[mipLevel];

    if (offset == 0 || mipSize == 0) {
        set_error(outError, "无效的 mipmap 级别");
        return nullptr;
    }
    if (static_cast<uint64_t>(offset) + mipSize > size) {
        set_error(outError, "Mipmap 数据超出文件范围");
        return nullptr;
    }

    *outMipSize = mipSize;
    return data + offset;
}

// ---------------------------------------------------------------------------
// Decoders
// ---------------------------------------------------------------------------

std::optional<RawImage> decode_paletted(const uint8_t* data, size_t size,
                                        const BlpHeader& header,
                                        uint32_t mipLevel, std::string* outError) {
    constexpr size_t PALETTE_OFFSET = HEADER_SIZE;
    constexpr size_t PALETTE_SIZE   = 256 * 4;

    if (size < PALETTE_OFFSET + PALETTE_SIZE) {
        set_error(outError, "文件太小，无法读取调色板");
        return std::nullopt;
    }
    const uint8_t* palette = data + PALETTE_OFFSET; // 256 entries, BGRA

    const uint32_t width  = std::max(1u, header.width  >> mipLevel);
    const uint32_t height = std::max(1u, header.height >> mipLevel);
    const size_t pixelCount = static_cast<size_t>(width) * height;

    uint32_t mipSize = 0;
    const uint8_t* indexData = mip_segment(data, size, header, mipLevel, &mipSize, outError);
    if (!indexData) return std::nullopt;

    if (mipSize < pixelCount) {
        set_error(outError, "Mipmap 数据不足以容纳索引数据");
        return std::nullopt;
    }

    RawImage image;
    image.width  = width;
    image.height = height;
    image.rgba.resize(pixelCount * 4);

    for (size_t i = 0; i < pixelCount; ++i) {
        const uint8_t* color = palette + indexData[i] * 4;
        image.rgba[i * 4 + 0] = color[2]; // R
        image.rgba[i * 4 + 1] = color[1]; // G
        image.rgba[i * 4 + 2] = color[0]; // B
        image.rgba[i * 4 + 3] = 255;
    }

    if (header.alphaBits > 0) {
        // Alpha data follows the index data within the same mip segment.
        const uint8_t* alphaData = indexData + pixelCount;
        const size_t remaining   = mipSize - pixelCount;

        if (header.alphaBits == 8 && remaining >= pixelCount) {
            for (size_t i = 0; i < pixelCount; ++i) {
                image.rgba[i * 4 + 3] = alphaData[i];
            }
        } else if (header.alphaBits == 4 && remaining >= (pixelCount + 1) / 2) {
            for (size_t i = 0; i < pixelCount; ++i) {
                const uint8_t alphaByte = alphaData[i / 2];
                const uint8_t alpha4    = (i % 2 == 0) ? (alphaByte & 0x0F) : (alphaByte >> 4);
                image.rgba[i * 4 + 3]  = static_cast<uint8_t>(alpha4 * 17);
            }
        } else if (header.alphaBits == 1 && remaining >= (pixelCount + 7) / 8) {
            for (size_t i = 0; i < pixelCount; ++i) {
                const uint8_t alphaByte = alphaData[i / 8];
                image.rgba[i * 4 + 3]  = ((alphaByte >> (i % 8)) & 1) ? 255 : 0;
            }
        }
    }

    return image;
}

std::optional<RawImage> decode_jpeg(const uint8_t* data, size_t size,
                                    const BlpHeader& header,
                                    uint32_t mipLevel, std::string* outError) {
    // JPEG-content BLP1 layout: after the fixed header sits a uint32 giving the
    // size of the shared JPEG header, followed by that many header bytes.  Each
    // mip segment contains [shared-header || mip scan data], forming a complete JPEG.
    if (size < HEADER_SIZE + 4) {
        set_error(outError, "文件太小，无法读取 JPEG 头部大小");
        return std::nullopt;
    }

    const uint32_t jpegHeaderSize = read_u32(data + HEADER_SIZE);
    if (static_cast<uint64_t>(HEADER_SIZE) + 4 + jpegHeaderSize > size) {
        set_error(outError, "文件太小，无法读取 JPEG 头部");
        return std::nullopt;
    }
    const uint8_t* jpegHeader = data + HEADER_SIZE + 4;

    uint32_t mipSize = 0;
    const uint8_t* mipData = mip_segment(data, size, header, mipLevel, &mipSize, outError);
    if (!mipData) return std::nullopt;

    std::vector<uint8_t> jpegData;
    jpegData.reserve(jpegHeaderSize + mipSize);
    jpegData.insert(jpegData.end(), jpegHeader, jpegHeader + jpegHeaderSize);
    jpegData.insert(jpegData.end(), mipData, mipData + mipSize);

    TjDecompressor tj;
    if (!tj.ok()) {
        set_error(outError, "turbojpeg 初始化失败");
        return std::nullopt;
    }

    int imgWidth = 0, imgHeight = 0, jpegSubsamp = 0, jpegColorspace = 0;
    if (tjDecompressHeader3(static_cast<tjhandle>(tj),
                            jpegData.data(),
                            static_cast<unsigned long>(jpegData.size()),
                            &imgWidth, &imgHeight,
                            &jpegSubsamp, &jpegColorspace) != 0) {
        set_error(outError, std::string("turbojpeg 头部解析失败: ") + tjGetErrorStr());
        return std::nullopt;
    }

    RawImage image;
    image.width  = static_cast<uint32_t>(imgWidth);
    image.height = static_cast<uint32_t>(imgHeight);
    const size_t pixelCount = static_cast<size_t>(imgWidth) * imgHeight;
    image.rgba.resize(pixelCount * 4);

    // BLP1/JPEG pixel semantics aligned with War3 / legacy blp crate behaviour:
    //   4-component (standard War3 texture): use CMYK/YCCK pass-through to obtain
    //     four raw components, then reinterpret as BGRA.
    //   3-component (third-party tools whose component IDs mimic 'R','G','B'): read
    //     as BGR, no alpha channel.
    //   1-component: greyscale, alpha = 255.
    if (jpegColorspace == TJCS_CMYK || jpegColorspace == TJCS_YCCK) {
        if (tjDecompress2(static_cast<tjhandle>(tj),
                          jpegData.data(),
                          static_cast<unsigned long>(jpegData.size()),
                          image.rgba.data(), imgWidth, 0, imgHeight,
                          TJPF_CMYK, 0) != 0) {
            set_error(outError, std::string("turbojpeg 解码失败: ") + tjGetErrorStr());
            return std::nullopt;
        }
        // BGRA → RGBA channel swap
        for (size_t i = 0; i < pixelCount; ++i) {
            std::swap(image.rgba[i * 4 + 0], image.rgba[i * 4 + 2]);
        }
    } else if (jpegColorspace == TJCS_GRAY) {
        std::vector<uint8_t> gray(pixelCount);
        if (tjDecompress2(static_cast<tjhandle>(tj),
                          jpegData.data(),
                          static_cast<unsigned long>(jpegData.size()),
                          gray.data(), imgWidth, 0, imgHeight,
                          TJPF_GRAY, 0) != 0) {
            set_error(outError, std::string("turbojpeg 解码失败: ") + tjGetErrorStr());
            return std::nullopt;
        }
        for (size_t i = 0; i < pixelCount; ++i) {
            image.rgba[i * 4 + 0] = gray[i];
            image.rgba[i * 4 + 1] = gray[i];
            image.rgba[i * 4 + 2] = gray[i];
            image.rgba[i * 4 + 3] = 255;
        }
    } else {
        // TJCS_RGB (pass-through) or TJCS_YCbCr (converted): War3 interprets both as BGR.
        std::vector<uint8_t> bgr(pixelCount * 3);
        if (tjDecompress2(static_cast<tjhandle>(tj),
                          jpegData.data(),
                          static_cast<unsigned long>(jpegData.size()),
                          bgr.data(), imgWidth, 0, imgHeight,
                          TJPF_RGB, 0) != 0) {
            set_error(outError, std::string("turbojpeg 解码失败: ") + tjGetErrorStr());
            return std::nullopt;
        }
        for (size_t i = 0; i < pixelCount; ++i) {
            image.rgba[i * 4 + 0] = bgr[i * 3 + 2]; // R
            image.rgba[i * 4 + 1] = bgr[i * 3 + 1]; // G
            image.rgba[i * 4 + 2] = bgr[i * 3 + 0]; // B
            image.rgba[i * 4 + 3] = 255;
        }
    }

    return image;
}

} // namespace

// ---------------------------------------------------------------------------
// Public API
// ---------------------------------------------------------------------------

bool is_blp1(const uint8_t* data, size_t size) {
    return data && size >= 4 && read_u32(data) == MAGIC_BLP1;
}

bool is_blp_magic(const uint8_t* data, size_t size) {
    if (!data || size < 4) return false;
    const uint32_t magic = read_u32(data);
    return magic == MAGIC_BLP1 || magic == MAGIC_BLP2;
}

std::optional<RawImage> decode_mip(const uint8_t* data, size_t size,
                                   uint32_t mipLevel, std::string* outError) {
    if (mipLevel >= static_cast<uint32_t>(MAX_MIPS)) {
        set_error(outError, "mipmap 级别超出范围");
        return std::nullopt;
    }

    BlpHeader header;
    if (!parse_header(data, size, header, outError)) return std::nullopt;

    if (header.compression == COMPRESSION_PALETTED)
        return decode_paletted(data, size, header, mipLevel, outError);
    if (header.compression == COMPRESSION_JPEG)
        return decode_jpeg(data, size, header, mipLevel, outError);

    set_error(outError, "不支持的 BLP 压缩类型: " + std::to_string(header.compression));
    return std::nullopt;
}

std::optional<RawImage> decode(const uint8_t* data, size_t size, std::string* outError) {
    BlpHeader header;
    if (!parse_header(data, size, header, outError)) return std::nullopt;

    // Iterate from level 0 upward, tolerating individual corrupt mip entries.
    std::string firstError;
    bool tried = false;
    for (uint32_t level = 0; level < static_cast<uint32_t>(MAX_MIPS); ++level) {
        if (header.mipmapOffsets[level] == 0 || header.mipmapSizes[level] == 0) continue;
        std::string levelError;
        auto image = decode_mip(data, size, level, &levelError);
        if (image) return image;
        if (!tried) {
            firstError = levelError;
            tried = true;
        }
    }

    if (!tried) {
        // No valid mip entries found; attempt level 0 anyway to surface a specific error.
        return decode_mip(data, size, 0, outError);
    }
    set_error(outError, firstError.empty() ? "BLP 解码失败" : firstError);
    return std::nullopt;
}

bool encode_jpeg_blp1(const uint8_t* rgba, uint32_t width, uint32_t height,
                      int quality, int mipCount,
                      std::vector<uint8_t>& outBlp, std::string* outError) {
    outBlp.clear();

    if (!rgba || width == 0 || height == 0) {
        set_error(outError, "无效的图像数据");
        return false;
    }

    // Compute the full mip chain length (down to 1×1), then clamp to the caller's request.
    int fullLevels = 1;
    while ((std::max(width, height) >> fullLevels) >= 1 && fullLevels < MAX_MIPS) {
        ++fullLevels;
    }
    const int levels    = std::clamp(mipCount, 1, fullLevels);
    const int tjQuality = std::clamp(quality, 1, 100);

    TjCompressor tj;
    if (!tj.ok()) {
        set_error(outError, "turbojpeg 初始化失败");
        return false;
    }

    if (tj3Set(static_cast<tjhandle>(tj), TJPARAM_QUALITY,     tjQuality)  != 0 ||
        tj3Set(static_cast<tjhandle>(tj), TJPARAM_SUBSAMP,     TJSAMP_444) != 0 ||
        tj3Set(static_cast<tjhandle>(tj), TJPARAM_COLORSPACE,  TJCS_CMYK)  != 0 ||
        tj3Set(static_cast<tjhandle>(tj), TJPARAM_FASTDCT,     0)          != 0) {
        set_error(outError, std::string("turbojpeg 参数设置失败: ") +
                            tj3GetErrorStr(static_cast<tjhandle>(tj)));
        return false;
    }

    std::vector<std::vector<uint8_t>> mipJpegs;
    mipJpegs.reserve(levels);
    std::vector<uint8_t> levelRgba;
    std::vector<uint8_t> bgra;

    bool ok = true;
    for (int level = 0; level < levels && ok; ++level) {
        const uint32_t mipW     = std::max(1u, width  >> level);
        const uint32_t mipH     = std::max(1u, height >> level);
        const size_t pixelCount = static_cast<size_t>(mipW) * mipH;

        const uint8_t* src = rgba;
        if (level > 0) {
            // Always rescale from the original to avoid cumulative error;
            // STBIR_RGBA uses alpha-weighted sampling to prevent colour bleeding
            // at transparent edges.
            levelRgba.resize(pixelCount * 4);
            if (!stbir_resize_uint8_srgb(rgba,
                                         static_cast<int>(width),
                                         static_cast<int>(height), 0,
                                         levelRgba.data(),
                                         static_cast<int>(mipW),
                                         static_cast<int>(mipH), 0,
                                         STBIR_RGBA)) {
                set_error(outError, "mipmap 缩放失败");
                ok = false;
                break;
            }
            src = levelRgba.data();
        }

        // War3 BLP1 JPEG-content stores pixels as B/G/R/A in the four JPEG
        // components.  No pre-multiplication or inversion; alpha is the 4th component.
        bgra.resize(pixelCount * 4);
        for (size_t i = 0; i < pixelCount; ++i) {
            bgra[i * 4 + 0] = src[i * 4 + 2]; // B
            bgra[i * 4 + 1] = src[i * 4 + 1]; // G
            bgra[i * 4 + 2] = src[i * 4 + 0]; // R
            bgra[i * 4 + 3] = src[i * 4 + 3]; // A
        }

        TjBuffer jpegBuf;
        if (tj3Compress8(static_cast<tjhandle>(tj),
                         bgra.data(),
                         static_cast<int>(mipW), 0, static_cast<int>(mipH),
                         TJPF_CMYK, &jpegBuf.data, &jpegBuf.size) != 0) {
            set_error(outError, std::string("turbojpeg 编码失败: ") +
                                tj3GetErrorStr(static_cast<tjhandle>(tj)));
            ok = false;
        } else {
            mipJpegs.emplace_back(jpegBuf.data, jpegBuf.data + jpegBuf.size);
            // TjBuffer destructor frees jpegBuf.data — no manual tj3Free needed.
        }
    }
    if (!ok) return false;

    // Assemble output: 156-byte header + jpegHeaderSize(=0) + per-mip complete JPEGs.
    size_t totalSize = HEADER_SIZE + 4;
    for (const auto& jpeg : mipJpegs) totalSize += jpeg.size();
    outBlp.resize(totalSize, 0);

    write_u32(outBlp,  0, MAGIC_BLP1);
    write_u32(outBlp,  4, COMPRESSION_JPEG);
    write_u32(outBlp,  8, 8);  // alphaBits
    write_u32(outBlp, 12, width);
    write_u32(outBlp, 16, height);
    write_u32(outBlp, 20, 4);  // extra (matches original Blizzard files)
    write_u32(outBlp, 24, levels > 1 ? 1u : 0u); // hasMipmaps

    size_t cursor = HEADER_SIZE;
    write_u32(outBlp, cursor, 0); // jpegHeaderSize = 0: each mip is a self-contained JPEG
    cursor += 4;

    for (int level = 0; level < levels; ++level) {
        const auto& jpeg = mipJpegs[static_cast<size_t>(level)];
        write_u32(outBlp, 28 + static_cast<size_t>(level) * 4,
                  static_cast<uint32_t>(cursor));
        write_u32(outBlp, 28 + 64 + static_cast<size_t>(level) * 4,
                  static_cast<uint32_t>(jpeg.size()));
        std::memcpy(outBlp.data() + cursor, jpeg.data(), jpeg.size());
        cursor += jpeg.size();
    }

    return true;
}

} // namespace blpcodec
