---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "main.cpp"
ref: HEAD
---

#include <QApplication>
#include <QTranslator>
#include <QLibraryInfo>
#include <QDir>
#include "mainwindow.h"

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    app.setApplicationName("War3UiBuilder");
    app.setApplicationVersion("1.0.0");
    app.setOrganizationName("War3UiBuilder");

    // Ensure Qt image format plugins (png, jpg, tga, etc.) are found
    app.addLibraryPath(QLibraryInfo::path(QLibraryInfo::PluginsPath));

    // Load Chinese translation
    QTranslator translator;
    if (translator.load(":/war3uibuilder_zh_CN.qm")) {
        app.installTranslator(&translator);
    }

    MainWindow w;
    w.show();

    return app.exec();
}
