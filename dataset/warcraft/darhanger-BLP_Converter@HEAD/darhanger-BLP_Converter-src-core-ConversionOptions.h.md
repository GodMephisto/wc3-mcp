---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/ConversionOptions.h"
ref: HEAD
---

#pragma once

#include <QString>

namespace darblp {

enum class OutputFormat
{
    Png,
    Tga,
    Blp
};

enum class BlpCompression
{
    Auto,
    Dxt1,
    Dxt3,
    Dxt5
};

struct ConversionOptions
{
    OutputFormat outputFormat = OutputFormat::Png;
    BlpCompression compression = BlpCompression::Auto;
    bool generateMipmaps = true;
    bool preserveAlpha = true;
    bool resizeToPowerOfTwo = false;
    bool keepFolderStructure = false;
    QString outputFolder;
    QString sourceRoot;
};

inline QString toExtension(OutputFormat format)
{
    switch (format) {
    case OutputFormat::Png:
        return QStringLiteral("png");
    case OutputFormat::Tga:
        return QStringLiteral("tga");
    case OutputFormat::Blp:
        return QStringLiteral("blp");
    }
    return QStringLiteral("png");
}

inline QString toDisplayName(OutputFormat format)
{
    switch (format) {
    case OutputFormat::Png:
        return QStringLiteral("PNG");
    case OutputFormat::Tga:
        return QStringLiteral("TGA");
    case OutputFormat::Blp:
        return QStringLiteral("BLP");
    }
    return QStringLiteral("PNG");
}

inline QString toDisplayName(BlpCompression compression)
{
    switch (compression) {
    case BlpCompression::Auto:
        return QStringLiteral("Auto");
    case BlpCompression::Dxt1:
        return QStringLiteral("DXT1");
    case BlpCompression::Dxt3:
        return QStringLiteral("DXT3");
    case BlpCompression::Dxt5:
        return QStringLiteral("DXT5");
    }
    return QStringLiteral("Auto");
}

} // namespace darblp
