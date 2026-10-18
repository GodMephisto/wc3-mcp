---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/MipmapGenerator.cpp"
ref: HEAD
---

#include "blp/MipmapGenerator.h"

#include "blp/BlpHeader.h"

#include <QtMath>

namespace darblp {

QVector<QImage> MipmapGenerator::generate(const QImage &baseImage)
{
    QVector<QImage> mips;
    if (baseImage.isNull()) {
        return mips;
    }

    QImage current = baseImage.convertToFormat(QImage::Format_RGBA8888);
    mips.push_back(current);

    while (current.width() > 1 || current.height() > 1) {
        const int nextWidth = qMax(1, current.width() / 2);
        const int nextHeight = qMax(1, current.height() / 2);
        current = current.scaled(nextWidth, nextHeight, Qt::IgnoreAspectRatio, Qt::SmoothTransformation);
        mips.push_back(current);

        if (mips.size() >= BlpMaxMipmaps) {
            break;
        }
    }

    return mips;
}

} // namespace darblp
