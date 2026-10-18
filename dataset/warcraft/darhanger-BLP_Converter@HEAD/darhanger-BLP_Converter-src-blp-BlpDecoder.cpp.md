---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpDecoder.cpp"
ref: HEAD
---

#include "blp/BlpDecoder.h"

namespace darblp {

bool BlpDecoder::decode(const QString &path,
                        const Blp2Header &header,
                        QImage &image,
                        QString *errorMessage)
{
    Q_UNUSED(path)
    Q_UNUSED(header)
    image = QImage();

    if (errorMessage) {
        *errorMessage = QStringLiteral("BLP2 DXT decoding is not implemented yet. Header parsing is available.");
    }
    return false;
}

} // namespace darblp
