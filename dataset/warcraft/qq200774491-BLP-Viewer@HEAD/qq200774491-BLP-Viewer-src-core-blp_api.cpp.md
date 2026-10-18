---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/blp_api.cpp"
ref: HEAD
---

#include "blp_api.h"

std::string BlpApi::version() const {
    return "blp_codec (built-in, libjpeg-turbo)";
}

bool BlpApi::load_from_buffer(const std::vector<uint8_t>& data,
                              blpcodec::RawImage* outImage,
                              std::string* outError) const {
    if (!outImage) {
        if (outError) *outError = "输出图像为空";
        return false;
    }

    auto image = blpcodec::decode(data.data(), data.size(), outError);
    if (!image) return false;

    *outImage = std::move(*image);
    return true;
}
