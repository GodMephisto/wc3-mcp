---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/utils/Logger.h"
ref: HEAD
---

#pragma once

#include <QString>

namespace darblp {

class Logger
{
public:
    static QString line(const QString &message);
    static QString success(const QString &message);
    static QString error(const QString &message);
};

} // namespace darblp
