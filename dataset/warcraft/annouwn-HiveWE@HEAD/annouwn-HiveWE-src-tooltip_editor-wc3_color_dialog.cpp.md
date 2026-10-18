---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/tooltip_editor/wc3_color_dialog.cpp"
ref: HEAD
---

#include "wc3_color_dialog.h"

#include <QDialog>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLineEdit>
#include <QPushButton>
#include <QLabel>
#include <QColorDialog>
#include <QDialogButtonBox>
#include <QTextEdit>
#include <QTextCursor>
#include <QTextCharFormat>
#include <QTextBlock>
#include <QTextFragment>

namespace {
	// Parse the user-provided string into a QColor.
	// Accepts: |cffRRGGBB  cffRRGGBB  ffRRGGBB  RRGGBB  (case-insensitive)
	QColor parse_wc3_string(const QString& raw) {
		QString t = raw.trimmed().toLower();

		// Strip leading |c, |, or c
		if (t.startsWith("|c")) {
			t = t.mid(2);
		} else if (t.startsWith('|')) {
			t = t.mid(1);
		} else if (t.startsWith('c')) {
			t = t.mid(1);
		}

		// t is now either AARRGGBB (8 hex chars) or RRGGBB (6 hex chars)
		bool ok;
		if (t.size() >= 8) {
			// AARRGGBB — alpha is ignored since QColor doesn't affect WC3 rendering
			const int rr = t.mid(2, 2).toInt(&ok, 16);
			if (!ok) return {};
			const int gg = t.mid(4, 2).toInt(&ok, 16);
			if (!ok) return {};
			const int bb = t.mid(6, 2).toInt(&ok, 16);
			if (!ok) return {};
			return QColor(rr, gg, bb);
		}
		if (t.size() == 6) {
			const int rr = t.mid(0, 2).toInt(&ok, 16);
			if (!ok) return {};
			const int gg = t.mid(2, 2).toInt(&ok, 16);
			if (!ok) return {};
			const int bb = t.mid(4, 2).toInt(&ok, 16);
			if (!ok) return {};
			return QColor(rr, gg, bb);
		}
		return {};
	}

	QString color_to_wc3(const QColor& c) {
		return QString("cff%1%2%3")
		    .arg(c.red(),   2, 16, QChar('0'))
		    .arg(c.green(), 2, 16, QChar('0'))
		    .arg(c.blue(),  2, 16, QChar('0'));
	}
} // namespace

QString strip_wc3_codes(const QString& text) {
	QString result;
	result.reserve(text.size());
	int i = 0;
	while (i < text.size()) {
		if (text[i] == '|' && i + 1 < text.size()) {
			const char next = text[i + 1].toLatin1();
			if (next == 'c' && i + 10 <= text.size()) {
				i += 10;  // Skip |cAARRGGBB
				continue;
			}
			if (next == 'r') {
				i += 2;  // Skip |r
				continue;
			}
			// |n and anything else: keep as-is
		}
		result += text[i];
		++i;
	}
	return result;
}

QColor parse_wc3_color(const QString& code) {
	return parse_wc3_string(code);
}

void parse_wc3_to_doc(const QString& wc3, QTextEdit* edit) {
	edit->clear();
	QTextCursor cursor(edit->document());

	QTextCharFormat default_fmt;
	QColor current_color;  // invalid = no color active
	QString buf;

	auto flush = [&]() {
		if (buf.isEmpty()) {
			return;
		}
		QTextCharFormat fmt;
		if (current_color.isValid()) {
			fmt.setForeground(current_color);
		}
		cursor.insertText(buf, fmt);
		buf.clear();
	};

	int i = 0;
	while (i < wc3.size()) {
		if (wc3[i] == '|' && i + 1 < wc3.size()) {
			const char next = wc3[i + 1].toLatin1();
			if (next == 'c' && i + 10 <= wc3.size()) {
				const QString hex = wc3.mid(i + 2, 8);
				bool ok1, ok2, ok3;
				const int rr = hex.mid(2, 2).toInt(&ok1, 16);
				const int gg = hex.mid(4, 2).toInt(&ok2, 16);
				const int bb = hex.mid(6, 2).toInt(&ok3, 16);
				if (ok1 && ok2 && ok3) {
					flush();
					current_color = QColor(rr, gg, bb);
					i += 10;
					continue;
				}
			} else if (next == 'r') {
				flush();
				current_color = QColor();  // invalidate = end color
				i += 2;
				continue;
			} else if (next == 'n') {
				flush();
				cursor.insertBlock();
				i += 2;
				continue;
			}
		}
		if (wc3[i] == '\n') {
			flush();
			cursor.insertBlock();
		} else {
			buf += wc3[i];
		}
		++i;
	}
	flush();
}

