---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/utils/Logger.cpp"
ref: HEAD
---

#include "utils/Logger.h"

#include <QDateTime>

namespace darblp {

QString Logger::line(const QString &message)
{
    return QStringLiteral("[%1] %2")
        .arg(QDateTime::currentDateTime().toString(QStringLiteral("HH:mm:ss")), message);
}

QString Logger::success(const QString &message)
{
    return line(QStringLiteral("OK  %1").arg(message));
}

QString Logger::error(const QString &message)
{
    return line(QStringLiteral("ERR %1").arg(message));
}

} // namespace darblp
