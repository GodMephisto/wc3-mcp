---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/new_map_dialog.h"
ref: HEAD
---

#pragma once

#include <QString>
#include <QDialog>

class QComboBox;
class QGridLayout;
class QLabel;
class QLineEdit;

class NewMapDialog : public QDialog {
  public:
	struct Result {
		QString map_name;
		char tileset = 'L';
		int width = 64;
		int height = 64;
	};

	explicit NewMapDialog(QWidget* parent = nullptr);

	[[nodiscard]]
	Result result() const;

 private:
	void refresh_summary() const;
	void refresh_tileset_preview() const;

	static QString size_descriptor(int width, int height);

	QLineEdit* map_name = nullptr;
	QComboBox* tileset = nullptr;
	QComboBox* width = nullptr;
	QComboBox* height = nullptr;
	QLabel* size_description = nullptr;
	QLabel* playable_summary = nullptr;
	QGridLayout* tileset_preview = nullptr;
};
