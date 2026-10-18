---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/tooltip_editor/wc3_color_dialog.h"
ref: HEAD
---

#pragma once

#include <QColor>
#include <QString>

class QWidget;
class QTextEdit;

// Opens a color picker that accepts both visual selection and WC3 hex input (e.g. cff4ccff6).
// Returns an invalid QColor if the user cancels.
QColor pick_wc3_color(QWidget* parent = nullptr, const QColor& initial = Qt::white);

// Removes all WC3 color codes (|cAARRGGBB and |r) from text, leaving only visible content.
QString strip_wc3_codes(const QString& text);

// Parses a WC3 color code string (|cffRRGGBB, cffRRGGBB, ffRRGGBB, or RRGGBB) into a QColor.
QColor parse_wc3_color(const QString& code);

// Parses a WC3 format string into a QTextEdit document (rich text, no code characters).
void parse_wc3_to_doc(const QString& wc3, QTextEdit* edit);

// Serializes a QTextEdit document back to a WC3 format string.
QString doc_to_wc3(const QTextEdit* edit);
