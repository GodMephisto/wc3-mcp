---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/utils_blp.cpp"
ref: HEAD
---

#include <bit>
#include <blp/blp.hpp>
#include "assets_mpq_importer/blp.hpp"

namespace assmpq::blp {

enum class ColorShift : uint8_t {
    Red = 24U,
    Green = 16U,
    Blue = 8U,
    Alpha = 0U
};

template<ColorShift SHIFT>
inline auto get_color(uint32_t rgba)-> float
{
    static constexpr uint32_t kByteMask = 0xFF;
    static constexpr float kColorBase = 255.0F;
    return static_cast<float>((rgba >> static_cast<uint8_t>(SHIFT)) & kByteMask) / kColorBase;
}

auto get_mipmap_buffer_rgba(const wc3lib::blp::Blp::MipMap& mipmap)
    -> std::vector<uint32_t>
{
    const size_t width = mipmap.width();
    const size_t height = mipmap.height();
    std::vector<uint32_t> colors_buffer(width * height * kRgbaChannels);

    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const size_t idx = (iy * width) + ix;
            const uint32_t rgba = mipmap.colorAt(ix, iy).rgba();
            colors_buffer[idx] = std::byteswap(rgba);
        }
    }

    return colors_buffer;
}

auto get_paletted_mipmap_buffer_rgba(
    const wc3lib::blp::Blp::MipMap& mipmap,
    const  wc3lib::blp::Blp::ColorPtr& palette)
    -> std::vector<uint32_t>
{
    const size_t width = mipmap.width();
    const size_t height = mipmap.height();

    std::vector<uint32_t> colors_buffer(width * height * kRgbaChannels);

    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const size_t idx = (iy * width) + ix;
            const uint32_t rgba = mipmap.colorAt(ix, iy).paletteColor(palette.get());
            colors_buffer[idx] = std::byteswap(rgba);
        }
    }

    return colors_buffer;
}

auto get_mipmap_buffer_float(const wc3lib::blp::Blp::MipMap& mipmap)
    -> std::vector<float>
{
    const size_t width = mipmap.width();
    const size_t height = mipmap.height();
    std::vector<float> colors_buffer(width * height * kRgbaChannels);

    const size_t offset_r = width * height * 0;
    const size_t offset_g = width * height * 1;
    const size_t offset_b = width * height * 2;
    const size_t offset_a = width * height * 3;

    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const size_t idx = (iy * width) + ix;
            const uint32_t rgba = mipmap.colorAt(ix, iy).rgba();

            colors_buffer[idx + offset_r] = get_color<ColorShift::Red>(rgba);
            colors_buffer[idx + offset_g] = get_color<ColorShift::Green>(rgba);
            colors_buffer[idx + offset_b] = get_color<ColorShift::Blue>(rgba);
            colors_buffer[idx + offset_a] = get_color<ColorShift::Alpha>(rgba);
        }
    }

    return colors_buffer;
}

auto get_paletted_mipmap_buffer_float(
    const wc3lib::blp::Blp::MipMap& mipmap,
    const  wc3lib::blp::Blp::ColorPtr& palette)
    -> std::vector<float>
{
    const size_t width = mipmap.width();
    const size_t height = mipmap.height();

    std::vector<float> colors_buffer(width * height * kRgbaChannels);

    const size_t offset_r = width * height * 0;
    const size_t offset_g = width * height * 1;
    const size_t offset_b = width * height * 2;
    const size_t offset_a = width * height * 3;

    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const size_t idx = (iy * width) + ix;
            const uint32_t rgba = mipmap.colorAt(ix, iy).paletteColor(palette.get());

            colors_buffer[idx + offset_r] = get_color<ColorShift::Red>(rgba);
            colors_buffer[idx + offset_g] = get_color<ColorShift::Green>(rgba);
            colors_buffer[idx + offset_b] = get_color<ColorShift::Blue>(rgba);
            colors_buffer[idx + offset_a] = get_color<ColorShift::Alpha>(rgba);
        }
    }

    return colors_buffer;
}

} // namespace assmpq::blp