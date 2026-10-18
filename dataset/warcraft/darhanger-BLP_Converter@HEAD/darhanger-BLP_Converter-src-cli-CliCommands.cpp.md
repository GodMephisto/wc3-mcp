---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/cli/CliCommands.cpp"
ref: HEAD
---

#include "cli/CliCommands.h"

#include "cli/CliParser.h"
#include "core/Converter.h"
#include "utils/FileUtils.h"

#include <QDir>
#include <QFileInfo>
#include <QTextStream>
#include <QVector>

#include <cstdio>

namespace darblp::cli {

namespace {

QTextStream out()
{
    return QTextStream(stdout);
}

QTextStream err()
{
    return QTextStream(stderr);
}

bool applyCommonOptions(const QStringList &arguments, int startIndex, darblp::ConversionOptions &options)
{
    for (int i = startIndex; i < arguments.size(); ++i) {
        const QString arg = arguments.at(i);
        if (arg == QLatin1String("--mipmaps")) {
            options.generateMipmaps = true;
        } else if (arg == QLatin1String("--no-mipmaps")) {
            options.generateMipmaps = false;
        } else if (arg == QLatin1String("--no-alpha")) {
            options.preserveAlpha = false;
        } else if (arg == QLatin1String("--pot")) {
            options.resizeToPowerOfTwo = true;
        } else if (arg == QLatin1String("--keep-structure")) {
            options.keepFolderStructure = true;
        } else if (arg == QLatin1String("--compression") && i + 1 < arguments.size()) {
            darblp::BlpCompression compression;
            if (!CliParser::parseCompression(arguments.at(++i), compression)) {
                err() << "Unknown compression value.\n";
                return false;
            }
            options.compression = compression;
        } else if (arg == QLatin1String("--to") && i + 1 < arguments.size()) {
            darblp::OutputFormat format;
            if (!CliParser::parseOutputFormat(arguments.at(++i), format)) {
                err() << "Unknown output format.\n";
                return false;
            }
            options.outputFormat = format;
        } else {
            err() << "Unknown option: " << arg << '\n';
            return false;
        }
    }

    return true;
}

} // namespace

bool CliCommands::isCliCommand(const QString &command)
{
    return command == QLatin1String("convert")
        || command == QLatin1String("batch")
        || command == QLatin1String("--help")
        || command == QLatin1String("-h");
}

int CliCommands::run(const QStringList &arguments)
{
    if (arguments.size() < 2 || arguments.at(1) == QLatin1String("--help") || arguments.at(1) == QLatin1String("-h")) {
        out() << CliParser::usage();
        return 0;
    }

    const QString command = arguments.at(1);
    if (command == QLatin1String("convert")) {
        return runConvert(arguments);
    }

    if (command == QLatin1String("batch")) {
        return runBatch(arguments);
    }

    err() << CliParser::usage();
    return 2;
}

int CliCommands::runConvert(const QStringList &arguments)
{
    if (arguments.size() < 4) {
        err() << CliParser::usage();
        return 2;
    }

    const QString inputPath = QFileInfo(arguments.at(2)).absoluteFilePath();
    const QString outputPath = QFileInfo(arguments.at(3)).absoluteFilePath();

    darblp::ConversionOptions options;
    options.outputFormat = CliParser::formatFromPath(outputPath);
    options.outputFolder = QFileInfo(outputPath).absolutePath();
    options.generateMipmaps = false;

    if (!applyCommonOptions(arguments, 4, options)) {
        return 2;
    }

    darblp::Converter converter;
    const darblp::ConversionResult result = converter.convertFile(inputPath, options, outputPath);
    if (result.success) {
        out() << "Converted: " << result.outputPath << '\n';
        return 0;
    }

    err() << "Conversion failed: " << result.errorMessage << '\n';
    return 1;
}

int CliCommands::runBatch(const QStringList &arguments)
{
    if (arguments.size() < 6) {
        err() << CliParser::usage();
        return 2;
    }

    const QString inputFolder = QFileInfo(arguments.at(2)).absoluteFilePath();
    const QString outputFolder = QFileInfo(arguments.at(3)).absoluteFilePath();
    if (!QFileInfo(inputFolder).isDir()) {
        err() << "Input folder does not exist: " << inputFolder << '\n';
        return 2;
    }

    darblp::ConversionOptions options;
    options.outputFolder = outputFolder;
    options.sourceRoot = inputFolder;
    options.generateMipmaps = false;

    if (!applyCommonOptions(arguments, 4, options)) {
        return 2;
    }

    const QStringList files = darblp::FileUtils::collectSupportedFiles(inputFolder);
    if (files.isEmpty()) {
        err() << "No supported textures found.\n";
        return 1;
    }

    darblp::Converter converter;
    const QVector<darblp::ConversionResult> results = converter.convertBatch(files, options);

    int successCount = 0;
    int errorCount = 0;
    for (const darblp::ConversionResult &result : results) {
        if (result.success) {
            ++successCount;
            out() << "OK  " << result.outputPath << '\n';
        } else {
            ++errorCount;
            err() << "ERR " << result.inputPath << ": " << result.errorMessage << '\n';
        }
    }

    out() << "Finished: " << successCount << " success, " << errorCount << " error(s), output: " << outputFolder << '\n';
    return errorCount == 0 ? 0 : 1;
}

} // namespace darblp::cli
