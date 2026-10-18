---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/Theme.cpp"
ref: HEAD
---

#include "gui/Theme.h"

#include <QApplication>
#include <QColor>
#include <QPalette>
#include <QString>

namespace darblp::gui {

void Theme::apply(QApplication &app)
{
    app.setStyle(QStringLiteral("Fusion"));

    QPalette palette;
    palette.setColor(QPalette::Window, QColor(QStringLiteral("#16181D")));
    palette.setColor(QPalette::WindowText, QColor(QStringLiteral("#E8E8E8")));
    palette.setColor(QPalette::Base, QColor(QStringLiteral("#20232A")));
    palette.setColor(QPalette::AlternateBase, QColor(QStringLiteral("#252932")));
    palette.setColor(QPalette::Text, QColor(QStringLiteral("#E8E8E8")));
    palette.setColor(QPalette::Button, QColor(QStringLiteral("#252932")));
    palette.setColor(QPalette::ButtonText, QColor(QStringLiteral("#E8E8E8")));
    palette.setColor(QPalette::Highlight, QColor(QStringLiteral("#F28E26")));
    palette.setColor(QPalette::HighlightedText, QColor(QStringLiteral("#16181D")));
    app.setPalette(palette);

    app.setStyleSheet(QStringLiteral(R"(
        QWidget {
            background: #16181D;
            color: #E8E8E8;
            font-family: "Segoe UI", "Inter", "Arial";
            font-size: 10pt;
        }

        QLabel,
        QCheckBox {
            background: transparent;
        }

        QWidget#TransparentPanel {
            background: transparent;
        }

        QFrame#Panel, QGroupBox {
            background: #20232A;
            border: 1px solid #2E333D;
            border-radius: 8px;
        }

        QSplitter::handle {
            background: #16181D;
        }

        QSplitter::handle:hover {
            background: #2E333D;
        }

        QGroupBox {
            margin-top: 18px;
            padding-top: 14px;
            font-weight: 600;
        }

        QGroupBox::title {
            subcontrol-origin: margin;
            left: 12px;
            padding: 0 4px;
            color: #FAD760;
        }

        QListView, QTextEdit, QLineEdit, QComboBox {
            background: #252932;
            border: 1px solid #343946;
            border-radius: 7px;
            padding: 6px;
            selection-background-color: #F28E26;
            selection-color: #16181D;
        }

        QLineEdit, QComboBox {
            min-height: 20px;
        }

        QTextEdit {
            padding: 8px;
        }

        QListView::item {
            border-radius: 6px;
            padding: 7px;
            margin: 2px;
        }

        QListView::item:hover {
            background: #2E333D;
        }

        QListView::item:selected {
            background: #F28E26;
            color: #16181D;
        }

        QPushButton, QToolButton {
            background: #252932;
            border: 1px solid #3A404D;
            border-radius: 7px;
            padding: 7px 12px;
            min-height: 24px;
        }

        QPushButton:hover, QToolButton:hover {
            background: #303641;
            border-color: #F28E26;
        }

        QPushButton:pressed, QToolButton:pressed {
            background: #F28E26;
            color: #16181D;
        }

        QToolButton:checked {
            background: #F28E26;
            color: #16181D;
            border-color: #F28E26;
        }

        QPushButton:disabled, QToolButton:disabled, QComboBox:disabled, QLineEdit:disabled, QCheckBox:disabled {
            color: #6F7683;
            background: #20232A;
            border-color: #2E333D;
        }

        QPushButton#PrimaryButton {
            background: #F28E26;
            color: #16181D;
            border-color: #F28E26;
            font-weight: 700;
        }

        QPushButton#PrimaryButton:hover {
            background: #FAD760;
            border-color: #FAD760;
        }

        QCheckBox {
            spacing: 8px;
            min-height: 22px;
        }

        QCheckBox::indicator {
            width: 16px;
            height: 16px;
            border-radius: 4px;
            border: 1px solid #3A404D;
            background: #252932;
        }

        QCheckBox::indicator:checked {
            background: #F28E26;
            border-color: #F28E26;
        }

        QProgressBar {
            background: #252932;
            border: 1px solid #343946;
            border-radius: 7px;
            text-align: center;
            min-height: 22px;
        }

        QProgressBar::chunk {
            background: #F28E26;
            border-radius: 7px;
        }

        QLabel#Muted {
            color: #9DA3AF;
        }

        QScrollBar:vertical {
            background: #20232A;
            width: 12px;
            margin: 0;
        }

        QScrollBar::handle:vertical {
            background: #3A404D;
            border-radius: 6px;
            min-height: 28px;
        }

        QScrollBar::handle:vertical:hover {
            background: #4B5565;
        }

        QScrollBar::add-line:vertical,
        QScrollBar::sub-line:vertical {
            height: 0;
        }
    )"));
}

} // namespace darblp::gui
