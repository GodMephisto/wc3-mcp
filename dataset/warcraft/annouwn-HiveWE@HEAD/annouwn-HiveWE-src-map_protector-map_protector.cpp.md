---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/map_protector/map_protector.cpp"
ref: HEAD
---

#include "map_protector.h"

import std;
import Map;
import MapGlobal;

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QGroupBox>
#include <QWidget>
#include <QFileDialog>
#include <QMessageBox>
#include <QSettings>
#include <QMetaObject>
#include <QDir>

namespace {
	QCheckBox* make_check(QGroupBox* group, QVBoxLayout* layout, const QString& text, const QString& tooltip, bool checked) {
		auto* check = new QCheckBox(text, group);
		check->setChecked(checked);
		check->setToolTip(tooltip);
		layout->addWidget(check);
		return check;
	}
} // namespace

MapProtector::MapProtector(QWidget* parent) : QMainWindow(parent) {
	setWindowTitle("Map Protector");
	resize(700, 560);

	auto* central = new QWidget(this);
	setCentralWidget(central);
	auto* main_layout = new QVBoxLayout(central);

	// Source
	auto* source_group = new QGroupBox("Source", central);
	auto* source_layout = new QVBoxLayout(source_group);
	source_current_map_radio = new QRadioButton("Currently loaded map", source_group);
	source_external_radio = new QRadioButton("Map file or folder:", source_group);
	source_layout->addWidget(source_current_map_radio);
	source_layout->addWidget(source_external_radio);

	auto* source_path_row = new QHBoxLayout;
	source_path_row->addSpacing(20);
	source_path_edit = new QLineEdit(source_group);
	source_browse_file_button = new QPushButton("Browse File...", source_group);
	source_browse_folder_button = new QPushButton("Browse Folder...", source_group);
	source_path_row->addWidget(source_path_edit, 1);
	source_path_row->addWidget(source_browse_file_button);
	source_path_row->addWidget(source_browse_folder_button);
	source_layout->addLayout(source_path_row);

	main_layout->addWidget(source_group);

	connect(source_current_map_radio, &QRadioButton::toggled, this, &MapProtector::update_source_controls_enabled);
	connect(source_browse_file_button, &QPushButton::clicked, this, &MapProtector::on_source_browse_file_clicked);
	connect(source_browse_folder_button, &QPushButton::clicked, this, &MapProtector::on_source_browse_folder_clicked);

	// Output path
	auto* output_row = new QHBoxLayout;
	output_row->addWidget(new QLabel("Output file:", central));
	output_path_edit = new QLineEdit(central);
	browse_button = new QPushButton("Browse...", central);
	output_row->addWidget(output_path_edit, 1);
	output_row->addWidget(browse_button);
	main_layout->addLayout(output_row);

	// Keep the output filename in sync with whichever source is actually selected - it must not
	// keep defaulting to the currently-loaded HiveWE map's name once an external source is picked.
	connect(source_current_map_radio, &QRadioButton::toggled, this, &MapProtector::populate_default_output_path);
	connect(source_external_radio, &QRadioButton::toggled, this, &MapProtector::populate_default_output_path);
	connect(source_path_edit, &QLineEdit::textChanged, this, &MapProtector::populate_default_output_path);

	// MPQ archive hardening
	auto* mpq_group = new QGroupBox("MPQ Archive Hardening", central);
	auto* mpq_layout = new QVBoxLayout(mpq_group);
	remove_listfile_check = make_check(mpq_group, mpq_layout, "Remove listfile", "Deletes (listfile) from the MPQ so editors cannot enumerate files.", true);
	remove_attributes_check = make_check(mpq_group, mpq_layout, "Remove attributes file", "Deletes (attributes), the MD5 hash table used by some tools.", true);
	encrypt_files_check = make_check(mpq_group, mpq_layout, "Encrypt MPQ files", "Encrypts every file in the archive (MPQ_FILE_ENCRYPTED). Warcraft III reads encrypted files transparently.", true);
	inject_junk_files_check = make_check(mpq_group, mpq_layout, "Inject junk files", "Adds dummy files with random names and content to confuse deprotection tools.", false);

	auto* junk_count_row = new QHBoxLayout;
	junk_count_row->addSpacing(20);
	junk_count_row->addWidget(new QLabel("Junk file count:", mpq_group));
	junk_file_count_spin = new QSpinBox(mpq_group);
	junk_file_count_spin->setRange(1, 1000);
	junk_file_count_spin->setValue(50);
	junk_file_count_spin->setEnabled(false);
	junk_count_row->addWidget(junk_file_count_spin);
	junk_count_row->addStretch(1);
	mpq_layout->addLayout(junk_count_row);
	connect(inject_junk_files_check, &QCheckBox::toggled, junk_file_count_spin, &QSpinBox::setEnabled);

	main_layout->addWidget(mpq_group);

	// Trigger hardening
	auto* trigger_group = new QGroupBox("Trigger / Script Hardening", central);
	auto* trigger_layout = new QVBoxLayout(trigger_group);
	remove_gui_triggers_check = make_check(trigger_group, trigger_layout, "Remove GUI trigger data", "Deletes war3map.wtg (trigger tree). The map runs on its compiled script; editors show nothing.", true);
	strip_trigger_strings_check = make_check(trigger_group, trigger_layout, "Strip trigger strings", "Inlines war3map.wts text directly into the script and into war3map.w3i's own fields (map name, author, description, loading screen text), then deletes war3map.wts. Note: object data fields (e.g. a very long custom tooltip) can independently reference a trigger string too - those will show raw \"TRIGSTR_XXX\" text in-game after stripping.", false);
	main_layout->addWidget(trigger_group);

	// Asset path obfuscation
	auto* asset_group = new QGroupBox("Asset Path Obfuscation", central);
	auto* asset_layout = new QVBoxLayout(asset_group);
	obfuscate_asset_paths_check = make_check(
		asset_group, asset_layout, "Rename imported files to random names",
		"Renames every custom model, texture, etc. to a random name and rewrites every reference to "
		"it (object data, war3map.w3i, paths inside models, script literals) so the map still plays "
		"correctly - defeats tools that recover filenames via a known-name dictionary (e.g. Ladik's "
		"MPQ Editor). Core map files, stock game assets, and sound files are never renamed. Aborts "
		"the export instead of shipping a map with a broken reference if anything can't be verified.",
		false
	);
	main_layout->addWidget(asset_group);

	// Metadata sanitization
	auto* metadata_group = new QGroupBox("Metadata Sanitization", central);
	auto* metadata_layout = new QVBoxLayout(metadata_group);
	clear_author_check = make_check(metadata_group, metadata_layout, "Clear map author", "Blanks the author field in war3map.w3i.", false);
	clear_description_check = make_check(metadata_group, metadata_layout, "Clear map description", "Blanks the description field.", false);
	clear_loading_text_check = make_check(metadata_group, metadata_layout, "Clear loading screen text", "Blanks the loading screen title, subtitle, and body text.", false);
	normalize_name_check = make_check(metadata_group, metadata_layout, "Normalize map name", "Replaces the map name with a generic placeholder.", false);
	main_layout->addWidget(metadata_group);

	main_layout->addStretch(1);

	// Status / action bar
	auto* action_row = new QHBoxLayout;
	status_label = new QLabel("Ready", central);
	progress_bar = new QProgressBar(central);
	progress_bar->setRange(0, 0);
	progress_bar->setVisible(false);
	progress_bar->setMaximumWidth(160);
	export_button = new QPushButton("Export Protected Map", central);
	action_row->addWidget(status_label, 1);
	action_row->addWidget(progress_bar);
	action_row->addWidget(export_button);
	main_layout->addLayout(action_row);

	connect(browse_button, &QPushButton::clicked, this, &MapProtector::on_browse_clicked);
	connect(export_button, &QPushButton::clicked, this, &MapProtector::on_export_clicked);

	load_settings();
	populate_default_output_path();
	update_source_controls_enabled();
}

