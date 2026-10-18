---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/Converter.cpp"
ref: HEAD
---

#include "core/Converter.h"

#include "blp/BlpReader.h"
#include "blp/BlpWriter.h"
#include "image/PngCodec.h"
#include "image/TgaCodec.h"
#include "utils/FileUtils.h"

#include <QDir>
#include <QFileInfo>
#include <QImage>

namespace darblp {

ConversionResult Converter::convertFile(const QString &inputPath,
                                        const ConversionOptions &options,
                                        const QString &explicitOutputPath) const
{
    ConversionResult result;
    result.inputPath = inputPath;
    result.outputPath = buildOutputPath(inputPath, options, explicitOutputPath);

    ImageData image;
    QString error;
    if (!loadImage(inputPath, image, &error)) {
        result.errorMessage = error;
        return result;
    }

    QImage exportImage = prepareForExport(image.image, options);
    if (exportImage.isNull()) {
        result.errorMessage = QStringLiteral("Image is empty after export preparation.");
        return result;
    }

    QFileInfo outputInfo(result.outputPath);
    QDir().mkpath(outputInfo.absolutePath());

    bool written = false;
    switch (options.outputFormat) {
    case OutputFormat::Png:
        written = PngCodec::write(result.outputPath, exportImage, &error);
        break;
    case OutputFormat::Tga:
        written = TgaCodec::write(result.outputPath, exportImage, &error);
        break;
    case OutputFormat::Blp:
        written = BlpWriter::writeImage(result.outputPath, exportImage, options, &error);
        break;
    }

    result.success = written;
    if (!written) {
        result.errorMessage = error;
    }
    return result;
}

QVector<ConversionResult> Converter::convertBatch(const QStringList &inputPaths,
                                                 const ConversionOptions &options) const
{
    QVector<ConversionResult> results;
    results.reserve(inputPaths.size());
    for (const QString &path : inputPaths) {
        results.push_back(convertFile(path, options));
    }
    return results;
}

bool Converter::loadImage(const QString &path, ImageData &image, QString *errorMessage) const
{
    const QString suffix = QFileInfo(path).suffix().toLower();
    bool ok = false;
    QImage qimage;

    if (suffix == QLatin1String("png")) {
        ok = PngCodec::read(path, qimage, errorMessage);
    } else if (suffix == QLatin1String("tga")) {
        ok = TgaCodec::read(path, qimage, errorMessage);
    } else if (suffix == QLatin1String("blp")) {
        ok = BlpReader::readImage(path, qimage, errorMessage);
    } else if (errorMessage) {
        *errorMessage = QStringLiteral("Unsupported file extension: %1").arg(suffix);
    }

    if (!ok || qimage.isNull()) {
        return false;
    }

    image = ImageData(qimage.convertToFormat(QImage::Format_RGBA8888));
    image.sourcePath = path;
    image.format = suffix.toUpper();
    image.fileSize = QFileInfo(path).size();
    image.alphaDepth = image.hasAlpha() ? 8 : 0;
    image.compression = QStringLiteral("Uncompressed");
    return true;
}

QString Converter::buildOutputPath(const QString &inputPath,
                                   const ConversionOptions &options,
                                   const QString &explicitOutputPath) const
{
    if (!explicitOutputPath.isEmpty()) {
        return QFileInfo(explicitOutputPath).absoluteFilePath();
    }

    const QFileInfo inputInfo(inputPath);
    QString outputRoot = options.outputFolder.trimmed();
    if (outputRoot.isEmpty()) {
        outputRoot = inputInfo.absolutePath();
    }

    QString relativeName = inputInfo.completeBaseName() + QLatin1Char('.') + toExtension(options.outputFormat);
    if (options.keepFolderStructure && !options.sourceRoot.isEmpty()) {
        QDir sourceDir(options.sourceRoot);
        const QString relativeInput = sourceDir.relativeFilePath(inputInfo.absoluteFilePath());
        const QFileInfo relativeInfo(relativeInput);
        relativeName = relativeInfo.path() == QLatin1String(".")
            ? relativeInfo.completeBaseName() + QLatin1Char('.') + toExtension(options.outputFormat)
            : relativeInfo.path() + QLatin1Char('/') + relativeInfo.completeBaseName() + QLatin1Char('.') + toExtension(options.outputFormat);
    }

    return QDir(outputRoot).absoluteFilePath(relativeName);
}

QImage Converter::prepareForExport(const QImage &image, const ConversionOptions &options)
{
    QImage prepared = options.preserveAlpha
        ? image.convertToFormat(QImage::Format_RGBA8888)
        : image.convertToFormat(QImage::Format_RGB888);

    if (!options.resizeToPowerOfTwo) {
        return prepared;
    }

    const int width = nextPowerOfTwo(prepared.width());
    const int height = nextPowerOfTwo(prepared.height());
    if (width == prepared.width() && height == prepared.height()) {
        return prepared;
    }

    return prepared.scaled(width, height, Qt::IgnoreAspectRatio, Qt::SmoothTransformation);
}

int Converter::nextPowerOfTwo(int value)
{
    if (value <= 1) {
        return 1;
    }

    int power = 1;
    while (power < value) {
        power <<= 1;
    }
    return power;
}

} // namespace darblp
