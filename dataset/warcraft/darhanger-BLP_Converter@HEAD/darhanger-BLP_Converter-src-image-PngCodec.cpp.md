---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/image/PngCodec.cpp"
ref: HEAD
---

#include "image/PngCodec.h"

#include <QImageReader>
#include <QImageWriter>

namespace darblp {

bool PngCodec::read(const QString &path, QImage &image, QString *errorMessage)
{
    QImageReader reader(path, "png");
    reader.setAutoTransform(true);
    image = reader.read();
    if (!image.isNull()) {
        return true;
    }

    if (errorMessage) {
        *errorMessage = reader.errorString();
    }
    return false;
}

bool PngCodec::write(const QString &path, const QImage &image, QString *errorMessage)
{
    QImageWriter writer(path, "png");
    writer.setQuality(100);
    if (writer.write(image)) {
        return true;
    }

    if (errorMessage) {
        *errorMessage = writer.errorString();
    }
    return false;
}

} // namespace darblp