QString doc_to_wc3(const QTextEdit* edit) {
	QString result;
	const QTextDocument* doc = edit->document();
	bool first_block = true;
	bool color_open = false;
	QColor last_color;

	for (QTextBlock block = doc->begin(); block != doc->end(); block = block.next()) {
		if (!first_block) {
			if (color_open) {
				result += "|r";
				color_open = false;
				last_color = QColor();
			}
			result += "|n";
		}
		first_block = false;

		for (auto it = block.begin(); !it.atEnd(); ++it) {
			const QTextFragment frag = it.fragment();
			if (!frag.isValid()) {
				continue;
			}
			const QTextCharFormat fmt = frag.charFormat();
			const bool has_color = fmt.hasProperty(QTextFormat::ForegroundBrush)
			                       && fmt.foreground().style() != Qt::NoBrush;
			const QColor frag_color = has_color ? fmt.foreground().color() : QColor();

			if (frag_color != last_color) {
				if (color_open) {
					result += "|r";
					color_open = false;
				}
				if (frag_color.isValid()) {
					result += QString("|cff%1%2%3")
					               .arg(frag_color.red(),   2, 16, QChar('0'))
					               .arg(frag_color.green(), 2, 16, QChar('0'))
					               .arg(frag_color.blue(),  2, 16, QChar('0'));
					color_open = true;
				}
				last_color = frag_color;
			}
			result += frag.text();
		}
	}

	if (color_open) {
		result += "|r";
	}
	return result;
}

QColor pick_wc3_color(QWidget* parent, const QColor& initial) {
	QDialog dialog(parent);
	dialog.setWindowTitle("Pick Color");
	dialog.setMinimumWidth(380);

	auto* layout = new QVBoxLayout(&dialog);
	layout->setSpacing(8);

	layout->addWidget(new QLabel("WC3 color code (e.g. <b>cff4ccff6</b> or <b>|cffRRGGBB</b>):"));

	auto* input_row = new QHBoxLayout;
	auto* wc3_input = new QLineEdit;
	wc3_input->setPlaceholderText("cffRRGGBB");
	if (initial.isValid()) {
		wc3_input->setText(color_to_wc3(initial));
	}
	input_row->addWidget(wc3_input);

	auto* pick_btn = new QPushButton("Pick Visually...");
	input_row->addWidget(pick_btn);
	layout->addLayout(input_row);

	auto* feedback = new QLabel;
	feedback->setFixedHeight(24);
	feedback->setStyleSheet("border-radius: 4px;");
	layout->addWidget(feedback);

	// Live color swatch: update as the user types
	auto update_swatch = [&]() {
		const QColor c = parse_wc3_string(wc3_input->text());
		if (c.isValid()) {
			const QString fg = (c.lightness() > 128) ? "black" : "white";
			feedback->setStyleSheet(QString("background:%1; border-radius:4px; color:%2;").arg(c.name(), fg));
			feedback->setText("  " + c.name().toUpper());
		} else {
			feedback->setStyleSheet("border-radius: 4px;");
			feedback->setText(wc3_input->text().isEmpty() ? "" : "  Invalid code");
		}
	};
	update_swatch();

	QObject::connect(wc3_input, &QLineEdit::textChanged, [&](const QString&) { update_swatch(); });

	// Visual picker: fills the WC3 code field from the chosen color
	QObject::connect(pick_btn, &QPushButton::clicked, [&]() {
		QColor current = parse_wc3_string(wc3_input->text());
		if (!current.isValid()) {
			current = initial.isValid() ? initial : Qt::white;
		}
		const QColor chosen = QColorDialog::getColor(current, &dialog, "Pick Color");
		if (chosen.isValid()) {
			wc3_input->setText(color_to_wc3(chosen));
		}
	});

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	layout->addWidget(buttons);

	QObject::connect(buttons, &QDialogButtonBox::accepted, &dialog, &QDialog::accept);
	QObject::connect(buttons, &QDialogButtonBox::rejected, &dialog, &QDialog::reject);

	if (dialog.exec() != QDialog::Accepted) {
		return {};
	}
	return parse_wc3_string(wc3_input->text());
}
