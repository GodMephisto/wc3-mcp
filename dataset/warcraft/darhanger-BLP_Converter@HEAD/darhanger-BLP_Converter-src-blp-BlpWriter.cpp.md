---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpWriter.cpp"
ref: HEAD
---

#include "blp/BlpWriter.h"

#include "blp/BlpEncoder.h"

namespace darblp {

bool BlpWriter::writeImage(const QString &path,
                           const QImage &image,
                           const ConversionOptions &options,
                           QString *errorMessage)
{
    return BlpEncoder::encode(path, image, options, errorMessage);
}

} // namespace darblp
