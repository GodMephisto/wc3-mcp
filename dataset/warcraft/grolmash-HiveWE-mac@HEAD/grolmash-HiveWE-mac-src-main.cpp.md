---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main.cpp"
ref: HEAD
---

#define MI_MALLOC_OVERRIDE
#include <mimalloc.h>

#include "runtime_config.h"

#if HIVEWE_USE_METAL_RUNTIME
#define QT_NO_OPENGL
#endif

#include <QApplication>
#include <QCommandLineOption>
#include <QCommandLineParser>
#include <QCoreApplication>
#include <QDebug>
#include <QDir>
#include <QFile>
#include <QFileDialog>
#include <QFileInfo>
#include <QLocale>
#include <QPalette>
#include <QSurfaceFormat>
#include <QSettings>
#include <QTemporaryDir>
#include <QTimer>

#include "main_window/hivewe.h"
#include "DockManager.h"

#include <cstdlib>
#include <exception>
#include <filesystem>
#include <future>
#include <memory>
#include <print>
#include <string>

#ifdef WIN32
// To force HiveWE to run on the discrete GPU if available
extern "C" {
__declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;
__declspec(dllexport) unsigned long NvOptimusEnablement = 1;
}
#endif

#include <tracy/Tracy.hpp>

#include "std_module_compat.h"
import Map;
import Timer;
import MapGlobal;
import Globals;
import Utilities;
import Hierarchy;
import TaskPool;
namespace fs = std::filesystem;

#include "smoke/smoke_tests.h"

namespace {
QString absolute_cli_path(const QString& path, const QString& original_current_path) {
	if (path.isEmpty()) {
		return path;
	}

	const QFileInfo file_info(path);
	if (file_info.isAbsolute()) {
		return QDir::cleanPath(path);
	}

	return QDir::cleanPath(QDir(original_current_path).absoluteFilePath(path));
}

QString startup_map_cli_help() {
	return QStringLiteral(
		"HiveWE CLI opens unpacked Warcraft III map folders.\n\n"
		"Usage:\n"
		"  hivewe /path/to/unpacked-map-folder\n"
		"  hivewe --warcraft-dir \"/Applications/Warcraft III\" /path/to/unpacked-map-folder\n\n"
		"The map folder must contain at least these files:\n"
		"  war3map.w3i\n"
		"  war3map.w3e\n"
		"  war3map.doo\n\n"
		"If you have a .w3x or .w3m archive, unpack it first or open it from the GUI with File > Open Map (MPQ)."
	);
}

QString startup_map_validation_error(const QString& path) {
	const QFileInfo map_info(path);
	if (!map_info.exists()) {
		return QStringLiteral("Map path does not exist:\n  %1\n\n%2").arg(QDir::toNativeSeparators(path), startup_map_cli_help());
	}

	if (!map_info.isDir()) {
		const QString suffix = map_info.suffix().toLower();
		if (suffix == "w3x" || suffix == "w3m") {
			return QStringLiteral(
				"HiveWE CLI currently opens unpacked map folders, not .%1 archives:\n  %2\n\n%3"
			).arg(suffix, QDir::toNativeSeparators(path), startup_map_cli_help());
		}

		return QStringLiteral("Map path is not a folder:\n  %1\n\n%2").arg(QDir::toNativeSeparators(path), startup_map_cli_help());
	}

	const QDir map_directory(path);
	QStringList missing_files;
	for (const QString& required_file : {QStringLiteral("war3map.w3i"), QStringLiteral("war3map.w3e"), QStringLiteral("war3map.doo")}) {
		if (!map_directory.exists(required_file)) {
			missing_files.append(required_file);
		}
	}

	if (!missing_files.isEmpty()) {
		return QStringLiteral(
			"Map folder is missing required Warcraft III map files:\n  %1\n\nChecked folder:\n  %2\n\n%3"
		).arg(missing_files.join(QStringLiteral("\n  ")), QDir::toNativeSeparators(map_directory.absolutePath()), startup_map_cli_help());
	}

	return {};
}

void use_bundle_resources_as_current_directory() {
#if defined(__APPLE__)
	const QDir bundle_resources(QCoreApplication::applicationDirPath() + "/../Resources");
	if (bundle_resources.exists("data")) {
		QDir::setCurrent(bundle_resources.absolutePath());
	}
#endif
}

const char* renderer_backend_name() {
#if HIVEWE_USE_METAL_RUNTIME
	return "metal";
#else
	return "opengl";
#endif
}
} // namespace