void MapProtector::showEvent(QShowEvent* event) {
	QMainWindow::showEvent(event);
	populate_default_output_path();
}

QString MapProtector::default_output_base_name() const {
	if (source_current_map_radio->isChecked()) {
		if (!map || !map->loaded || map->name.empty()) {
			return QString();
		}
		return QString::fromStdString(map->name);
	}

	const QString source_text = source_path_edit->text();
	if (source_text.isEmpty()) {
		return QString();
	}
	// stem() strips only the final extension either way, so a packed "Foo.w3x" file and a
	// folder-mode "Foo.w3x" directory both yield "Foo", and a plain folder name with no
	// extension (e.g. "Foo") is returned unchanged.
	return QString::fromStdWString(fs::path(source_text.toStdWString()).stem().wstring());
}

void MapProtector::populate_default_output_path() {
	// Only overwrite the field if it's empty or still holds a default we generated ourselves -
	// never clobber a path the user typed in by hand.
	const QString current = output_path_edit->text();
	if (!current.isEmpty() && current != auto_generated_output_path) {
		return;
	}

	const QString base_name = default_output_base_name();
	if (base_name.isEmpty()) {
		return;
	}

	// Default next to the source map, not the app-wide "openDirectory" setting: that setting is
	// shared by every open/save dialog in HiveWE and for folder-expansion maps holds the
	// last-opened map's own folder (see HiveWE::load_folder()), which may be a stale path from an
	// unrelated map and isn't guaranteed to exist.
	QString directory;
	if (source_current_map_radio->isChecked()) {
		if (!map || !map->loaded) {
			return;
		}
		if (!map->filesystem_path.empty()) {
			// Map::load()/save() always leave filesystem_path with a trailing separator, which
			// makes filename() empty and a single parent_path() a no-op (it just strips that
			// separator and returns the map's own folder again). Strip it explicitly so
			// parent_path() below lands one level up, next to the map's folder, instead of inside it.
			fs::path map_folder = map->filesystem_path;
			if (map_folder.filename().empty()) {
				map_folder = map_folder.parent_path();
			}
			directory = QString::fromStdWString(map_folder.parent_path().wstring());
		}
	} else {
		const fs::path source_fs_path = source_path_edit->text().toStdWString();
		directory = QString::fromStdWString(source_fs_path.parent_path().wstring());
	}

	if (directory.isEmpty()) {
		QSettings settings;
		directory = settings.value("openDirectory", QDir::current().path()).toString();
	}

	const QString new_path = directory + "/" + base_name + "_protected.w3x";
	output_path_edit->setText(new_path);
	auto_generated_output_path = new_path;
}

