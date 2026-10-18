---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/item_drop_editor.h"
ref: HEAD
---

#pragma once

#include <QIcon>
#include <QString>

#include <string>
#include <utility>

class QWidget;

QString describe_item_drop_entry(const std::pair<int, std::string>& entry);
QIcon icon_for_item_drop_entry(const std::pair<int, std::string>& entry);
bool edit_item_drop_entry(std::pair<int, std::string>& entry, QWidget* parent = nullptr);
