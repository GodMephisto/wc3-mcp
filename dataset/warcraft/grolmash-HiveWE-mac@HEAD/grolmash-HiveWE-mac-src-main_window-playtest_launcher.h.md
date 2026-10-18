---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main_window/playtest_launcher.h"
ref: HEAD
---

#pragma once

#include <filesystem>

#include <QString>
#include <QStringList>

namespace hivewe {

[[nodiscard]] QString canonical_existing_path(const std::filesystem::path& path);
[[nodiscard]] QString play_test_executable_path(const std::filesystem::path& warcraft_root);
[[nodiscard]] QStringList play_test_arguments(const std::filesystem::path& map_path, const QString& extra_arguments);

}
