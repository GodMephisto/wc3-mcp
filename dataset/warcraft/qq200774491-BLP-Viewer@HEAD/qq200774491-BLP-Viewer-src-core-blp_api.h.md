---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/blp_api.h"
ref: HEAD
---

#pragma once

#include <string>
#include <vector>

#include "blp/blp_codec.h"

// Built-in BLP codec facade. History: was blp_lib.dll (Rust) dynamic loader.
// Now built-in; keeps old interface to avoid changing callers.
class BlpApi {
public:
    /**
     * @brief  No-op initialisation stub; always succeeds for the built-in codec.
     * @param  outError  Ignored; may be nullptr.
     * @return Always true.
     */
    bool ensure_loaded(std::string* outError) const { (void)outError; return true; }

    /**
     * @brief  Returns true because the built-in codec is always available.
     * @return Always true.
     */
    bool is_loaded() const { return true; }

    /**
     * @brief  Returns a human-readable codec location description.
     * @return The string "built-in".
     */
    std::string library_path() const { return "built-in"; }

    /**
     * @brief  Returns a human-readable codec version string.
     * @return Version identifier including the underlying JPEG library name.
     */
    std::string version() const;

    /**
     * @brief  Decodes a BLP file from an in-memory buffer into a raw RGBA image.
     * @param  data      Raw bytes of the BLP file.
     * @param  outImage  Receives the decoded image on success; must not be nullptr.
     * @param  outError  Optional; receives an error description on failure.
     * @return true on success; false if data is invalid or outImage is nullptr.
     */
    bool load_from_buffer(const std::vector<uint8_t>& data,
                          blpcodec::RawImage* outImage,
                          std::string* outError = nullptr) const;
};