void MapProtector::closeEvent(QCloseEvent* event) {
	if (export_in_progress) {
		event->ignore();
		return;
	}
	save_settings();
	QMainWindow::closeEvent(event);
}

void MapProtector::on_browse_clicked() {
	const QString start = output_path_edit->text();
	const QString path = QFileDialog::getSaveFileName(this, "Export Protected Map", start, "Warcraft III Scenario (*.w3x)");
	if (!path.isEmpty()) {
		output_path_edit->setText(path);
	}
}

void MapProtector::on_source_browse_file_clicked() {
	const QString start = source_path_edit->text();
	const QString path = QFileDialog::getOpenFileName(this, "Select Map File", start, "Warcraft III Scenario (*.w3x *.w3m)");
	if (!path.isEmpty()) {
		source_path_edit->setText(path);
	}
}

void MapProtector::on_source_browse_folder_clicked() {
	const QString start = source_path_edit->text();
	const QString path = QFileDialog::getExistingDirectory(this, "Select Map Folder", start, QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);
	if (!path.isEmpty()) {
		source_path_edit->setText(path);
	}
}

void MapProtector::update_source_controls_enabled() {
	const bool external = source_external_radio->isChecked();
	source_path_edit->setEnabled(external);
	source_browse_file_button->setEnabled(external);
	source_browse_folder_button->setEnabled(external);
}

