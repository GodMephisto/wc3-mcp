---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/main.cpp"
ref: HEAD
---

#include "cli/CliCommands.h"
#include "gui/MainWindow.h"
#include "gui/Theme.h"

#include <QApplication>
#include <QCommandLineParser>

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    QApplication::setApplicationName(QStringLiteral("DarBLP Converter"));
    QApplication::setApplicationVersion(QStringLiteral("0.1.0"));
    QApplication::setOrganizationName(QStringLiteral("DarBLP"));

    const QStringList args = app.arguments();
    if (args.size() > 1 && darblp::cli::CliCommands::isCliCommand(args.at(1))) {
        return darblp::cli::CliCommands::run(args);
    }

    darblp::gui::Theme::apply(app);

    darblp::gui::MainWindow window;
    window.resize(1280, 860);
    window.show();

    return app.exec();
}
