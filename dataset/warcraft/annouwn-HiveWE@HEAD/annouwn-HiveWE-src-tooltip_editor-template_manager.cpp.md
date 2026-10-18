---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/tooltip_editor/template_manager.cpp"
ref: HEAD
---

#include "template_manager.h"
#include "wc3_color_dialog.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QSplitter>
#include <QPushButton>
#include <QLabel>
#include <QSettings>
#include <QMessageBox>
#include <QToolBar>
#include <QTextEdit>
#include <QTextCharFormat>
#include <QTextCursor>

namespace {
	struct ColorPreset {
		const char* label;
		const char* code;
		const char* bg;
		const char* fg;
	};

	constexpr std::array COLOR_PRESETS = {
		ColorPreset{"Gold",       "|cffffcc00", "#ffcc00", "black"},
		ColorPreset{"White",      "|cffffffff", "#e8e8e8", "black"},
		ColorPreset{"Red",        "|cffff0303", "#ff0303", "white"},
		ColorPreset{"Green",      "|cff1ce6b9", "#1ce6b9", "black"},
		ColorPreset{"Blue",       "|cff0042ff", "#0042ff", "white"},
		ColorPreset{"Orange",     "|cffff8000", "#ff8000", "black"},
		ColorPreset{"Purple",     "|cff9b00ff", "#9b00ff", "white"},
		ColorPreset{"Dark Red",   "|cff9b0000", "#9b0000", "white"},
		ColorPreset{"Cyan",       "|cff00ffff", "#00ffff", "black"},
		ColorPreset{"Dark Green", "|cff008000", "#008000", "white"},
	};
} // namespace

QString TemplateManager::wc3_to_html(const QString& text) {
	QString html;
	html.reserve(text.size() * 2);
	int i = 0;
	while (i < text.size()) {
		if (text[i] == '|' && i + 1 < text.size()) {
			const char next = text[i + 1].toLatin1();
			if (next == 'c' && i + 10 <= text.size()) {
				const QString hex = text.mid(i + 2, 8);
				if (hex.size() == 8) {
					html += "<span style=\"color:#" + hex.mid(2, 2) + hex.mid(4, 2) + hex.mid(6, 2) + "\">";
					i += 10;
					continue;
				}
			} else if (next == 'r') {
				html += "</span>";
				i += 2;
				continue;
			} else if (next == 'n') {
				html += "<br>";
				i += 2;
				continue;
			}
		}
		if (text[i] == '\n') {
			html += "<br>";
		} else if (text[i] == '<') {
			html += "&lt;";
		} else if (text[i] == '>') {
			html += "&gt;";
		} else if (text[i] == '&') {
			html += "&amp;";
		} else {
			html += text[i];
		}
		++i;
	}
	return html;
}

void TemplateManager::insert_color_code(const QString& code) {
	if (!active_field) {
		return;
	}
	const QColor color = parse_wc3_color(code);
	QTextCursor cursor = active_field->textCursor();
	if (cursor.hasSelection()) {
		QTextCharFormat fmt;
		if (color.isValid()) {
			fmt.setForeground(color);
		} else {
			fmt.setForeground(QBrush(Qt::NoBrush));
		}
		cursor.mergeCharFormat(fmt);
	} else {
		if (color.isValid()) {
			QTextCharFormat fmt;
			fmt.setForeground(color);
			active_field->setCurrentCharFormat(fmt);
		}
	}
	active_field->setFocus();
}

int TemplateManager::favorite_count() const {
	int count = 0;
	for (const auto& t : templates) {
		if (t.favorite) {
			++count;
		}
	}
	return count;
}

void TemplateManager::update_fav_ui() {
	const int count = favorite_count();
	fav_count_label->setText(QString("Favorites: %1/5").arg(count));
	if (current_index >= 0 && current_index < static_cast<int>(templates.size())) {
		const bool is_fav = templates[current_index].favorite;
		const QSignalBlocker b(fav_btn);
		fav_btn->setChecked(is_fav);
		fav_btn->setText(is_fav ? "★ Favorite" : "☆ Favorite");
	}
}