ProtectionOptions MapProtector::collect_options() const {
	ProtectionOptions options;
	options.remove_listfile = remove_listfile_check->isChecked();
	options.remove_attributes = remove_attributes_check->isChecked();
	options.encrypt_files = encrypt_files_check->isChecked();
	options.inject_junk_files = inject_junk_files_check->isChecked();
	options.junk_file_count = junk_file_count_spin->value();
	options.remove_gui_triggers = remove_gui_triggers_check->isChecked();
	options.strip_trigger_strings = strip_trigger_strings_check->isChecked();
	options.obfuscate_asset_paths = obfuscate_asset_paths_check->isChecked();
	options.clear_author = clear_author_check->isChecked();
	options.clear_description = clear_description_check->isChecked();
	options.clear_loading_text = clear_loading_text_check->isChecked();
	options.normalize_name = normalize_name_check->isChecked();
	return options;
}

void MapProtector::set_options_enabled(bool enabled) {
	const std::initializer_list<QWidget*> widgets = {
		source_current_map_radio, source_external_radio,
		remove_listfile_check, remove_attributes_check, encrypt_files_check, inject_junk_files_check,
		remove_gui_triggers_check, strip_trigger_strings_check, obfuscate_asset_paths_check,
		clear_author_check, clear_description_check, clear_loading_text_check, normalize_name_check,
		output_path_edit, browse_button, export_button
	};
	for (QWidget* widget : widgets) {
		widget->setEnabled(enabled);
	}
	// Keep the spinner's enabled state tied to its checkbox rather than blanket-enabling it.
	junk_file_count_spin->setEnabled(enabled && inject_junk_files_check->isChecked());
	// Keep the source path row tied to the radio selection rather than blanket-enabling it.
	update_source_controls_enabled();
}

void MapProtector::on_export_clicked() {
	const bool use_current_map = source_current_map_radio->isChecked();

	fs::path source_path;
	if (use_current_map) {
		if (!map || !map->loaded) {
			QMessageBox::warning(this, "Map Protector", "No map is loaded.");
			return;
		}
	} else {
		const QString source_text = source_path_edit->text();
		if (source_text.isEmpty()) {
			QMessageBox::warning(this, "Map Protector", "Choose a source map file or folder first.");
			return;
		}
		source_path = source_text.toStdWString();
		if (!fs::exists(source_path)) {
			QMessageBox::warning(this, "Map Protector", "The selected source path does not exist.");
			return;
		}
	}

	const QString output = output_path_edit->text();
	if (output.isEmpty()) {
		QMessageBox::warning(this, "Map Protector", "Choose an output file first.");
		return;
	}

	const ProtectionOptions options = collect_options();
	save_settings();

	set_options_enabled(false);
	status_label->setText(use_current_map ? "Saving map data..." : "Preparing source map...");
	progress_bar->setVisible(true);

	temp_dir = std::make_unique<QTemporaryDir>();
	if (!temp_dir->isValid()) {
		status_label->setText("Failed to create a temporary folder.");
		set_options_enabled(true);
		progress_bar->setVisible(false);
		temp_dir.reset();
		return;
	}

	const fs::path temp_path = temp_dir->path().toStdWString();
	const SyncSaveResult save_result = use_current_map
		? run_sync_save_and_restore(temp_path, options)
		: prepare_source_path(source_path, temp_path, options);
	if (!save_result.success) {
		status_label->setText(QString::fromStdString("Error: " + save_result.error));
		set_options_enabled(true);
		progress_bar->setVisible(false);
		temp_dir.reset();
		return;
	}

	status_label->setText("Packing protected map...");
	export_in_progress = true;

	const fs::path output_path = output.toStdWString();

	pack_thread = QThread::create([this, temp_path, output_path, options]() {
		const PackResult result = run_async_pack(temp_path, output_path, options);
		QMetaObject::invokeMethod(this, [this, result]() { on_export_finished(result); }, Qt::QueuedConnection);
	});
	pack_thread->start();
}

