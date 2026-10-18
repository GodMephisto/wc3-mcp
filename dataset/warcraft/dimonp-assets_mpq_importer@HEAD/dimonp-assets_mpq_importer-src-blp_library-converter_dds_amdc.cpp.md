---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/converter_dds_amdc.cpp"
ref: HEAD
---

#include <expected>
#include <spanstream>
#include <algorithm>

#ifdef _MSC_VER
#define NOMINMAX
#endif

#include <spdlog/spdlog.h>
#include <cmp_compressonatorlib/compressonator.h>
#include <cmp_compressonatorlib/common.h>
#include <cmp_framework/common/cmp_boxfilter.h>
#include <_plugins/cimage/dds/dds_file.h>
#include <_plugins/cimage/dds/dds_helpers.h>

#include <blp/blp.hpp>
#include "assets_mpq_importer/blp.hpp"
#include "utils_blp.hpp"

namespace assmpq::blp {

// NOLINTBEGIN(clang-diagnostic-missing-designated-field-initializers, cppcoreguidelines-avoid-non-const-global-variables, cppcoreguidelines-pro-type-reinterpret-cast)

CMIPS g_CMIPS;

struct MipSetDeleter {
    void operator()(CMP_MipSet* mipset_ptr) const {
        if (mipset_ptr != nullptr) {
            CMP_FreeMipSet(mipset_ptr);
            delete mipset_ptr; // NOLINT(cppcoreguidelines-owning-memory)
        }
    }
};

using MipSetPtr = std::unique_ptr<CMP_MipSet, MipSetDeleter>;

static auto get_dxgi_format(const MipSet& mipSet)-> DXGI_FORMAT
{
    switch (mipSet.m_format) {
    case CMP_FORMAT_BC1:
        return DXGI_FORMAT_BC1_UNORM;
    case CMP_FORMAT_BC3:
        return DXGI_FORMAT_BC3_UNORM;
    case CMP_FORMAT_BC7:
        return DXGI_FORMAT_BC7_UNORM;
    default:
        return DXGI_FORMAT_UNKNOWN;
    }
}

auto persist_dds_dx10(const MipSet& mipSet)-> std::vector<char>
{
    DDS_FILE_HEADER ddsd2 = {};
    ddsd2.size        = sizeof(DDS_FILE_HEADER);
    ddsd2.flags       = DDSD_WIDTH | DDSD_HEIGHT | DDSD_LINEARSIZE;
    ddsd2.height      = static_cast<uint32_t>(mipSet.m_nHeight);
    ddsd2.width       = static_cast<uint32_t>(mipSet.m_nWidth);
    ddsd2.pitchOrLinearSize = g_CMIPS.GetMipLevel(&mipSet, 0)->m_dwLinearSize;
    ddsd2.mipMapCount = static_cast<uint32_t>(mipSet.m_nMipLevels);
    ddsd2.ddspf.size  = sizeof(DDPIXELFORMAT);
    ddsd2.ddspf.flags = DDPF_FOURCC;
    ddsd2.ddspf.fourCC = CMP_MAKEFOURCC('D', 'X', '1', '0');
    ddsd2.caps        = DDSCAPS_TEXTURE;

    if (mipSet.m_nMipLevels > 1) {
        ddsd2.flags |= DDSD_MIPMAPCOUNT;
        ddsd2.caps  |= DDSCAPS_MIPMAP;
    }

    std::ostringstream output;

    // Write the header
    output.write( reinterpret_cast<const char*>(&DDS_HEADER), static_cast<std::streamsize>(sizeof(DDS_HEADER)));
    output.write( reinterpret_cast<const char*>(&ddsd2), static_cast<std::streamsize>(sizeof(ddsd2)));

    DDS_FILE_HEADER_DXT10 HeaderDDS10 = {
        .dxgiFormat          = get_dxgi_format(mipSet),
        .resourceDimension   = 3, // D3D10_RESOURCE_DIMENSION_TEXTURE2D
        .miscFlag            = 0,
        .arraySize           = 1,
        .miscFlags2          = 0
    };

    // Write DDS10 header
    output.write( reinterpret_cast<const char*>(&HeaderDDS10), static_cast<std::streamsize>(sizeof(HeaderDDS10)));

    // Write the data
    for (int nMipLevel = 0; nMipLevel < mipSet.m_nMipLevels; nMipLevel++) {
        const auto *mip_level_ptr = g_CMIPS.GetMipLevel(&mipSet, nMipLevel);
        output.write(
            reinterpret_cast<const char*>(mip_level_ptr->m_pbData), // NOLINT(cppcoreguidelines-pro-type-union-access)
            static_cast<std::streamsize>(mip_level_ptr->m_dwLinearSize));
    }
    const auto& buffer_str = output.str();
    return { buffer_str.begin(), buffer_str.end() };
}

// Generate additional mipmaps up to 1x1 size
static auto generate_extra_mipmaps(
    MipSet& mipset_in,
    const wc3lib::blp::Blp::MipMap& last_mipmap,
    const size_t last_mipmap_idx
)-> bool
{
    size_t mip_idx = last_mipmap_idx;
    int mip_width = static_cast<int>(last_mipmap.width());
    int mip_height = static_cast<int>(last_mipmap.height());

    while (mip_width > 1 || mip_height > 1) {
        mip_width = std::max(1, mip_width / 2);
        mip_height = std::max(1, mip_height / 2);
        mip_idx++;

        CMP_MipLevel* this_mip_level = g_CMIPS.GetMipLevel(
            &mipset_in,
            static_cast<CMP_INT>(mip_idx),
            0);

        if (this_mip_level == nullptr) {
            spdlog::error("generate_extra_mipmaps: Error obtaining new mipmap level.");
            return false;
        }

        if (!g_CMIPS.AllocateMipLevelData(
            this_mip_level,
            mip_width,
            mip_height,
            mipset_in.m_ChannelFormat,
            mipset_in.m_TextureDataType))
        {
            spdlog::error("generate_extra_mipmaps: Error allocating mipmap level data.");
            return false;
        }

        CMP_MipLevel* prev_mip_level = g_CMIPS.GetMipLevel( // NOLINT wrong const result suggestion
            &mipset_in,
            static_cast<CMP_INT>(mip_idx - 1),
            0);

        if (this_mip_level == nullptr) {
            spdlog::error("generate_extra_mipmaps: Error obtaining base mipmap level.");
            return false;
        }

        GenerateMipmapLevel(
            this_mip_level,
            &prev_mip_level,
            1,
            mipset_in.m_format);
    }
    return true;
}


// NOLINTEND(clang-diagnostic-missing-designated-field-initializers, cppcoreguidelines-avoid-non-const-global-variables, cppcoreguidelines-pro-type-reinterpret-cast)

auto convert_blp_to_dds_texture_amdc( // NOLINT
    const FileData& blp_file,
    const Compression& compression,
    bool regen_mipmaps
)-> std::expected<FileData, ErrorMessage>
try {
    wc3lib::blp::Blp texture;

    static const std::unordered_map<Compression, CMP_FORMAT> format_map = {
        { Compression::DDS_BC1, CMP_FORMAT_BC1 },
        { Compression::DDS_BC3, CMP_FORMAT_BC3 },
        { Compression::DDS_BC7, CMP_FORMAT_BC7 },
    };

    std::ispanstream input(blp_file);
    texture.read(input);

    CMP_InitFramework();

    const bool has_mipmaps = texture.mipMaps().size() > 1;
    const size_t mipmap_count = regen_mipmaps ? 1 : texture.mipMaps().size();
    const int blp_width = static_cast<int>(texture.mipMaps()[0].width());
    const int blp_height = static_cast<int>(texture.mipMaps()[0].height());

    const MipSetPtr mipset_in(new CMP_MipSet{});

    if (!g_CMIPS.AllocateMipSet(mipset_in.get(), CF_8bit, TDT_ARGB, TT_2D, blp_width, blp_height, 1)) {
        return std::unexpected("Compressionator: Error allocating Compressionator::MipSet");
    }

    const auto max_mipmaps = static_cast<size_t>(mipset_in->m_nMaxMipLevels);
    const auto extra_mipmaps = regen_mipmaps || has_mipmaps ? max_mipmaps - mipmap_count : 0;

    mipset_in->m_nMipLevels = static_cast<CMP_INT>(mipmap_count + extra_mipmaps);
    mipset_in->m_format     = CMP_FORMAT_RGBA_8888;

    for (size_t mip_idx = 0; mip_idx < mipmap_count; ++mip_idx) {
        const auto& mipmap = texture.mipMaps()[mip_idx];
        const int mip_width = static_cast<int>(mipmap.width());
        const int mip_height = static_cast<int>(mipmap.height());

        CMP_MipLevel* mip_level_ptr = g_CMIPS.GetMipLevel(mipset_in.get(), static_cast<CMP_INT>(mip_idx));
        if (!g_CMIPS.AllocateMipLevelData(mip_level_ptr, mip_width, mip_height, CF_8bit, TDT_ARGB)) {
            return std::unexpected("Compressionator: Error allocating MipLevelData");
        }

        std::vector<uint32_t> mipmap_color_buffer;
        if (texture.compression() == wc3lib::blp::Blp::Compression::Paletted) {
            const auto& palette = texture.palette();
            mipmap_color_buffer = get_paletted_mipmap_buffer_rgba(mipmap, palette);
        } else {
            mipmap_color_buffer = get_mipmap_buffer_rgba(mipmap);
        }

        CMP_BYTE* data_ptr = mip_level_ptr->m_pbData; // NOLINT(cppcoreguidelines-pro-type-union-access)
        memcpy(data_ptr, mipmap_color_buffer.data(), mipmap_color_buffer.size());

        // Assign miplevel 0 to MipSetin pData ref
        if (mipset_in->pData == nullptr) {
            mipset_in->pData       = data_ptr;
            mipset_in->dwDataSize  = static_cast<CMP_DWORD>(mipmap_color_buffer.size());
            mipset_in->dwWidth     = static_cast<CMP_DWORD>(mip_width);
            mipset_in->dwHeight    = static_cast<CMP_DWORD>(mip_height);
        }
    }

    // auto generate extra mipmaps up to 1x1 dimesion
    if (extra_mipmaps > 0) {
        const bool result = generate_extra_mipmaps(
            *mipset_in,
            texture.mipMaps()[mipmap_count - 1],
            mipmap_count - 1);

        if (!result) {
            return std::unexpected("Compressionator: Error generating extra mipmaps.");
        }
    }


    // Do compression
    KernelOptions options = {};
    options.encodeWith    = CMP_Compute_type::CMP_CPU;
    options.format        = format_map.at(compression);  // Destination format (e.g., BC1, BC3, BC7)
    options.fquality      = 1.0F;                           // Quality level (0.0 to 1.0)
    options.threads       = 0;

    if (options.format == CMP_FORMAT_BC1) {
        options.bc15.useAlphaThreshold  = true; // NOLINT(cppcoreguidelines-pro-type-union-access)
        options.bc15.alphaThreshold     = 1;    // NOLINT(cppcoreguidelines-pro-type-union-access)
    }

    const MipSetPtr mipset_out(new CMP_MipSet{});

    auto dont_stop_callback = [] (CMP_FLOAT /*fProgress*/, CMP_DWORD_PTR /*pUser1*/, CMP_DWORD_PTR /*pUser2*/) -> bool {
        return false;
    };

    // Perform compression CMP_CalculateBufferSize()
    if (CMP_ProcessTexture(mipset_in.get(), mipset_out.get(), options, dont_stop_callback) != CMP_OK) {
        return std::unexpected("Compressionator: Error processing texture.");
    }

    return persist_dds_dx10(*mipset_out);
} catch (std::exception &e) {
    return std::unexpected(e.what());
}


} // namespace assmpq::blp