void TemplateManager::load_from_settings() {
	QSettings settings;
	settings.beginGroup("TooltipEditor");
	const int count = settings.beginReadArray("templates");
	templates.clear();
	templates.reserve(count);
	for (int i = 0; i < count; ++i) {
		settings.setArrayIndex(i);
		templates.push_back({
			settings.value("name").toString(),
			settings.value("tip_text").toString(),
			settings.value("text").toString(),
			settings.value("favorite", false).toBool()
		});
	}
	settings.endArray();
	settings.endGroup();
}

void TemplateManager::save_to_settings() const {
	QSettings settings;
	settings.beginGroup("TooltipEditor");
	settings.beginWriteArray("templates");
	for (int i = 0; i < static_cast<int>(templates.size()); ++i) {
		settings.setArrayIndex(i);
		settings.setValue("name", templates[i].name);
		settings.setValue("tip_text", templates[i].tip_text);
		settings.setValue("text", templates[i].text);
		settings.setValue("favorite", templates[i].favorite);
	}
	settings.endArray();
	settings.endGroup();
}

void TemplateManager::refresh_list() {
	const QSignalBlocker blocker(template_list);
	template_list->clear();
	for (const auto& t : templates) {
		template_list->addItem((t.favorite ? "★ " : "") + t.name);
	}
}

void TemplateManager::load_template(int index) {
	if (index < 0 || index >= static_cast<int>(templates.size())) {
		return;
	}
	current_index = index;
	loading = true;
	name_edit->setText(templates[index].name);
	parse_wc3_to_doc(templates[index].tip_text, tip_edit);
	parse_wc3_to_doc(templates[index].text, text_edit);
	loading = false;
	update_preview();
	update_fav_ui();
}

void TemplateManager::commit_current() {
	if (loading || current_index < 0 || current_index >= static_cast<int>(templates.size())) {
		return;
	}
	templates[current_index].name = name_edit->text().trimmed();
	templates[current_index].tip_text = doc_to_wc3(tip_edit);
	templates[current_index].text = doc_to_wc3(text_edit);

	if (auto* item = template_list->item(current_index)) {
		const QSignalBlocker blocker(template_list);
		const bool is_fav = templates[current_index].favorite;
		item->setText((is_fav ? "★ " : "") + templates[current_index].name);
	}
	save_to_settings();
}

void TemplateManager::update_preview() {
	const QString tip = doc_to_wc3(tip_edit);
	const QString ubertip = doc_to_wc3(text_edit);

	QString html;
	if (!tip.isEmpty()) {
		html += "<b>" + wc3_to_html(tip) + "</b>";
		if (!ubertip.isEmpty()) {
			html += "<hr style='border:0; border-top:1px solid #444; margin:4px 0;'/>";
		}
	}
	if (!ubertip.isEmpty()) {
		html += wc3_to_html(ubertip);
	}
	preview->setHtml(html);
}