void MapProtector::on_export_finished(PackResult result) {
	export_in_progress = false;
	progress_bar->setVisible(false);
	set_options_enabled(true);

	if (result.success) {
		status_label->setText("Done - saved to " + output_path_edit->text());
	} else {
		status_label->setText(QString::fromStdString("Error: " + result.error));
	}

	temp_dir.reset();

	if (pack_thread) {
		pack_thread->wait();
		pack_thread->deleteLater();
		pack_thread = nullptr;
	}
}

void MapProtector::load_settings() {
	QSettings settings;
	settings.beginGroup("MapProtector");
	const bool default_use_current_map = map && map->loaded;
	const bool use_current_map = settings.value("useCurrentMap", default_use_current_map).toBool();
	source_current_map_radio->setChecked(use_current_map);
	source_external_radio->setChecked(!use_current_map);
	source_path_edit->setText(settings.value("sourcePath", "").toString());
	// outputPath is deliberately not restored: it must always default freshly from whichever
	// source is selected (see populate_default_output_path()), not a stale path remembered from a
	// previous, possibly unrelated map.
	remove_listfile_check->setChecked(settings.value("removeListfile", true).toBool());
	remove_attributes_check->setChecked(settings.value("removeAttributes", true).toBool());
	encrypt_files_check->setChecked(settings.value("encryptFiles", true).toBool());
	inject_junk_files_check->setChecked(settings.value("injectJunkFiles", false).toBool());
	junk_file_count_spin->setValue(settings.value("junkFileCount", 50).toInt());
	junk_file_count_spin->setEnabled(inject_junk_files_check->isChecked());
	remove_gui_triggers_check->setChecked(settings.value("removeGuiTriggers", true).toBool());
	strip_trigger_strings_check->setChecked(settings.value("stripTriggerStrings", false).toBool());
	obfuscate_asset_paths_check->setChecked(settings.value("obfuscateAssetPaths", false).toBool());
	clear_author_check->setChecked(settings.value("clearAuthor", false).toBool());
	clear_description_check->setChecked(settings.value("clearDescription", false).toBool());
	clear_loading_text_check->setChecked(settings.value("clearLoadingText", false).toBool());
	normalize_name_check->setChecked(settings.value("normalizeName", false).toBool());
	settings.endGroup();
}

void MapProtector::save_settings() const {
	QSettings settings;
	settings.beginGroup("MapProtector");
	settings.setValue("useCurrentMap", source_current_map_radio->isChecked());
	settings.setValue("sourcePath", source_path_edit->text());
	settings.setValue("removeListfile", remove_listfile_check->isChecked());
	settings.setValue("removeAttributes", remove_attributes_check->isChecked());
	settings.setValue("encryptFiles", encrypt_files_check->isChecked());
	settings.setValue("injectJunkFiles", inject_junk_files_check->isChecked());
	settings.setValue("junkFileCount", junk_file_count_spin->value());
	settings.setValue("removeGuiTriggers", remove_gui_triggers_check->isChecked());
	settings.setValue("stripTriggerStrings", strip_trigger_strings_check->isChecked());
	settings.setValue("obfuscateAssetPaths", obfuscate_asset_paths_check->isChecked());
	settings.setValue("clearAuthor", clear_author_check->isChecked());
	settings.setValue("clearDescription", clear_description_check->isChecked());
	settings.setValue("clearLoadingText", clear_loading_text_check->isChecked());
	settings.setValue("normalizeName", normalize_name_check->isChecked());
	settings.endGroup();
}
