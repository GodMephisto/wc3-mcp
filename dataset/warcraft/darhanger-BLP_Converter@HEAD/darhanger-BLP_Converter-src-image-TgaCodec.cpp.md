---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/image/TgaCodec.cpp"
ref: HEAD
---

#include "image/TgaCodec.h"

#include <QImageReader>
#include <QImageWriter>

namespace darblp {

bool TgaCodec::read(const QString &path, QImage &image, QString *errorMessage)
{
    QImageReader reader(path, "tga");
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

bool TgaCodec::write(const QString &path, const QImage &image, QString *errorMessage)
{
    QImageWriter writer(path, "tga");
    if (writer.write(image)) {
        return true;
    }

    if (errorMessage) {
        *errorMessage = writer.errorString();
    }
    return false;
}

} // namespace darblp