TemplateManager::TemplateManager(QWidget* parent)
	: QWidget(parent) {
	setWindowTitle("Template Manager");
	setWindowFlags(Qt::Window);
	setMinimumSize(700, 520);

	load_from_settings();

	auto* root_layout = new QVBoxLayout(this);
	root_layout->setContentsMargins(4, 4, 4, 4);
	root_layout->setSpacing(4);

	// ── Color toolbar ──────────────────────────────────────────────────────
	auto* color_bar = new QWidget;
	auto* color_bar_layout = new QHBoxLayout(color_bar);
	color_bar_layout->setContentsMargins(0, 0, 0, 0);
	color_bar_layout->setSpacing(3);

	for (const auto& p : COLOR_PRESETS) {
		auto* btn = new QPushButton(QString::fromUtf8(p.label));
		btn->setFixedWidth(72);
		btn->setFixedHeight(22);
		btn->setToolTip(QString("Insert %1 color code (%2)").arg(p.label, p.code));
		btn->setStyleSheet(
			QString("QPushButton{background:%1;color:%2;border-radius:3px;padding:1px 4px;font-size:11px;}"
			        "QPushButton:hover{border:1px solid rgba(255,255,255,140);}")
				.arg(p.bg, p.fg));
		const QString code = QString::fromUtf8(p.code);
		connect(btn, &QPushButton::clicked, this, [this, code]() { insert_color_code(code); });
		color_bar_layout->addWidget(btn);
	}

	color_bar_layout->addSpacing(6);

	auto* custom_btn = new QPushButton("Custom...");
	custom_btn->setFixedHeight(22);
	custom_btn->setToolTip("Pick a custom color");
	connect(custom_btn, &QPushButton::clicked, this, [this]() {
		const QColor color = pick_wc3_color(this);
		if (!color.isValid()) {
			return;
		}
		const QString code = QString("|cff%1%2%3")
		                         .arg(color.red(), 2, 16, QChar('0'))
		                         .arg(color.green(), 2, 16, QChar('0'))
		                         .arg(color.blue(), 2, 16, QChar('0'));
		insert_color_code(code);
	});
	color_bar_layout->addWidget(custom_btn);
	color_bar_layout->addStretch();

	root_layout->addWidget(color_bar);

	// ── Main splitter ──────────────────────────────────────────────────────
	auto* splitter = new QSplitter(Qt::Horizontal);
	root_layout->addWidget(splitter);

	// Left panel: list + action buttons
	auto* left_panel = new QWidget;
	auto* left_layout = new QVBoxLayout(left_panel);
	left_layout->setContentsMargins(4, 4, 4, 4);
	left_layout->setSpacing(4);

	left_layout->addWidget(new QLabel("Templates:"));
	template_list = new QListWidget;
	left_layout->addWidget(template_list);

	auto* btn_row = new QHBoxLayout;
	auto* add_btn = new QPushButton("New");
	auto* del_btn = new QPushButton("Delete");
	btn_row->addWidget(add_btn);
	btn_row->addWidget(del_btn);
	left_layout->addLayout(btn_row);

	fav_count_label = new QLabel("Favorites: 0/5");
	fav_count_label->setStyleSheet("color: gray; font-size: 11px;");
	left_layout->addWidget(fav_count_label);

	auto* apply_btn = new QPushButton("Apply Template");
	apply_btn->setStyleSheet("QPushButton { background-color: #3a6ba0; color: white; padding: 5px; border-radius: 3px; }"
	                         "QPushButton:hover { background-color: #4a7bb0; }");
	left_layout->addWidget(apply_btn);

	left_panel->setMinimumWidth(160);
	left_panel->setMaximumWidth(220);

	// Right panel: name + favorite + tip + ubertip + preview
	auto* right_panel = new QWidget;
	auto* right_layout = new QVBoxLayout(right_panel);
	right_layout->setContentsMargins(4, 4, 4, 4);
	right_layout->setSpacing(6);

	right_layout->addWidget(new QLabel("Template Name:"));
	name_edit = new QLineEdit;
	name_edit->setPlaceholderText("Template name...");
	right_layout->addWidget(name_edit);

	fav_btn = new QPushButton("☆ Favorite");
	fav_btn->setCheckable(true);
	fav_btn->setToolTip("Mark this template as a favorite (max 5). Favorites appear as quick-apply buttons in the Tooltip Editor.");
	right_layout->addWidget(fav_btn);

	right_layout->addWidget(new QLabel("Tooltip Normal:"));
	tip_edit = new QTextEdit;
	tip_edit->setPlaceholderText("Optional title/name text...");
	tip_edit->setMaximumHeight(60);
	right_layout->addWidget(tip_edit);

	right_layout->addWidget(new QLabel("Tooltip Extended:"));
	text_edit = new QTextEdit;
	text_edit->setPlaceholderText("Body text...");
	right_layout->addWidget(text_edit);

	right_layout->addWidget(new QLabel("Preview:"));
	preview = new QTextBrowser;
	preview->setMaximumHeight(120);
	preview->setStyleSheet("background: #1c1c1c; color: #ffffff; border-radius: 4px;");
	right_layout->addWidget(preview);

	splitter->addWidget(left_panel);
	splitter->addWidget(right_panel);
	splitter->setStretchFactor(0, 0);
	splitter->setStretchFactor(1, 1);

	refresh_list();
	update_fav_ui();

	// ── Signals ────────────────────────────────────────────────────────────

	connect(template_list, &QListWidget::currentRowChanged, this, [this](int row) {
		commit_current();
		load_template(row);
	});

	connect(add_btn, &QPushButton::clicked, this, [this]() {
		// If nothing is selected, seed the new template with whatever is in the fields
		QString initial_name = (current_index < 0) ? name_edit->text().trimmed() : "";
		QString initial_tip = (current_index < 0) ? doc_to_wc3(tip_edit) : "";
		QString initial_text = (current_index < 0) ? doc_to_wc3(text_edit) : "";
		if (initial_name.isEmpty()) {
			initial_name = "New Template";
		}

		commit_current();
		templates.push_back({initial_name, initial_tip, initial_text, false});
		save_to_settings();
		refresh_list();
		template_list->setCurrentRow(static_cast<int>(templates.size()) - 1);
	});

	connect(del_btn, &QPushButton::clicked, this, [this]() {
		const int row = template_list->currentRow();
		if (row < 0 || row >= static_cast<int>(templates.size())) {
			return;
		}
		if (QMessageBox::question(this, "Delete Template",
		                          QString("Delete \"%1\"?").arg(templates[row].name))
		    != QMessageBox::Yes) {
			return;
		}
		const bool was_fav = templates[row].favorite;
		templates.erase(templates.begin() + row);
		current_index = -1;
		save_to_settings();
		refresh_list();
		name_edit->clear();
		tip_edit->clear();
		text_edit->clear();
		preview->clear();
		update_fav_ui();
		if (was_fav) {
			emit favorites_changed();
		}
	});

	connect(fav_btn, &QPushButton::clicked, this, [this](bool checked) {
		if (current_index < 0 || current_index >= static_cast<int>(templates.size())) {
			return;
		}
		if (checked && favorite_count() >= 5) {
			const QSignalBlocker b(fav_btn);
			fav_btn->setChecked(false);
			fav_btn->setText("☆ Favorite");
			QMessageBox::information(this, "Favorites Full",
			                         "You can have at most 5 favorite templates.\nRemove a favorite first.");
			return;
		}
		templates[current_index].favorite = checked;
		fav_btn->setText(checked ? "★ Favorite" : "☆ Favorite");
		if (auto* item = template_list->item(current_index)) {
			const QSignalBlocker b(template_list);
			item->setText((checked ? "★ " : "") + templates[current_index].name);
		}
		update_fav_ui();
		save_to_settings();
		emit favorites_changed();
	});

	connect(name_edit, &QLineEdit::textEdited, this, [this]() {
		commit_current();
	});

	auto on_field_changed = [this]() {
		if (loading) {
			return;
		}
		commit_current();
		update_preview();
	};

	connect(tip_edit, &QTextEdit::textChanged, this, on_field_changed);
	connect(text_edit, &QTextEdit::textChanged, this, on_field_changed);

	// Track which field is focused for color button insertion
	connect(tip_edit, &QTextEdit::cursorPositionChanged, this, [this]() {
		active_field = tip_edit;
	});
	connect(text_edit, &QTextEdit::cursorPositionChanged, this, [this]() {
		active_field = text_edit;
	});

	connect(apply_btn, &QPushButton::clicked, this, [this]() {
		if (current_index >= 0 && current_index < static_cast<int>(templates.size())) {
			emit template_apply_requested(
				templates[current_index].tip_text,
				templates[current_index].text
			);
		}
	});

	if (!templates.empty()) {
		template_list->setCurrentRow(0);
	}
}

TemplateManager::~TemplateManager() {
	commit_current();
}

#include "template_manager.moc"