int main(int argc, char* argv[]) {
	ZoneScopedN("main");

	Timer start_timer;

#if !HIVEWE_USE_METAL_RUNTIME
	QSurfaceFormat format;
	format.setDepthBufferSize(24);
	format.setStencilBufferSize(8);
	format.setVersion(4, 5);
	format.setProfile(QSurfaceFormat::CoreProfile);
	format.setOption(QSurfaceFormat::DebugContext);
	format.setSwapInterval(1);
	//format.setColorSpace(QSurfaceFormat::sRGBColorSpace);
	QSurfaceFormat::setDefaultFormat(format);

	QCoreApplication::setAttribute(Qt::AA_ShareOpenGLContexts);
#endif
	QCoreApplication::setOrganizationName("HiveWE");
	QCoreApplication::setApplicationName("HiveWE");

	QLocale::setDefault(QLocale("en_US"));

	// Create a dark palette
	// For some magically unknown reason Qt draws Qt::white text as black, so we use QColor(255, 254, 255) instead
	QPalette darkPalette;
	darkPalette.setColor(QPalette::Window, QColor(53, 53, 53));
	darkPalette.setColor(QPalette::WindowText, QColor(255, 254, 255));
	darkPalette.setColor(QPalette::Disabled, QPalette::WindowText, QColor(127, 127, 127));
	darkPalette.setColor(QPalette::Base, QColor(42, 42, 42));
	darkPalette.setColor(QPalette::AlternateBase, QColor(66, 66, 66));
	darkPalette.setColor(QPalette::ToolTipBase, QColor(66, 66, 66));
	darkPalette.setColor(QPalette::ToolTipText, QColor(255, 254, 255));
	darkPalette.setColor(QPalette::Text, QColor(255, 254, 255));
	darkPalette.setColor(QPalette::PlaceholderText, Qt::gray);
	darkPalette.setColor(QPalette::Disabled, QPalette::Text, QColor(127, 127, 127));
	darkPalette.setColor(QPalette::Dark, QColor(35, 35, 35));
	darkPalette.setColor(QPalette::Shadow, QColor(20, 20, 20));
	darkPalette.setColor(QPalette::Button, QColor(53, 53, 53));
	darkPalette.setColor(QPalette::ButtonText, QColor(255, 254, 255));
	darkPalette.setColor(QPalette::Disabled, QPalette::ButtonText, QColor(127, 127, 127));
	darkPalette.setColor(QPalette::BrightText, Qt::red);
	darkPalette.setColor(QPalette::Link, QColor(42, 130, 218));
	darkPalette.setColor(QPalette::Highlight, QColor(42, 130, 218));
	darkPalette.setColor(QPalette::Disabled, QPalette::Highlight, QColor(80, 80, 80));
	darkPalette.setColor(QPalette::HighlightedText, QColor(255, 254, 255));
	darkPalette.setColor(QPalette::Disabled, QPalette::HighlightedText, QColor(127, 127, 127));

	QApplication::setPalette(darkPalette);
	QApplication::setStyle("Fusion");

	QApplication a(argc, argv);
	const QString original_current_path = QDir::currentPath();
	QCommandLineParser parser;
	parser.setApplicationDescription("HiveWE");
	parser.addHelpOption();
	QCommandLineOption print_renderer_backend_option("print-render-backend", "Print the compiled renderer backend and exit.");
	QCommandLineOption map_path_option("map", "Load an unpacked Warcraft III map folder at startup.", "path", "data/test map/");
	QCommandLineOption warcraft_dir_option("warcraft-dir", "Use this Warcraft III installation directory instead of the saved/default path.", "path");
	QCommandLineOption smoke_exit_option("smoke-exit-ms", "Exit after the given number of milliseconds for startup smoke tests.", "milliseconds");
	QCommandLineOption smoke_save_reload_option("smoke-save-reload", "Save the startup map to a temporary folder and reload it for smoke validation.");
	QCommandLineOption smoke_mutated_save_reload_option("smoke-mutated-save-reload", "Mutate serialized startup map data, save to a temporary folder, and reload it for smoke validation.");
	QCommandLineOption smoke_mpq_export_reload_option("smoke-mpq-export-reload", "Export the startup map to a temporary MPQ archive, unpack it, and reload it for smoke validation.");
	QCommandLineOption capture_frame_option("capture-frame", "Save one rendered map canvas frame to a PNG path and exit.", "path");
	QCommandLineOption smoke_render_modes_option("smoke-render-modes", "Render the startup map with overlay/toggle modes enabled, then verify the Metal material/cliff renderer when available.");
	QCommandLineOption smoke_picking_option(
		"smoke-picking",
		"Render the startup map and verify unit, doodad, and helper-only doodad picking can identify projected visible objects."
	);
	QCommandLineOption smoke_model_previews_option("smoke-model-previews", "Render known model previews and, in Metal builds, synthetic model and particle frames, then verify readback pixels.");
	QCommandLineOption smoke_synthetic_model_previews_option(
		"smoke-synthetic-model-previews",
		"Render Warcraft-free synthetic model editor and object-grid previews after startup map load."
	);
	QCommandLineOption smoke_shared_preview_probes_option(
		"smoke-shared-preview-probes",
		"Run Warcraft-free synthetic model editor and object-grid preview probes shared by OpenGL and Metal, then exit."
	);
#if HIVEWE_USE_METAL_RUNTIME
	QCommandLineOption smoke_metal_renderer_probes_option(
		"smoke-metal-renderer-probes",
		"Run Warcraft-free synthetic Metal renderer probes and exit."
	);
	QCommandLineOption smoke_metal_preview_probes_option(
		"smoke-metal-preview-probes",
		"Run Warcraft-free synthetic Metal model editor and object-grid preview probes and exit."
	);
#endif
	QCommandLineOption smoke_artifact_dir_option("smoke-artifact-dir", "Write PNG artifacts from smoke render readbacks to a directory.", "path");
	QCommandLineOption capture_delay_option("capture-delay-ms", "Delay --capture-frame by the given number of milliseconds after startup.", "milliseconds", "1000");
	parser.addOption(print_renderer_backend_option);
	parser.addOption(map_path_option);
	parser.addOption(warcraft_dir_option);
	parser.addOption(smoke_exit_option);
	parser.addOption(smoke_save_reload_option);
	parser.addOption(smoke_mutated_save_reload_option);
	parser.addOption(smoke_mpq_export_reload_option);
	parser.addOption(capture_frame_option);
	parser.addOption(smoke_render_modes_option);
	parser.addOption(smoke_picking_option);
	parser.addOption(smoke_model_previews_option);
	parser.addOption(smoke_synthetic_model_previews_option);
	parser.addOption(smoke_shared_preview_probes_option);
#if HIVEWE_USE_METAL_RUNTIME
	parser.addOption(smoke_metal_renderer_probes_option);
	parser.addOption(smoke_metal_preview_probes_option);
#endif
	parser.addOption(smoke_artifact_dir_option);
	parser.addOption(capture_delay_option);
	parser.addPositionalArgument("map", "Unpacked Warcraft III map folder to load at startup.");
	parser.process(a);
	if (parser.isSet(print_renderer_backend_option)) {
		std::println("{}", renderer_backend_name());
		return EXIT_SUCCESS;
	}
	const QStringList positional_arguments = parser.positionalArguments();
	if (positional_arguments.size() > 1) {
		qCritical() << "Expected at most one positional map path.";
		return EXIT_FAILURE;
	}
	if (!positional_arguments.isEmpty() && parser.isSet(map_path_option)) {
		qCritical() << "Pass the startup map either as a positional path or with --map, not both.";
		return EXIT_FAILURE;
	}
	const bool capture_requested = parser.isSet(capture_frame_option);
	const bool smoke_render_modes_requested = parser.isSet(smoke_render_modes_option);
	const bool smoke_picking_requested = parser.isSet(smoke_picking_option);
	const bool smoke_model_previews_requested = parser.isSet(smoke_model_previews_option);
	const bool smoke_synthetic_model_previews_requested = parser.isSet(smoke_synthetic_model_previews_option);
	const bool smoke_shared_preview_probes_requested = parser.isSet(smoke_shared_preview_probes_option);
#if HIVEWE_USE_METAL_RUNTIME
	const bool smoke_metal_renderer_probes_requested = parser.isSet(smoke_metal_renderer_probes_option);
	const bool smoke_metal_preview_probes_requested = parser.isSet(smoke_metal_preview_probes_option);
#endif
	const bool render_smoke_requested = capture_requested || smoke_render_modes_requested || smoke_picking_requested || smoke_model_previews_requested
		|| smoke_synthetic_model_previews_requested;
	const bool smoke_mode = parser.isSet(smoke_exit_option) || parser.isSet(smoke_save_reload_option)
		|| parser.isSet(smoke_mutated_save_reload_option) || parser.isSet(smoke_mpq_export_reload_option)
		|| render_smoke_requested || smoke_shared_preview_probes_requested;
	const QString capture_frame_path = capture_requested
		? absolute_cli_path(parser.value(capture_frame_option), original_current_path)
		: QString();
	const QString smoke_artifact_directory = parser.isSet(smoke_artifact_dir_option)
		? absolute_cli_path(parser.value(smoke_artifact_dir_option), original_current_path)
		: QString();
	const QString startup_map_path = [&] {
		if (parser.isSet(map_path_option)) {
			return absolute_cli_path(parser.value(map_path_option), original_current_path);
		}
		if (!positional_arguments.isEmpty()) {
			return absolute_cli_path(positional_arguments.front(), original_current_path);
		}
		return parser.value(map_path_option);
	}();
	const QString explicit_warcraft_directory = parser.isSet(warcraft_dir_option)
		? absolute_cli_path(parser.value(warcraft_dir_option), original_current_path)
		: QString();
	bool capture_delay_ok = false;
	const int capture_delay_ms = parser.value(capture_delay_option).toInt(&capture_delay_ok);
	if (parser.isSet(warcraft_dir_option) && explicit_warcraft_directory.isEmpty()) {
		qCritical() << "--warcraft-dir requires a Warcraft III installation directory.";
		return EXIT_FAILURE;
	}
	if (capture_requested && capture_frame_path.isEmpty()) {
		qCritical() << "--capture-frame requires a PNG output path.";
		return EXIT_FAILURE;
	}
	if (parser.isSet(smoke_artifact_dir_option) && smoke_artifact_directory.isEmpty()) {
		qCritical() << "--smoke-artifact-dir requires an output directory.";
		return EXIT_FAILURE;
	}
	if (render_smoke_requested && (!capture_delay_ok || capture_delay_ms < 0)) {
		qCritical() << "Invalid --capture-delay-ms value:" << parser.value(capture_delay_option);
		return EXIT_FAILURE;
	}
	use_bundle_resources_as_current_directory();
	if (const QString validation_error = startup_map_validation_error(startup_map_path); !validation_error.isEmpty()) {
		qCritical().noquote() << validation_error;
		return EXIT_FAILURE;
	}

	ads::CDockManager::setConfigFlag(ads::CDockManager::FocusHighlighting);
	ads::CDockManager::setConfigFlag(ads::CDockManager::AllTabsHaveCloseButton);
	ads::CDockManager::setConfigFlag(ads::CDockManager::DockAreaDynamicTabsMenuButtonVisibility);
	ads::CDockManager::setConfigFlag(ads::CDockManager::OpaqueSplitterResize);
	ads::CDockManager::setConfigFlag(ads::CDockManager::MiddleMouseButtonClosesTab);

	QSettings settings;
	QFile file("data/themes/" + settings.value("theme", "Dark").toString() + ".qss");
	if (!file.open(QIODevice::ReadOnly)) {
		qWarning() << "Error: Reading theme failed:" << file.error() << ": " << file.errorString();
		return -1;
	}

	a.setStyleSheet(QLatin1String(file.readAll()));

	if (smoke_shared_preview_probes_requested
#if HIVEWE_USE_METAL_RUNTIME
		|| smoke_metal_renderer_probes_requested || smoke_metal_preview_probes_requested
#endif
	) {
		if (!smoke_artifact_directory.isEmpty() && !QDir().mkpath(smoke_artifact_directory)) {
			qCritical() << "Unable to create smoke artifact directory:" << smoke_artifact_directory;
			return EXIT_FAILURE;
		}

		bool probes_ok = true;
		if (smoke_shared_preview_probes_requested) {
			const bool preview_probes_rendered = run_shared_model_preview_probe_smoke(nullptr, smoke_artifact_directory);
			probes_ok = probes_ok && preview_probes_rendered;
			if (preview_probes_rendered) {
				qInfo() << "Smoke shared preview probes succeeded.";
			} else {
				qCritical() << "Smoke shared preview probes failed.";
			}
		}
#if HIVEWE_USE_METAL_RUNTIME
		if (smoke_metal_renderer_probes_requested) {
			const bool renderer_probes_rendered = run_metal_renderer_probe_smoke(nullptr, smoke_artifact_directory);
			probes_ok = probes_ok && renderer_probes_rendered;
			if (renderer_probes_rendered) {
				qInfo() << "Smoke Metal renderer probes succeeded.";
			} else {
				qCritical() << "Smoke Metal renderer probes failed.";
			}
		}
		if (smoke_metal_preview_probes_requested) {
			const bool preview_probes_rendered = run_metal_preview_probe_smoke(nullptr, smoke_artifact_directory);
			probes_ok = probes_ok && preview_probes_rendered;
			if (preview_probes_rendered) {
				qInfo() << "Smoke Metal preview probes succeeded.";
			} else {
				qCritical() << "Smoke Metal preview probes failed.";
			}
		}
#endif
		return probes_ok ? EXIT_SUCCESS : EXIT_FAILURE;
	}

	const auto load_files = [] -> bool {
		const auto ensure_hierarchy_file = [](const fs::path& path) {
			auto file = hierarchy.open_file(path);
			if (!file) {
				qCritical() << "Unable to load" << QString::fromStdString(path.string()) << ":"
							<< QString::fromStdString(file.error());
				return false;
			}
			return true;
		};

		if (!ensure_hierarchy_file("scripts/common.j") || !ensure_hierarchy_file("scripts/blizzard.j")
			|| !ensure_hierarchy_file("UI/WorldEditStrings.txt") || !ensure_hierarchy_file("UI/WorldEditGameStrings.txt")
			|| !ensure_hierarchy_file("UI/WorldEditData.txt")) {
			return false;
		}

		world_edit_strings.load("UI/WorldEditStrings.txt");
		world_edit_game_strings.load("UI/WorldEditGameStrings.txt");
		world_edit_data.load("UI/WorldEditData.txt");

		world_edit_data.substitute(world_edit_game_strings, "WorldEditStrings");
		world_edit_data.substitute(world_edit_strings, "WorldEditStrings");
		return true;
	};
	const auto configured_warcraft_directory = [&explicit_warcraft_directory] {
		return explicit_warcraft_directory.isEmpty()
			? find_warcraft_directory()
			: fs::path(explicit_warcraft_directory.toStdString());
	};
	const auto report_warcraft_open_failure = [](const fs::path& directory) {
		qCritical() << "Unable to open Warcraft CASC storage at"
					<< QString::fromStdString(directory.empty() ? std::string("<empty>") : directory.string());
	};

#if !HIVEWE_USE_METAL_RUNTIME
	bool is_casc_open = false;
	const auto casc_future = std::async(std::launch::async, [&]() {
		const fs::path directory = configured_warcraft_directory();

		is_casc_open = hierarchy.open_casc(directory);
		if (is_casc_open) {
			is_casc_open = load_files();
		}
	});

	cpu_task_pool.init(8);

	casc_future.wait();

	if (!is_casc_open) {
		fs::path directory = configured_warcraft_directory();

		while (!hierarchy.open_casc(directory)) {
			if (!explicit_warcraft_directory.isEmpty() || smoke_mode) {
				report_warcraft_open_failure(directory);
				return EXIT_FAILURE;
			}
			directory = QFileDialog::getExistingDirectory(nullptr, "Select Warcraft Directory", "/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks).toStdWString();
			if (directory == "") {
				exit(EXIT_SUCCESS);
			}
		}
		if (explicit_warcraft_directory.isEmpty()) {
			settings.setValue("warcraftDirectory", QString::fromStdString(directory.string()));
		}

		if (!load_files()) {
			return EXIT_FAILURE;
		}
	}
#else
	const bool can_use_bundled_smoke_data = smoke_mode && explicit_warcraft_directory.isEmpty() && !smoke_model_previews_requested;
	if (can_use_bundled_smoke_data) {
		const fs::path smoke_data_directory = fs::path("data/warcraft");
		if (!hierarchy.open_local_data(smoke_data_directory)) {
			qCritical() << "Unable to open bundled smoke data at"
						<< QString::fromStdString(smoke_data_directory.string());
			return EXIT_FAILURE;
		}
		qInfo() << "Running Metal smoke with bundled local Warcraft data.";
	} else {
		fs::path directory = configured_warcraft_directory();
		while (!hierarchy.open_casc(directory)) {
			if (!explicit_warcraft_directory.isEmpty() || smoke_mode) {
				report_warcraft_open_failure(directory);
				return EXIT_FAILURE;
			}
			directory = QFileDialog::getExistingDirectory(nullptr, "Select Warcraft Directory", "/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks).toStdWString();
			if (directory == "") {
				exit(EXIT_SUCCESS);
			}
		}
		if (explicit_warcraft_directory.isEmpty()) {
			settings.setValue("warcraftDirectory", QString::fromStdString(directory.string()));
		}
	}
	if (!load_files()) {
		return EXIT_FAILURE;
	}
#endif

	std::unique_ptr<QTemporaryDir> smoke_save_reload_temp_dir;
	std::unique_ptr<QTemporaryDir> smoke_mutated_save_reload_temp_dir;
	std::unique_ptr<QTemporaryDir> smoke_mpq_export_reload_temp_dir;
	HiveWE w;

	std::println("Application start: {}ms", start_timer.elapsed_ms());

	try {
		map->load(startup_map_path.isEmpty() ? "data/test map/" : startup_map_path.toStdString());
	} catch (const std::exception& e) {
		qCritical().noquote() << QStringLiteral(
			"Failed to load startup map:\n  %1\n\nError:\n  %2\n\n%3"
		).arg(
			QDir::toNativeSeparators(startup_map_path.isEmpty() ? QStringLiteral("data/test map/") : startup_map_path),
			QString::fromLocal8Bit(e.what()),
			startup_map_cli_help()
		);
		return EXIT_FAILURE;
	}
	// map->load("C:/Users/User/Desktop/MCFC.w3x");

	if (parser.isSet(smoke_save_reload_option)) {
		SmokeSaveReloadResult save_reload_result = run_smoke_save_reload(w);
		if (!save_reload_result.ok) {
			return EXIT_FAILURE;
		}
		smoke_save_reload_temp_dir = std::move(save_reload_result.temp_dir);
		if (!render_smoke_requested && !parser.isSet(smoke_mutated_save_reload_option) && !parser.isSet(smoke_mpq_export_reload_option)) {
			return EXIT_SUCCESS;
		}
	}
	if (parser.isSet(smoke_mutated_save_reload_option)) {
		SmokeSaveReloadResult mutated_save_reload_result = run_smoke_mutated_save_reload(w);
		if (!mutated_save_reload_result.ok) {
			return EXIT_FAILURE;
		}
		smoke_mutated_save_reload_temp_dir = std::move(mutated_save_reload_result.temp_dir);
		if (!render_smoke_requested && !parser.isSet(smoke_mpq_export_reload_option)) {
			return EXIT_SUCCESS;
		}
	}
	if (parser.isSet(smoke_mpq_export_reload_option)) {
		SmokeSaveReloadResult mpq_export_reload_result = run_smoke_mpq_export_reload(w);
		if (!mpq_export_reload_result.ok) {
			return EXIT_FAILURE;
		}
		smoke_mpq_export_reload_temp_dir = std::move(mpq_export_reload_result.temp_dir);
		if (!render_smoke_requested) {
			return EXIT_SUCCESS;
		}
	}

	const auto render_smoke_completed = std::make_shared<bool>(false);
	if (render_smoke_requested) {
		if (!map || !map->loaded) {
			qCritical() << "Render smoke requested before the startup map was loaded.";
			return EXIT_FAILURE;
		}

		if (capture_requested) {
			const QFileInfo capture_file(capture_frame_path);
			const QString capture_directory = capture_file.absolutePath();
			if (!capture_directory.isEmpty() && !QDir().mkpath(capture_directory)) {
				qCritical() << "Unable to create capture output directory:" << capture_directory;
				return EXIT_FAILURE;
			}
		}
		if (!smoke_artifact_directory.isEmpty() && !QDir().mkpath(smoke_artifact_directory)) {
			qCritical() << "Unable to create smoke artifact directory:" << smoke_artifact_directory;
			return EXIT_FAILURE;
		}

		QTimer::singleShot(
			capture_delay_ms,
			&w,
			[&w,
			 capture_requested,
			 smoke_render_modes_requested,
			 smoke_picking_requested,
			 smoke_model_previews_requested,
			 smoke_synthetic_model_previews_requested,
			 capture_frame_path,
			 smoke_artifact_directory,
			 render_smoke_completed] {
				bool ok = true;
				if (capture_requested) {
					const bool saved = w.save_rendered_frame(capture_frame_path);
					ok = ok && saved;
					if (saved) {
						qInfo() << "Captured rendered frame:" << capture_frame_path;
					} else {
						qCritical() << "Failed to capture rendered frame:" << capture_frame_path;
					}
				}

				if (smoke_render_modes_requested) {
					const bool rendered_modes = w.smoke_render_modes(smoke_artifact_directory);
					ok = ok && rendered_modes;
					if (rendered_modes) {
						qInfo() << "Smoke render modes succeeded.";
					} else {
						qCritical() << "Smoke render modes failed.";
					}

#if HIVEWE_USE_METAL_RUNTIME
					const bool terrain_materials_rendered = run_metal_terrain_material_smoke(&w, smoke_artifact_directory);
					ok = ok && terrain_materials_rendered;
					if (terrain_materials_rendered) {
						qInfo() << "Smoke Metal terrain material/cliff renderer succeeded.";
					} else {
						qCritical() << "Smoke Metal terrain material/cliff renderer failed.";
					}
#endif
				}

				if (smoke_picking_requested) {
					const bool picked = w.smoke_pick_rendered_objects();
					ok = ok && picked;
					if (picked) {
						qInfo() << "Smoke picking succeeded.";
					} else {
						qCritical() << "Smoke picking failed.";
					}
				}

				if (smoke_model_previews_requested) {
					const bool previews_rendered = run_model_preview_smoke(&w, smoke_artifact_directory);
					ok = ok && previews_rendered;
					if (previews_rendered) {
						qInfo() << "Smoke model previews succeeded.";
					} else {
						qCritical() << "Smoke model previews failed.";
					}
				}

#if HIVEWE_USE_METAL_RUNTIME
				if (smoke_render_modes_requested && smoke_picking_requested && smoke_model_previews_requested) {
					const bool renderer_probes_rendered = run_metal_renderer_probe_smoke(&w, smoke_artifact_directory);
					ok = ok && renderer_probes_rendered;
					if (renderer_probes_rendered) {
						qInfo() << "Smoke Metal renderer probes succeeded.";
					} else {
						qCritical() << "Smoke Metal renderer probes failed.";
					}
				}
#endif

				if (smoke_synthetic_model_previews_requested) {
					const bool previews_rendered = run_shared_model_preview_probe_smoke(&w, smoke_artifact_directory);
					ok = ok && previews_rendered;
					if (previews_rendered) {
						qInfo() << "Smoke synthetic model previews succeeded.";
					} else {
						qCritical() << "Smoke synthetic model previews failed.";
					}
				}

				*render_smoke_completed = true;
				QCoreApplication::exit(ok ? EXIT_SUCCESS : EXIT_FAILURE);
			}
		);
	}

	if (parser.isSet(smoke_exit_option)) {
		bool ok = false;
		const int milliseconds = parser.value(smoke_exit_option).toInt(&ok);
		if (ok && milliseconds >= 0) {
			QTimer::singleShot(milliseconds, &a, [render_smoke_requested, render_smoke_completed] {
				if (render_smoke_requested && !*render_smoke_completed) {
					qCritical() << "Render smoke did not complete before --smoke-exit-ms.";
					QCoreApplication::exit(EXIT_FAILURE);
					return;
				}
				QCoreApplication::exit(EXIT_SUCCESS);
			});
		} else {
			qWarning() << "Ignoring invalid --smoke-exit-ms value:" << parser.value(smoke_exit_option);
		}
	}

	return QApplication::exec();
}
