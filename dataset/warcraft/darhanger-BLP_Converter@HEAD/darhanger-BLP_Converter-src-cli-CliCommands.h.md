---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/cli/CliCommands.h"
ref: HEAD
---

#pragma once

#include <QStringList>

namespace darblp::cli {

class CliCommands
{
public:
    static bool isCliCommand(const QString &command);
    static int run(const QStringList &arguments);

private:
    static int runConvert(const QStringList &arguments);
    static int runBatch(const QStringList &arguments);
};

} // namespace darblp::cli
