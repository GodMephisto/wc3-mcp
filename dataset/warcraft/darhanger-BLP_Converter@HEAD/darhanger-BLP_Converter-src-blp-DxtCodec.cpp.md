---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/DxtCodec.cpp"
ref: HEAD
---

#include "blp/DxtCodec.h"

namespace darblp {

bool DxtCodec::decode(const QByteArray &blocks,
                      int width,
                      int height,
                      DxtFormat format,
                      QImage &image,
                      QString *errorMessage)
{
    Q_UNUSED(blocks)
    Q_UNUSED(width)
    Q_UNUSED(height)
    Q_UNUSED(format)
    image = QImage();

    if (errorMessage) {
        *errorMessage = QStringLiteral("DXT decoding TODO: implement DXT1/DXT3/DXT5 block expansion.");
    }
    return false;
}

bool DxtCodec::encode(const QImage &image,
                      DxtFormat format,
                      QByteArray &blocks,
                      QString *errorMessage)
{
    Q_UNUSED(image)
    Q_UNUSED(format)
    blocks.clear();

    if (errorMessage) {
        *errorMessage = QStringLiteral("DXT encoding TODO: implement DXT1/DXT3/DXT5 block compression.");
    }
    return false;
}

} // namespace darblp
