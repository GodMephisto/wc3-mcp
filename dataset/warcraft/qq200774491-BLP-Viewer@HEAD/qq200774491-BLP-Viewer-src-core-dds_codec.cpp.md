---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/dds_codec.cpp"
ref: HEAD
---

#include "dds_codec.h"

#include <algorithm>
#include <array>
#include <climits>
#include <cstring>
#include <limits>

namespace ddscodec {
namespace {

constexpr size_t DDS_HEADER_BYTES = 128;
constexpr size_t DDS_DX10_HEADER_BYTES = 20;
constexpr uint64_t MAX_DECODED_RGBA_BYTES = 1ULL << 30;
constexpr uint32_t DDS_MAGIC = 0x20534444; // "DDS "

constexpr uint32_t DDPF_ALPHAPIXELS = 0x00000001;
constexpr uint32_t DDPF_ALPHA       = 0x00000002;
constexpr uint32_t DDPF_FOURCC      = 0x00000004;
constexpr uint32_t DDPF_RGB         = 0x00000040;
constexpr uint32_t DDPF_LUMINANCE   = 0x00020000;
constexpr uint32_t DDSD_PITCH       = 0x00000008;

constexpr uint32_t make_fourcc(char a, char b, char c, char d) {
    return static_cast<uint32_t>(static_cast<uint8_t>(a)) |
           (static_cast<uint32_t>(static_cast<uint8_t>(b)) << 8) |
           (static_cast<uint32_t>(static_cast<uint8_t>(c)) << 16) |
           (static_cast<uint32_t>(static_cast<uint8_t>(d)) << 24);
}

constexpr uint32_t FOURCC_DXT1 = make_fourcc('D', 'X', 'T', '1');
constexpr uint32_t FOURCC_DXT3 = make_fourcc('D', 'X', 'T', '3');
constexpr uint32_t FOURCC_DXT5 = make_fourcc('D', 'X', 'T', '5');
constexpr uint32_t FOURCC_ATI1 = make_fourcc('A', 'T', 'I', '1');
constexpr uint32_t FOURCC_ATI2 = make_fourcc('A', 'T', 'I', '2');
constexpr uint32_t FOURCC_BC4U = make_fourcc('B', 'C', '4', 'U');
constexpr uint32_t FOURCC_BC4S = make_fourcc('B', 'C', '4', 'S');
constexpr uint32_t FOURCC_BC5U = make_fourcc('B', 'C', '5', 'U');
constexpr uint32_t FOURCC_BC5S = make_fourcc('B', 'C', '5', 'S');
constexpr uint32_t FOURCC_DX10 = make_fourcc('D', 'X', '1', '0');

enum class Encoding {
    Masked,
    R8,
    RG8,
    Rgba8,
    Bgra8,
    Bgrx8,
    B5G6R5,
    B5G5R5A1,
    BC1,
    BC2,
    BC3,
    BC4U,
    BC4S,
    BC5U,
    BC5S,
};

struct FormatInfo {
    Encoding encoding = Encoding::Masked;
    uint32_t bitCount = 0;
    uint32_t rMask = 0;
    uint32_t gMask = 0;
    uint32_t bMask = 0;
    uint32_t aMask = 0;
    bool luminance = false;
    bool alphaOnly = false;
};

void set_error(std::string* outError, const std::string& message) {
    if (outError) *outError = message;
}

uint32_t read_u32(const uint8_t* data) {
    return static_cast<uint32_t>(data[0]) |
           (static_cast<uint32_t>(data[1]) << 8) |
           (static_cast<uint32_t>(data[2]) << 16) |
           (static_cast<uint32_t>(data[3]) << 24);
}

uint16_t read_u16(const uint8_t* data) {
    return static_cast<uint16_t>(data[0]) |
           static_cast<uint16_t>(static_cast<uint16_t>(data[1]) << 8);
}

void write_u32(std::vector<uint8_t>& out, size_t offset, uint32_t value) {
    out[offset + 0] = static_cast<uint8_t>(value);
    out[offset + 1] = static_cast<uint8_t>(value >> 8);
    out[offset + 2] = static_cast<uint8_t>(value >> 16);
    out[offset + 3] = static_cast<uint8_t>(value >> 24);
}

bool validate_dimensions(uint32_t width, uint32_t height, size_t* pixelCount,
                         std::string* outError) {
    if (width == 0 || height == 0 || width > static_cast<uint32_t>(INT_MAX) ||
        height > static_cast<uint32_t>(INT_MAX)) {
        set_error(outError, "DDS 图像尺寸无效");
        return false;
    }
    const uint64_t count = static_cast<uint64_t>(width) * height;
    if (count > std::numeric_limits<size_t>::max() / 4 ||
        count * 4 > MAX_DECODED_RGBA_BYTES) {
        set_error(outError, "DDS 图像尺寸过大");
        return false;
    }
    *pixelCount = static_cast<size_t>(count);
    return true;
}

uint8_t expand_mask(uint32_t value, uint32_t mask, uint8_t fallback) {
    if (mask == 0) return fallback;
    unsigned shift = 0;
    while (((mask >> shift) & 1U) == 0U && shift < 31) ++shift;
    const uint32_t field = (value & mask) >> shift;
    const uint32_t maximum = mask >> shift;
    if (maximum == 0) return fallback;
    return static_cast<uint8_t>((static_cast<uint64_t>(field) * 255 + maximum / 2) / maximum);
}

std::array<uint8_t, 4> color_565(uint16_t color) {
    const uint8_t r5 = static_cast<uint8_t>((color >> 11) & 31);
    const uint8_t g6 = static_cast<uint8_t>((color >> 5) & 63);
    const uint8_t b5 = static_cast<uint8_t>(color & 31);
    return {static_cast<uint8_t>((r5 << 3) | (r5 >> 2)),
            static_cast<uint8_t>((g6 << 2) | (g6 >> 4)),
            static_cast<uint8_t>((b5 << 3) | (b5 >> 2)), 255};
}

void build_bc_colors(const uint8_t* block, bool allowTransparent,
                     std::array<std::array<uint8_t, 4>, 4>& colors) {
    const uint16_t c0 = read_u16(block);
    const uint16_t c1 = read_u16(block + 2);
    colors[0] = color_565(c0);
    colors[1] = color_565(c1);
    if (!allowTransparent || c0 > c1) {
        for (int channel = 0; channel < 3; ++channel) {
            colors[2][channel] = static_cast<uint8_t>((2 * colors[0][channel] + colors[1][channel]) / 3);
            colors[3][channel] = static_cast<uint8_t>((colors[0][channel] + 2 * colors[1][channel]) / 3);
        }
        colors[2][3] = colors[3][3] = 255;
    } else {
        for (int channel = 0; channel < 3; ++channel)
            colors[2][channel] = static_cast<uint8_t>((colors[0][channel] + colors[1][channel]) / 2);
        colors[2][3] = 255;
        colors[3] = {0, 0, 0, 0};
    }
}

void store_pixel(RawImage& image, uint32_t x, uint32_t y,
                 const std::array<uint8_t, 4>& color) {
    if (x >= image.width || y >= image.height) return;
    const size_t offset = (static_cast<size_t>(y) * image.width + x) * 4;
    std::copy(color.begin(), color.end(), image.rgba.begin() + offset);
}

void decode_bc_color_block(RawImage& image, uint32_t blockX, uint32_t blockY,
                           const uint8_t* block, bool allowTransparent,
                           const uint8_t* alphaValues) {
    std::array<std::array<uint8_t, 4>, 4> colors{};
    build_bc_colors(block, allowTransparent, colors);
    const uint32_t indices = read_u32(block + 4);
    for (uint32_t py = 0; py < 4; ++py) {
        for (uint32_t px = 0; px < 4; ++px) {
            const uint32_t pixel = py * 4 + px;
            auto color = colors[(indices >> (pixel * 2)) & 3U];
            if (alphaValues) color[3] = alphaValues[pixel];
            store_pixel(image, blockX * 4 + px, blockY * 4 + py, color);
        }
    }
}

uint8_t snorm_to_byte(int value) {
    value = std::clamp(value, -127, 127);
    return static_cast<uint8_t>(((value + 127) * 255 + 127) / 254);
}

void decode_bc4_values(const uint8_t* block, bool signedValues, uint8_t values[16]) {
    int table[8] = {};
    if (signedValues) {
        table[0] = std::max(-127, static_cast<int>(static_cast<int8_t>(block[0])));
        table[1] = std::max(-127, static_cast<int>(static_cast<int8_t>(block[1])));
        if (table[0] > table[1]) {
            for (int i = 1; i <= 6; ++i) table[i + 1] = ((7 - i) * table[0] + i * table[1]) / 7;
        } else {
            for (int i = 1; i <= 4; ++i) table[i + 1] = ((5 - i) * table[0] + i * table[1]) / 5;
            table[6] = -127;
            table[7] = 127;
        }
    } else {
        table[0] = block[0];
        table[1] = block[1];
        if (table[0] > table[1]) {
            for (int i = 1; i <= 6; ++i) table[i + 1] = ((7 - i) * table[0] + i * table[1]) / 7;
        } else {
            for (int i = 1; i <= 4; ++i) table[i + 1] = ((5 - i) * table[0] + i * table[1]) / 5;
            table[6] = 0;
            table[7] = 255;
        }
    }

    uint64_t indices = 0;
    for (int i = 0; i < 6; ++i) indices |= static_cast<uint64_t>(block[2 + i]) << (i * 8);
    for (int i = 0; i < 16; ++i) {
        const int value = table[(indices >> (i * 3)) & 7U];
        values[i] = signedValues ? snorm_to_byte(value) : static_cast<uint8_t>(value);
    }
}

bool decode_blocks(const uint8_t* src, size_t available, FormatInfo format,
                   RawImage& image, std::string* outError) {
    const uint32_t blocksWide = (image.width + 3) / 4;
    const uint32_t blocksHigh = (image.height + 3) / 4;
    const bool eightByteBlock = format.encoding == Encoding::BC1 ||
                                format.encoding == Encoding::BC4U ||
                                format.encoding == Encoding::BC4S;
    const size_t blockBytes = eightByteBlock ? 8 : 16;
    const uint64_t required = static_cast<uint64_t>(blocksWide) * blocksHigh * blockBytes;
    if (required > available) {
        set_error(outError, "DDS 压缩像素数据不完整");
        return false;
    }

    for (uint32_t by = 0; by < blocksHigh; ++by) {
        for (uint32_t bx = 0; bx < blocksWide; ++bx) {
            const uint8_t* block = src + (static_cast<size_t>(by) * blocksWide + bx) * blockBytes;
            if (format.encoding == Encoding::BC1) {
                decode_bc_color_block(image, bx, by, block, true, nullptr);
            } else if (format.encoding == Encoding::BC2) {
                uint8_t alpha[16];
                for (int i = 0; i < 16; ++i) {
                    const uint8_t nibble = static_cast<uint8_t>((block[i / 2] >> ((i & 1) * 4)) & 0x0f);
                    alpha[i] = static_cast<uint8_t>(nibble * 17);
                }
                decode_bc_color_block(image, bx, by, block + 8, false, alpha);
            } else if (format.encoding == Encoding::BC3) {
                uint8_t alpha[16];
                decode_bc4_values(block, false, alpha);
                decode_bc_color_block(image, bx, by, block + 8, false, alpha);
            } else {
                const bool signedValues = format.encoding == Encoding::BC4S ||
                                          format.encoding == Encoding::BC5S;
                uint8_t red[16];
                decode_bc4_values(block, signedValues, red);
                uint8_t green[16] = {};
                const bool twoChannels = format.encoding == Encoding::BC5U ||
                                         format.encoding == Encoding::BC5S;
                if (twoChannels) decode_bc4_values(block + 8, signedValues, green);
                for (uint32_t py = 0; py < 4; ++py) {
                    for (uint32_t px = 0; px < 4; ++px) {
                        const uint32_t i = py * 4 + px;
                        const std::array<uint8_t, 4> color = twoChannels
                            ? std::array<uint8_t, 4>{red[i], green[i], 0, 255}
                            : std::array<uint8_t, 4>{red[i], red[i], red[i], 255};
                        store_pixel(image, bx * 4 + px, by * 4 + py, color);
                    }
                }
            }
        }
    }
    return true;
}

bool decode_uncompressed(const uint8_t* src, size_t available, uint32_t pitchFromHeader,
                         uint32_t headerFlags, const FormatInfo& format,
                         RawImage& image, std::string* outError) {
    uint32_t bytesPerPixel = 0;
    switch (format.encoding) {
        case Encoding::R8: bytesPerPixel = 1; break;
        case Encoding::RG8: bytesPerPixel = 2; break;
        case Encoding::Rgba8:
        case Encoding::Bgra8:
        case Encoding::Bgrx8: bytesPerPixel = 4; break;
        case Encoding::B5G6R5:
        case Encoding::B5G5R5A1: bytesPerPixel = 2; break;
        default:
            if (format.bitCount == 0 || format.bitCount > 32 || (format.bitCount % 8) != 0) {
                set_error(outError, "不支持的 DDS 未压缩像素位数");
                return false;
            }
            bytesPerPixel = format.bitCount / 8;
            break;
    }

    const uint64_t minimumPitch64 = static_cast<uint64_t>(image.width) * bytesPerPixel;
    if (minimumPitch64 > std::numeric_limits<uint32_t>::max()) {
        set_error(outError, "DDS 行跨度过大");
        return false;
    }
    const uint32_t minimumPitch = static_cast<uint32_t>(minimumPitch64);
    const uint32_t rowPitch = ((headerFlags & DDSD_PITCH) && pitchFromHeader >= minimumPitch)
        ? pitchFromHeader : minimumPitch;
    if (static_cast<uint64_t>(rowPitch) * image.height > available) {
        set_error(outError, "DDS 像素数据不完整");
        return false;
    }

    for (uint32_t y = 0; y < image.height; ++y) {
        const uint8_t* row = src + static_cast<size_t>(y) * rowPitch;
        for (uint32_t x = 0; x < image.width; ++x) {
            const uint8_t* pixel = row + static_cast<size_t>(x) * bytesPerPixel;
            std::array<uint8_t, 4> color{0, 0, 0, 255};
            if (format.encoding == Encoding::R8) {
                color = {pixel[0], pixel[0], pixel[0], 255};
            } else if (format.encoding == Encoding::RG8) {
                color = {pixel[0], pixel[1], 0, 255};
            } else if (format.encoding == Encoding::Rgba8) {
                color = {pixel[0], pixel[1], pixel[2], pixel[3]};
            } else if (format.encoding == Encoding::Bgra8 || format.encoding == Encoding::Bgrx8) {
                color = {pixel[2], pixel[1], pixel[0],
                         format.encoding == Encoding::Bgra8 ? pixel[3] : static_cast<uint8_t>(255)};
            } else {
                uint32_t value = 0;
                for (uint32_t byte = 0; byte < bytesPerPixel; ++byte)
                    value |= static_cast<uint32_t>(pixel[byte]) << (byte * 8);
                if (format.encoding == Encoding::B5G6R5) {
                    color = {expand_mask(value, 0xf800, 0), expand_mask(value, 0x07e0, 0),
                             expand_mask(value, 0x001f, 0), 255};
                } else if (format.encoding == Encoding::B5G5R5A1) {
                    color = {expand_mask(value, 0x7c00, 0), expand_mask(value, 0x03e0, 0),
                             expand_mask(value, 0x001f, 0), expand_mask(value, 0x8000, 255)};
                } else if (format.alphaOnly) {
                    const uint32_t alphaMask = format.aMask ? format.aMask :
                        (format.bitCount == 32 ? 0xffffffffU : ((1U << format.bitCount) - 1U));
                    color = {255, 255, 255, expand_mask(value, alphaMask, 255)};
                } else {
                    const uint8_t r = expand_mask(value, format.rMask, 0);
                    const uint8_t g = expand_mask(value, format.gMask, format.luminance ? r : 0);
                    const uint8_t b = expand_mask(value, format.bMask, format.luminance ? r : 0);
                    color = {r, g, b, expand_mask(value, format.aMask, 255)};
                }
            }
            store_pixel(image, x, y, color);
        }
    }
    return true;
}

bool is_block_compressed(Encoding encoding) {
    return encoding == Encoding::BC1 || encoding == Encoding::BC2 ||
           encoding == Encoding::BC3 || encoding == Encoding::BC4U ||
           encoding == Encoding::BC4S || encoding == Encoding::BC5U ||
           encoding == Encoding::BC5S;
}

} // namespace

bool is_dds(const uint8_t* data, size_t size) {
    return data && size >= 4 && read_u32(data) == DDS_MAGIC;
}

std::optional<RawImage> decode(const uint8_t* data, size_t size, std::string* outError) {
    if (!is_dds(data, size)) {
        set_error(outError, "不是 DDS 文件");
        return std::nullopt;
    }
    if (size < DDS_HEADER_BYTES || read_u32(data + 4) != 124 || read_u32(data + 76) != 32) {
        set_error(outError, "DDS 头部无效或不完整");
        return std::nullopt;
    }

    const uint32_t headerFlags = read_u32(data + 8);
    const uint32_t height = read_u32(data + 12);
    const uint32_t width = read_u32(data + 16);
    const uint32_t pitch = read_u32(data + 20);
    size_t pixelCount = 0;
    if (!validate_dimensions(width, height, &pixelCount, outError)) return std::nullopt;

    const uint32_t pixelFlags = read_u32(data + 80);
    const uint32_t fourCC = read_u32(data + 84);
    FormatInfo format;
    size_t dataOffset = DDS_HEADER_BYTES;

    if (pixelFlags & DDPF_FOURCC) {
        switch (fourCC) {
            case FOURCC_DXT1: format.encoding = Encoding::BC1; break;
            case FOURCC_DXT3: format.encoding = Encoding::BC2; break;
            case FOURCC_DXT5: format.encoding = Encoding::BC3; break;
            case FOURCC_ATI1:
            case FOURCC_BC4U: format.encoding = Encoding::BC4U; break;
            case FOURCC_BC4S: format.encoding = Encoding::BC4S; break;
            case FOURCC_ATI2:
            case FOURCC_BC5U: format.encoding = Encoding::BC5U; break;
            case FOURCC_BC5S: format.encoding = Encoding::BC5S; break;
            case FOURCC_DX10: {
                if (size < DDS_HEADER_BYTES + DDS_DX10_HEADER_BYTES) {
                    set_error(outError, "DDS DX10 头部不完整");
                    return std::nullopt;
                }
                const uint32_t dxgiFormat = read_u32(data + DDS_HEADER_BYTES);
                const uint32_t resourceDimension = read_u32(data + DDS_HEADER_BYTES + 4);
                const uint32_t arraySize = read_u32(data + DDS_HEADER_BYTES + 12);
                if (resourceDimension != 3 || arraySize == 0) {
                    set_error(outError, "仅支持 DDS 2D 纹理");
                    return std::nullopt;
                }
                switch (dxgiFormat) {
                    case 28: case 29: format.encoding = Encoding::Rgba8; break;
                    case 49: format.encoding = Encoding::RG8; break;
                    case 61: format.encoding = Encoding::R8; break;
                    case 71: case 72: format.encoding = Encoding::BC1; break;
                    case 74: case 75: format.encoding = Encoding::BC2; break;
                    case 77: case 78: format.encoding = Encoding::BC3; break;
                    case 80: format.encoding = Encoding::BC4U; break;
                    case 81: format.encoding = Encoding::BC4S; break;
                    case 83: format.encoding = Encoding::BC5U; break;
                    case 84: format.encoding = Encoding::BC5S; break;
                    case 85: format.encoding = Encoding::B5G6R5; break;
                    case 86: format.encoding = Encoding::B5G5R5A1; break;
                    case 87: case 91: format.encoding = Encoding::Bgra8; break;
                    case 88: case 93: format.encoding = Encoding::Bgrx8; break;
                    default:
                        set_error(outError, "不支持的 DDS DXGI 像素格式：" + std::to_string(dxgiFormat));
                        return std::nullopt;
                }
                dataOffset += DDS_DX10_HEADER_BYTES;
                break;
            }
            default:
                set_error(outError, "不支持的 DDS FourCC 压缩格式");
                return std::nullopt;
        }
    } else if (pixelFlags & (DDPF_RGB | DDPF_LUMINANCE | DDPF_ALPHA | DDPF_ALPHAPIXELS)) {
        format.encoding = Encoding::Masked;
        format.bitCount = read_u32(data + 88);
        format.rMask = read_u32(data + 92);
        format.gMask = read_u32(data + 96);
        format.bMask = read_u32(data + 100);
        format.aMask = read_u32(data + 104);
        format.luminance = (pixelFlags & DDPF_LUMINANCE) != 0;
        format.alphaOnly = (pixelFlags & DDPF_ALPHA) != 0 && (pixelFlags & DDPF_RGB) == 0 &&
                           (pixelFlags & DDPF_LUMINANCE) == 0;
    } else {
        set_error(outError, "不支持的 DDS 像素格式");
        return std::nullopt;
    }

    RawImage image;
    image.width = width;
    image.height = height;
    image.rgba.resize(pixelCount * 4);
    const uint8_t* pixels = data + dataOffset;
    const size_t available = size - dataOffset;
    const bool ok = is_block_compressed(format.encoding)
        ? decode_blocks(pixels, available, format, image, outError)
        : decode_uncompressed(pixels, available, pitch, headerFlags, format, image, outError);
    if (!ok) return std::nullopt;
    return image;
}

bool encode_bgra8(const uint8_t* rgba, uint32_t width, uint32_t height,
                  std::vector<uint8_t>& outDds, std::string* outError) {
    size_t pixelCount = 0;
    if (!rgba) {
        set_error(outError, "DDS 编码输入为空");
        return false;
    }
    if (!validate_dimensions(width, height, &pixelCount, outError)) return false;
    if (pixelCount > (std::numeric_limits<size_t>::max() - DDS_HEADER_BYTES) / 4) {
        set_error(outError, "DDS 输出尺寸过大");
        return false;
    }

    outDds.assign(DDS_HEADER_BYTES + pixelCount * 4, 0);
    write_u32(outDds, 0, DDS_MAGIC);
    write_u32(outDds, 4, 124);
    write_u32(outDds, 8, 0x0000100f); // CAPS | HEIGHT | WIDTH | PITCH | PIXELFORMAT
    write_u32(outDds, 12, height);
    write_u32(outDds, 16, width);
    write_u32(outDds, 20, width * 4);
    write_u32(outDds, 76, 32);
    write_u32(outDds, 80, DDPF_RGB | DDPF_ALPHAPIXELS);
    write_u32(outDds, 88, 32);
    write_u32(outDds, 92, 0x00ff0000);
    write_u32(outDds, 96, 0x0000ff00);
    write_u32(outDds, 100, 0x000000ff);
    write_u32(outDds, 104, 0xff000000);
    write_u32(outDds, 108, 0x00001000); // DDSCAPS_TEXTURE

    uint8_t* dst = outDds.data() + DDS_HEADER_BYTES;
    for (size_t i = 0; i < pixelCount; ++i) {
        dst[i * 4 + 0] = rgba[i * 4 + 2];
        dst[i * 4 + 1] = rgba[i * 4 + 1];
        dst[i * 4 + 2] = rgba[i * 4 + 0];
        dst[i * 4 + 3] = rgba[i * 4 + 3];
    }
    return true;
}

} // namespace ddscodec
