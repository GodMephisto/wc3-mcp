---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/map_protector/map_protector.h"
ref: HEAD
---

#pragma once

#include <filesystem>
#include <memory>

namespace fs = std::filesystem;

#include <QMainWindow>
#include <QCheckBox>
#include <QRadioButton>
#include <QLineEdit>
#include <QLabel>
#include <QProgressBar>
#include <QPushButton>
#include <QSpinBox>
#include <QThread>
#include <QTemporaryDir>
#include <QCloseEvent>
#include <QShowEvent>

import ProtectionPipeline;

class MapProtector : public QMainWindow {
	Q_OBJECT

  public:
	explicit MapProtector(QWidget* parent = nullptr);

  protected:
	void closeEvent(QCloseEvent* event) override;
	void showEvent(QShowEvent* event) override;

  private:
	void on_browse_clicked();
	void on_source_browse_file_clicked();
	void on_source_browse_folder_clicked();
	void on_export_clicked();
	void on_export_finished(PackResult result);
	void set_options_enabled(bool enabled);
	void update_source_controls_enabled();
	void load_settings();
	void save_settings() const;
	void populate_default_output_path();
	QString default_output_base_name() const;
	ProtectionOptions collect_options() const;

	QRadioButton* source_current_map_radio;
	QRadioButton* source_external_radio;
	QLineEdit* source_path_edit;
	QPushButton* source_browse_file_button;
	QPushButton* source_browse_folder_button;

	QLineEdit* output_path_edit;
	QPushButton* browse_button;

	QCheckBox* remove_listfile_check;
	QCheckBox* remove_attributes_check;
	QCheckBox* encrypt_files_check;
	QCheckBox* inject_junk_files_check;
	QSpinBox* junk_file_count_spin;
	QCheckBox* remove_gui_triggers_check;
	QCheckBox* strip_trigger_strings_check;
	QCheckBox* obfuscate_asset_paths_check;
	QCheckBox* clear_author_check;
	QCheckBox* clear_description_check;
	QCheckBox* clear_loading_text_check;
	QCheckBox* normalize_name_check;

	QLabel* status_label;
	QProgressBar* progress_bar;
	QPushButton* export_button;

	std::unique_ptr<QTemporaryDir> temp_dir;
	QThread* pack_thread = nullptr;
	bool export_in_progress = false;

	// Tracks the last value populate_default_output_path() itself wrote, so it can tell "field is
	// empty/still holds our own default" apart from "user typed a custom path" without a separate
	// dirty flag - re-populating overwrites the former but never the latter.
	QString auto_generated_output_path;
};
