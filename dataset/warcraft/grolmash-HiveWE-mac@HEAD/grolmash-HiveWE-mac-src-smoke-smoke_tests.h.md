---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/smoke/smoke_tests.h"
ref: HEAD
---

#pragma once

#include <memory>

#include <QString>

#include "runtime_config.h"

class HiveWE;
class QTemporaryDir;
class QWidget;

struct SmokeSaveReloadResult {
	bool ok = false;
	std::unique_ptr<QTemporaryDir> temp_dir;
};

[[nodiscard]] SmokeSaveReloadResult run_smoke_save_reload(HiveWE& window);
[[nodiscard]] SmokeSaveReloadResult run_smoke_mutated_save_reload(HiveWE& window);
[[nodiscard]] SmokeSaveReloadResult run_smoke_mpq_export_reload(HiveWE& window);
[[nodiscard]] bool run_model_preview_smoke(QWidget* parent, const QString& artifact_directory);
[[nodiscard]] bool run_shared_model_preview_probe_smoke(QWidget* parent, const QString& artifact_directory);

#if HIVEWE_USE_METAL_RUNTIME
[[nodiscard]] bool run_metal_terrain_material_smoke(QWidget* parent, const QString& artifact_directory);
[[nodiscard]] bool run_metal_renderer_probe_smoke(QWidget* parent, const QString& artifact_directory);
[[nodiscard]] bool run_metal_preview_probe_smoke(QWidget* parent, const QString& artifact_directory);
#endif
