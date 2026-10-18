---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/converter_png.cpp"
ref: HEAD
---

#include <cstddef>
#include <expected>
#include <spanstream>
#include <format>

#include <nvtt/nvtt.h>
#define STB_IMAGE_WRITE_IMPLEMENTATION
#include <stb/stb_image_write.h>

#include <blp/blp.hpp>
#include "assets_mpq_importer/blp.hpp"


namespace assmpq::blp {

// NOLINTNEXTLINE
static void write_to_vector(void* context, void* data, int size) {
    auto* buffer = static_cast<std::vector<unsigned char>*>(context);
    auto* bytes = static_cast<unsigned char*>(data);
    auto bytes_size = static_cast<size_t>(size);

    const std::span<unsigned char> data_span(bytes, bytes_size);
    buffer->insert(buffer->end(), data_span.begin(), data_span.end());
}

static auto fill_rgba_pixel_buffer(const wc3lib::blp::Blp::MipMap& mipmap)-> std::vector<uint32_t>
{
    const uint32_t width = mipmap.width();
    const uint32_t height = mipmap.height();

    std::vector<uint32_t> pixel_buffer(static_cast<size_t>(width) * height);
    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const auto& color = mipmap.colorAt(ix, iy);
            const uint32_t rgba = color.rgba();
            pixel_buffer[(iy * width) + ix] = std::byteswap(rgba);
        }
    }

    return pixel_buffer;
}

static auto fill_palette_pixel_buffer(
    const wc3lib::blp::Blp::MipMap& mipmap,
    const wc3lib::blp::Blp::ColorPtr& palette)-> std::vector<uint32_t>
{
    const uint32_t width = mipmap.width();
    const uint32_t height = mipmap.height();

    std::vector<uint32_t> pixel_buffer(static_cast<size_t>(width) * height);
    for (uint32_t iy = 0; iy < height; ++iy) {
        for (uint32_t ix = 0; ix < width; ++ix) {
            const auto& color = mipmap.colorAt(ix, iy);
            const uint32_t rgba = color.paletteColor(palette.get());
            pixel_buffer[(iy * width) + ix] = std::byteswap(rgba);
        }
    }

    return pixel_buffer;
}

auto convert_blp_to_png_image(const FileData& blp_file, size_t mipmap_idx)-> std::expected<FileData, ErrorMessage>
try {
    wc3lib::blp::Blp texture;

    std::ispanstream input(blp_file);
    texture.read(input);

    if (mipmap_idx >= texture.mipMaps().size()) {
        return std::unexpected(std::format("Mipmap index {} is out of range.", mipmap_idx));
    }

    const auto& mipmap = texture.mipMaps()[mipmap_idx];
    const uint32_t width = mipmap.width();
    const uint32_t height = mipmap.height();

    std::vector<uint32_t> pixel_buffer;
    if (texture.compression() == wc3lib::blp::Blp::Compression::Paletted) {
        const auto& palette = texture.palette();
        pixel_buffer = fill_palette_pixel_buffer(mipmap, palette);
    } else {
        pixel_buffer = fill_rgba_pixel_buffer(mipmap);
    }

    std::vector<char> output_buffer;

    // Use stb_image_write to write the PNG to the memory buffer via the callback
    // The stride_bytes parameter is the pitch, which can be width * channels for tightly packed data
    stbi_write_png_to_func(
        write_to_vector,
        &output_buffer,
        static_cast<int>(width),
        static_cast<int>(height),
        kRgbaChannels,
        pixel_buffer.data(),
        static_cast<int>(width) * kRgbaChannels);

    return output_buffer;
} catch (std::exception &e) {
    return std::unexpected(e.what());
}


} // namespace assmpq::blp



