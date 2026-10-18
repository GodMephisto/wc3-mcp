---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/ImageData.cpp"
ref: HEAD
---

#include "core/ImageData.h"

#include <utility>

namespace darblp {

ImageData::ImageData(QImage input)
    : image(std::move(input))
{
}

bool ImageData::isValid() const
{
    return !image.isNull();
}

bool ImageData::hasAlpha() const
{
    return image.hasAlphaChannel();
}

int ImageData::width() const
{
    return image.width();
}

int ImageData::height() const
{
    return image.height();
}

} // namespace darblp
