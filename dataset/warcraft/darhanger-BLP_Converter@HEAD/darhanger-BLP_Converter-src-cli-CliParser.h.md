---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/cli/CliParser.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"

#include <QString>
#include <QStringList>

namespace darblp::cli {

class CliParser
{
public:
    static bool parseOutputFormat(const QString &value, darblp::OutputFormat &format);
    static bool parseCompression(const QString &value, darblp::BlpCompression &compression);
    static darblp::OutputFormat formatFromPath(const QString &path);
    static QString usage();
};

} // namespace darblp::cli
