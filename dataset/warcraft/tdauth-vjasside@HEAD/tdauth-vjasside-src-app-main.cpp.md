---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/main.cpp"
ref: HEAD
---

#include "mainwindow.h"

#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    MainWindow w;
    w.show();
    return a.exec();
}
