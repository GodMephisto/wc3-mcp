---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpEncoder.cpp"
ref: HEAD
---

#include "blp/BlpEncoder.h"

#include "blp/MipmapGenerator.h"

namespace darblp {

bool BlpEncoder::encode(const QString &path,
                        const QImage &image,
                        const ConversionOptions &options,
                        QString *errorMessage)
{
    Q_UNUSED(path)
    Q_UNUSED(image)
    Q_UNUSED(options)

    // TODO: Encode BLP2 payloads with DXT1/DXT3/DXT5 blocks and mipmap offsets.
    // The GUI and CLI already route BLP writes here, so the codec can be completed
    // without changing application flow.
    if (errorMessage) {
        *errorMessage = QStringLiteral("BLP2 writing is scaffolded; DXT compression is not implemented yet.");
    }
    return false;
}

} // namespace darblp
