---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/converter_dds_nvtt.cpp"
ref: HEAD
---

#include <expected>
#include <spanstream>

#include <spdlog/spdlog.h>
#include <nvtt/nvtt.h>
#include <nvtt/Surface.h>

#include <blp/blp.hpp>
#include "assets_mpq_importer/blp.hpp"
#include "utils_blp.hpp"

namespace assmpq::blp {

class MemoryOutputHandler : public nvtt::OutputHandler
{
    static constexpr size_t kDDSHeadetSize = 148;
public:
    explicit MemoryOutputHandler(size_t estimated_size = 0) {
        dds_data.reserve(estimated_size + kDDSHeadetSize);
    }

    // This vector will store the DDS data
    std::vector<char> dds_data;

    // The begin method is called at the start of the compression process.
    // It can be used to prepare the output buffer and optionally write the DDS header.
    void beginImage(int /*size*/, int /*width*/, int /*height*/, int /*depth*/, int /*faceCount*/, int /*mipmapCount*/) override {}

    // The writeData method is called to write compressed data chunks to the output.
    // We append the data to our vector.
    bool writeData(const void * data, int size) override
    {
        const auto* bytes = static_cast<const unsigned char*>(data);
        const std::span<const unsigned char> data_span(bytes, static_cast<size_t>(size));
        dds_data.insert(dds_data.end(), data_span.begin(), data_span.end());
        return true;
    }

    // The end method is called when the compression is finished.
    void endImage() override {}
};

// Generate additional mipmaps up to 1x1 size
static auto generate_extra_mipmaps(
    const nvtt::Context& context,
    const nvtt::CompressionOptions& compression_options,
    const nvtt::OutputOptions& output_options,
    const wc3lib::blp::Blp& texture,
    const wc3lib::blp::Blp::MipMap& last_mipmap,
    const size_t last_mipmap_idx
)-> bool
{
    nvtt::Surface surface;
    size_t mip_idx = last_mipmap_idx;

    std::vector<uint32_t> mipmap_color_buffer;
    if (texture.compression() == wc3lib::blp::Blp::Compression::Paletted) {
        const auto& palette = texture.palette();
        mipmap_color_buffer = get_paletted_mipmap_buffer_rgba(last_mipmap, palette);
    } else {
        mipmap_color_buffer = get_mipmap_buffer_rgba(last_mipmap);
    }

    const int mip_width = static_cast<int>(last_mipmap.width());
    const int mip_height = static_cast<int>(last_mipmap.height());

    if (!surface.setImage(nvtt::InputFormat_BGRA_8UB, mip_width, mip_height, 1, mipmap_color_buffer.data())) {
        spdlog::error("Error setting image data to nvtt::Surface.");
        return false;
    }

    while (surface.buildNextMipmap(nvtt::MipmapFilter_Triangle, 1)) {
        context.compress(
            surface,
            0,
            static_cast<int>(++mip_idx),
            compression_options,
            output_options);
    }

    return true;
}

auto convert_blp_to_dds_texture_nvtt(
    const FileData& blp_file,
    const Compression& compression,
    bool regen_mipmaps
)-> std::expected<FileData, ErrorMessage>
try {
    wc3lib::blp::Blp texture;

    static const std::unordered_map<Compression, nvtt::Format> format_map = {
        { Compression::DDS_BC1, nvtt::Format_BC1 },
        { Compression::DDS_BC3, nvtt::Format_BC3 },
        { Compression::DDS_BC7, nvtt::Format_BC7 },
    };

    std::ispanstream input(blp_file);
    texture.read(input);

    nvtt::CompressionOptions compression_options;
    // Set the desired compression format, e.g., BC1, BC3, or BC7
    compression_options.setFormat(format_map.at(compression));
    compression_options.setQuality(nvtt::Quality_Normal);

    const nvtt::Context context;
    // context.enableCudaAcceleration(!nocuda);

    const bool has_mipmaps = texture.mipMaps().size() > 1;
    const size_t mipmap_count = regen_mipmaps ? 1 : texture.mipMaps().size();

    const int blp_width = static_cast<int>(texture.mipMaps()[0].width());
    const int blp_height = static_cast<int>(texture.mipMaps()[0].height());

    const auto max_mipmaps = nv::countMipmaps(
        static_cast<unsigned>(blp_width),
        static_cast<unsigned>(blp_height), 0);
    const auto extra_mipmaps = regen_mipmaps || has_mipmaps ? max_mipmaps - mipmap_count : 0;

    const int estimated_size = context.estimateSize(
        blp_width,
        blp_height,
        1,
        static_cast<int>(mipmap_count + extra_mipmaps),
        compression_options);

    MemoryOutputHandler output_handler(static_cast<size_t>(estimated_size));

    nvtt::OutputOptions output_options;
    output_options.setContainer(nvtt::Container_DDS10);
    output_options.setOutputHandler(&output_handler);

    // Compress the texture
    // For NVTT 3, you typically call outputHeader() and then compress().
    if (context.outputHeader(
        nvtt::TextureType_2D,
        blp_width,
        blp_height,
        1,
        1,
        static_cast<int>(mipmap_count + extra_mipmaps),
        false,
        compression_options,
        output_options)) {

        // Conver and add each custom mipmap level
        for (size_t mip_idx = 0; mip_idx < mipmap_count; ++mip_idx) {
            const auto& mipmap = texture.mipMaps()[mip_idx];

            const int mip_width = static_cast<int>(mipmap.width());
            const int mip_height = static_cast<int>(mipmap.height());

            std::vector<float> mipmap_color_buffer;
            if (texture.compression() == wc3lib::blp::Blp::Compression::Paletted) {
                const auto& palette = texture.palette();
                mipmap_color_buffer = get_paletted_mipmap_buffer_float(mipmap, palette);
            } else {
                mipmap_color_buffer = get_mipmap_buffer_float(mipmap);
            }

            // Feed the custom data for the current mip level
            // The library will compress this data and write the compressed blocks to the output handler
            context.compress(
                mip_width,
                mip_height,
                1,
                0,
                static_cast<int>(mip_idx),
                mipmap_color_buffer.data(),
                compression_options,
                output_options
            );
        }

            // auto generate extra mipmaps up to 1x1 dimesion
        if (extra_mipmaps > 0) {
            const bool result = generate_extra_mipmaps(
                context,
                compression_options,
                output_options,
                texture,
                texture.mipMaps()[mipmap_count - 1],
                mipmap_count - 1
            );

            if (!result) {
                return std::unexpected("Error generating extra mipmaps.");
            }
        }
    }
    return output_handler.dds_data;
} catch (std::exception &e) {
    return std::unexpected(e.what());
}


} // namespace assmpq::blp



