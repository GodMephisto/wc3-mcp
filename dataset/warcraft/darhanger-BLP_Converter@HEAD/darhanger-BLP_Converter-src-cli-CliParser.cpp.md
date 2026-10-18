---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/cli/CliParser.cpp"
ref: HEAD
---

#include "cli/CliParser.h"

#include <QFileInfo>

namespace darblp::cli {

bool CliParser::parseOutputFormat(const QString &value, darblp::OutputFormat &format)
{
    const QString normalized = value.toLower();
    if (normalized == QLatin1String("png")) {
        format = darblp::OutputFormat::Png;
        return true;
    }
    if (normalized == QLatin1String("tga")) {
        format = darblp::OutputFormat::Tga;
        return true;
    }
    if (normalized == QLatin1String("blp")) {
        format = darblp::OutputFormat::Blp;
        return true;
    }
    return false;
}

bool CliParser::parseCompression(const QString &value, darblp::BlpCompression &compression)
{
    const QString normalized = value.toLower();
    if (normalized == QLatin1String("auto")) {
        compression = darblp::BlpCompression::Auto;
        return true;
    }
    if (normalized == QLatin1String("dxt1")) {
        compression = darblp::BlpCompression::Dxt1;
        return true;
    }
    if (normalized == QLatin1String("dxt3")) {
        compression = darblp::BlpCompression::Dxt3;
        return true;
    }
    if (normalized == QLatin1String("dxt5")) {
        compression = darblp::BlpCompression::Dxt5;
        return true;
    }
    return false;
}

darblp::OutputFormat CliParser::formatFromPath(const QString &path)
{
    darblp::OutputFormat format = darblp::OutputFormat::Png;
    parseOutputFormat(QFileInfo(path).suffix(), format);
    return format;
}

QString CliParser::usage()
{
    return QStringLiteral(
        "Usage:\n"
        "  DarBLPConverter.exe convert <input> <output> [--compression auto|dxt1|dxt3|dxt5] [--mipmaps] [--no-alpha] [--pot]\n"
        "  DarBLPConverter.exe batch <input-folder> <output-folder> --to png|tga|blp [--compression auto|dxt1|dxt3|dxt5] [--mipmaps] [--keep-structure]\n");
}

} // namespace darblp::cli
