---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/mpq_library/mpq.cpp"
ref: HEAD
---

#include <string>
#include <algorithm>
#include <functional>
#include <ranges>
#include <filesystem>
#include <expected>
#include <spanstream>

#include <re2/re2.h>

#include <platform.hpp>
#include <exception.hpp>
#include <mpq/archive.hpp>
#include <mpq/listfile.hpp>

#include "assets_mpq_importer/mpq.hpp"

// NOLINTBEGIN(maybe-uninitialized)

namespace assmpq::mpq {

using filelist_filter_t = std::function<bool(const std::string&)>;

static auto wildcard_to_regex(std::string mask)-> re2::RE2 {
    // Convert wildcard to regex pattern
    // Simple replacements; for production, escape other regex special chars
    std::size_t pos = 0;
    while ((pos = mask.find('.', pos)) != std::string::npos) { mask.replace(pos, 1, "\\."); pos += 2; }
    pos = 0;
    while ((pos = mask.find('?', pos)) != std::string::npos) { mask.replace(pos, 1, "."); pos += 1; }
    pos = 0;
    while ((pos = mask.find('*', pos)) != std::string::npos) { mask.replace(pos, 1, ".*"); pos += 2; }

    RE2::Options options;
    options.set_case_sensitive(false);
    return { mask, options };
}

auto list_mpq_files(const std::filesystem::path& archive_path, const std::string& mask)
    -> std::expected<ArchiveEntries, ErrorMessage>
{
    wc3lib::mpq::Archive archive;
    try {
        archive.open(archive_path.c_str());
    } catch (const wc3lib::Exception &exception) {
        return std::unexpected(exception.what());
    }

    if (archive.containsListfileFile()) {
        const wc3lib::mpq::Listfile filelist = archive.listfileFile();

        if (filelist.isValid()) {
            const auto mask_regex = wildcard_to_regex(mask);

            auto filter = mask.empty() ?
                filelist_filter_t([](const std::string &entry)-> bool { return !entry.empty(); }) :
                filelist_filter_t([&mask_regex](const std::string &entry)-> bool {
                    return re2::RE2::PartialMatch(entry, mask_regex);
                });

            wc3lib::mpq::Listfile::Entries entries = filelist.entries();
            std::ranges::sort(entries);

            return entries |
                std::views::filter(filter) |
                std::views::transform([&archive](const auto &entry)-> FileEntry {
                    const wc3lib::mpq::File file = archive.findFile(entry);
                    return FileEntry{ .filename = entry, .size = file.size() };
                }) |
                std::ranges::to<ArchiveEntries>();
        }
    }
    return std::unexpected("List file not found.");
}

auto extract_mpq_file(const std::filesystem::path& archive_path, const std::string& filename)
    -> std::expected<FileData, ErrorMessage>
try {
    wc3lib::mpq::Archive archive;
    archive.open(archive_path.c_str());

    const wc3lib::mpq::File file = archive.findFile(filename);
    if (!file.isValid()) {
        return std::unexpected("File not found.");
    }

    std::vector<char> output_buffer(file.size());
    std::ospanstream output_stream(output_buffer, std::ios::out | std::ios::binary);
    file.decompress(output_stream);

    return output_buffer;
} catch (const wc3lib::Exception &exception) {
    return std::unexpected(exception.what());
}
// NOLINTEND(maybe-uninitialized)


}  // namespace assmpq::mpq