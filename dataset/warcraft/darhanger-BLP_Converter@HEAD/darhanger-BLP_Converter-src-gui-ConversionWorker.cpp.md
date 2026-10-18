---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/ConversionWorker.cpp"
ref: HEAD
---

#include "gui/ConversionWorker.h"

#include "core/Converter.h"

namespace darblp::gui {

void ConversionWorker::convert(QStringList files, darblp::ConversionOptions options)
{
    darblp::Converter converter;
    int successCount = 0;
    int errorCount = 0;

    for (int i = 0; i < files.size(); ++i) {
        emit fileStarted(files.at(i), i, files.size());
        const darblp::ConversionResult result = converter.convertFile(files.at(i), options);
        if (result.success) {
            ++successCount;
        } else {
            ++errorCount;
        }
        emit fileFinished(result, i, files.size());
    }

    emit finished(successCount, errorCount, options.outputFolder);
}

} // namespace darblp::gui
