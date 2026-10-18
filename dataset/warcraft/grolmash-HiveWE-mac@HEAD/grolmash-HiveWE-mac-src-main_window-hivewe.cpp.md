---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main_window/hivewe.cpp"
ref: HEAD
---

#include "hivewe.h"
#define __STORMLIB_NO_STATIC_LINK__
#include "StormLib.h"

#include "pathing_palette.h"
#include "object_editor/object_editor.h"
#include "model_editor/model_editor.h"
#include "tile_setter.h"
#include "map_info_editor.h"
#include "terrain_palette.h"
#include "settings_editor.h"
#include "playtest_launcher.h"
#include "tile_pather.h"
#include "palette.h"
#include "doodad_palette.h"
#include "unit_palette.h"
#include "object_editor/icon_view.h"
#include "trigger_editor.h"
#include "QMessageBox"
#include "QProcess"
#include "QKeySequence"
#include "QString"
#include "menus/gameplay_constants_editor.h"
#include "asset_manager/asset_manager.h"
#include "runtime_config.h"
#include "std_module_compat.h"
import Hierarchy;
import MPQ;
import Camera;
import Globals;
import Map;
import MapGlobal;
import WorldUndoManager;
import ResourceManager;
import ImageDecoding;

namespace fs = std::filesystem;

HiveWE::HiveWE(QWidget* parent)
	: QMainWindow(parent) {
	setAutoFillBackground(true);

	// Buggy as of Qt 6.9.1. Likely requires 6.9.2 or later
	// setWindowFlag(Qt::ExpandedClientAreaHint, true);
	// setWindowFlag(Qt::NoTitleBarBackgroundHint, true);
	// setAttribute(Qt::WA_LayoutOnEntireRect, true);
	ui.setupUi(this);

	connect(ui.ribbon->undo, &QPushButton::clicked, [&]() {
		// ToDo: temporary, undoing should still allow a selection to persist
		if (map->brush) {
			map->brush->clear_selection();
		}

		auto context = WorldEditContext {
			.terrain = map->terrain,
			.units = map->units,
			.doodads = map->doodads,
			.brush = map->brush,
			.pathing_map = map->pathing_map,
		};

		map->world_undo.undo(context);
	});
	connect(ui.ribbon->redo, &QPushButton::clicked, [&]() {
		// ToDo: temporary, undoing should still allow a selection to persist
		if (map->brush) {
			map->brush->clear_selection();
		}

		auto context = WorldEditContext {
			.terrain = map->terrain,
			.units = map->units,
			.doodads = map->doodads,
			.brush = map->brush,
			.pathing_map = map->pathing_map,
		};

		map->world_undo.redo(context);
	});

	connect(new QShortcut(Qt::CTRL | Qt::Key_Z, this), &QShortcut::activated, ui.ribbon->undo, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_Y, this), &QShortcut::activated, ui.ribbon->redo, &QPushButton::click);

	connect(ui.ribbon->units_visible, &QPushButton::toggled, [](bool checked) { map->render_units = checked; });
	connect(ui.ribbon->doodads_visible, &QPushButton::toggled, [](bool checked) { map->render_doodads = checked; });
	connect(ui.ribbon->pathing_visible, &QPushButton::toggled, [](bool checked) { map->render_pathing = checked; });
	connect(ui.ribbon->brush_visible, &QPushButton::toggled, [](bool checked) { map->render_brush = checked; });
	connect(ui.ribbon->lighting_visible, &QPushButton::toggled, [](bool checked) { map->render_lighting = checked; });
	connect(ui.ribbon->water_visible, &QPushButton::toggled, [](bool checked) { map->render_water = checked; });
	connect(ui.ribbon->click_helpers_visible, &QPushButton::toggled, [](bool checked) { map->render_click_helpers = checked; });
	connect(ui.ribbon->wireframe_visible, &QPushButton::toggled, [](bool checked) { map->render_wireframe = checked; });
	connect(ui.ribbon->debug_visible, &QPushButton::toggled, [](bool checked) { map->render_debug = checked; });
	connect(ui.ribbon->minimap_visible, &QPushButton::toggled, [&](bool checked) { (checked) ? minimap->show() : minimap->hide(); });

	connect(new QShortcut(Qt::CTRL | Qt::Key_U, this), &QShortcut::activated, ui.ribbon->units_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_D, this), &QShortcut::activated, ui.ribbon->doodads_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_P, this), &QShortcut::activated, ui.ribbon->pathing_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_L, this), &QShortcut::activated, ui.ribbon->lighting_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_W, this), &QShortcut::activated, ui.ribbon->water_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_I, this), &QShortcut::activated, ui.ribbon->click_helpers_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_T, this), &QShortcut::activated, ui.ribbon->wireframe_visible, &QPushButton::click);
	connect(new QShortcut(Qt::Key_F3, this), &QShortcut::activated, ui.ribbon->debug_visible, &QPushButton::click);

	// Reload theme
	connect(new QShortcut(Qt::Key_F5, this), &QShortcut::activated, [&]() {
		QSettings settings;
		QFile file("data/themes/" + settings.value("theme").toString() + ".qss");
		const auto success = file.open(QFile::ReadOnly);
		if (!success) {
			std::println("Failed to open theme file");
			return;
		}
		QString StyleSheet = QLatin1String(file.readAll());

		qApp->setStyleSheet(StyleSheet);
	});

	connect(ui.ribbon->reset_camera, &QPushButton::clicked, [&]() { camera.reset(); });
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::SHIFT | Qt::Key_C), this), &QShortcut::activated, ui.ribbon->reset_camera, &QPushButton::click);

	connect(ui.ribbon->import_heightmap, &QPushButton::clicked, this, &HiveWE::import_heightmap);

	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_O), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui.ribbon->open_map_folder, &QPushButton::click);
	// connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_I), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui.ribbon->open_map_mpq, &QPushButton::click);
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_S), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui.ribbon->save_map, &QPushButton::click);
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::SHIFT | Qt::Key_S), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui.ribbon->save_map_as, &QPushButton::click);

	// connect(ui.ribbon->new_map, &QAction::triggered, this, &HiveWE::load);
	connect(ui.ribbon->open_map_folder, &QPushButton::clicked, this, &HiveWE::load_folder);
	connect(ui.ribbon->open_map_mpq, &QPushButton::clicked, this, &HiveWE::load_mpq);
	connect(ui.ribbon->save_map, &QPushButton::clicked, this, &HiveWE::save);
	connect(ui.ribbon->save_map_as, &QPushButton::clicked, this, &HiveWE::save_as);
	connect(ui.ribbon->export_mpq, &QPushButton::clicked, this, &HiveWE::export_mpq);
	connect(ui.ribbon->test_map, &QPushButton::clicked, this, &HiveWE::play_test);
	connect(ui.ribbon->settings, &QPushButton::clicked, [&]() { new SettingsEditor(this); });
	connect(ui.ribbon->switch_warcraft, &QPushButton::clicked, this, &HiveWE::switch_warcraft);
	connect(ui.ribbon->exit, &QPushButton::clicked, [&]() { QApplication::exit(); });

	connect(ui.ribbon->change_tileset, &QRibbonButton::clicked, [this]() { new TileSetter(this); });
	connect(ui.ribbon->change_tile_pathing, &QRibbonButton::clicked, [this]() { new TilePather(this); });

	connect(ui.ribbon->map_description, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(0); });
	connect(ui.ribbon->map_loading_screen, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(1); });
	connect(ui.ribbon->map_options, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(2); });
	// connect(ui, &QAction::triggered, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(3); });

	connect(new QShortcut(QKeySequence(Qt::Key_T), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, [&]() {
		open_palette<TerrainPalette>();
	});

	connect(ui.ribbon->terrain_palette, &QRibbonButton::clicked, [this]() {
		open_palette<TerrainPalette>();
	});

	connect(new QShortcut(QKeySequence(Qt::Key_D), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, [&]() {
		open_palette<DoodadPalette>();
	});
	connect(ui.ribbon->doodad_palette, &QRibbonButton::clicked, [this]() {
		open_palette<DoodadPalette>();
	});

	connect(new QShortcut(QKeySequence(Qt::Key_U), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, [&]() {
		open_palette<UnitPalette>();
	});

	connect(ui.ribbon->unit_palette, &QRibbonButton::clicked, [this]() {
		open_palette<UnitPalette>();
	});

	connect(new QShortcut(QKeySequence(Qt::Key_P), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, [&]() {
		ui.ribbon->pathing_visible->setChecked(true);
		open_palette<PathingPalette>();
	});

	connect(ui.ribbon->pathing_palette, &QRibbonButton::clicked, [this]() {
		ui.ribbon->pathing_visible->setChecked(true);
		open_palette<PathingPalette>();
	});

	connect(ui.ribbon->trigger_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		const auto editor = window_handler.create_or_raise<TriggerEditor>(nullptr, created);
		connect(this, &HiveWE::saving_initiated, editor, &TriggerEditor::save_changes, Qt::UniqueConnection);
	});

	connect(ui.ribbon->object_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ObjectEditor>(nullptr, created);
	});

	connect(ui.ribbon->model_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ModelEditor>(nullptr, created);
	});

	connect(ui.ribbon->gameplay_constants, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<GameplayConstantsEditor>(nullptr, created);
	});

	connect(ui.ribbon->asset_manager, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<AssetManager>(nullptr, created);
	});

	restore_window_state();
#if HIVEWE_USE_METAL_RUNTIME
	QTimer::singleShot(0, this, [this] {
		raise();
		activateWindow();
	});
#endif

	minimap->setParent(ui.widget);
	minimap->move(10, 10);
	minimap->show();
	minimap->raise();

	connect(minimap, &Minimap::clicked, [](QPointF location) { camera.position = { location.x() * map->terrain.width, (1.0 - location.y()) * map->terrain.height, camera.position.z }; });
#if !HIVEWE_USE_METAL_RUNTIME
	ui.widget->makeCurrent();
#endif

	map = new Map();
#if !HIVEWE_USE_METAL_RUNTIME
	map->render_manager->initialize_opengl();
#endif
	connect(&map->terrain, &Terrain::minimap_changed, minimap, &Minimap::set_minimap);
	connect(&map->terrain, &Terrain::tileset_changed, [&]() {
		const auto palette = window_handler.get_open<TerrainPalette>();
		if (palette) {
			palette.value()->refresh();
		}
	});

#if !HIVEWE_USE_METAL_RUNTIME
	map->render_manager->resize_framebuffers(ui.widget->width(), ui.widget->height());
#endif
}

void HiveWE::load_map(const fs::path& directory) {
#if HIVEWE_USE_METAL_RUNTIME
	window_handler.close_all();
	ui.widget->invalidate_map_resources();

	delete map;
	resource_manager.clear();
	map = new Map();

	connect(&map->terrain, &Terrain::minimap_changed, minimap, &Minimap::set_minimap);
	connect(&map->terrain, &Terrain::tileset_changed, [&]() {
		const auto palette = window_handler.get_open<TerrainPalette>();
		if (palette) {
			palette.value()->refresh();
		}
	});

	map->load(directory);
	setWindowTitle("HiveWE 0.11 - " + QString::fromStdString(map->filesystem_path.string()));
#else
	window_handler.close_all();
	ui.widget->makeCurrent();

	delete map;
	resource_manager.clear();
	map = new Map();
	map->render_manager->initialize_opengl();

	connect(&map->terrain, &Terrain::minimap_changed, minimap, &Minimap::set_minimap);
	connect(&map->terrain, &Terrain::tileset_changed, [&]() {
		const auto palette = window_handler.get_open<TerrainPalette>();
		if (palette) {
			palette.value()->refresh();
		}
	});

	map->load(directory);

	map->render_manager->resize_framebuffers(ui.widget->width(), ui.widget->height());
	setWindowTitle("HiveWE 0.11 - " + QString::fromStdString(map->filesystem_path.string()));
#endif
}

void HiveWE::load_folder() {
	QSettings settings;

	QString folder_name = QFileDialog::getExistingDirectory(this, "Open Map Directory",
															settings.value("openDirectory", QDir::current().path()).toString(),
															QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);

	if (folder_name == "") {
		return;
	}

	settings.setValue("openDirectory", folder_name);

	fs::path directory = folder_name.toStdString();

	if (!fs::exists(directory / "war3map.w3i")) {
		QMessageBox::information(this, "Opening map failed", "Opening the map failed. Select a map that is saved in folder mode or use the Open Map (MPQ) option");
		return;
	}

	QMessageBox* loading_box = new QMessageBox(QMessageBox::Icon::Information, "Loading Map", "Loading " + QString::fromStdString(directory.filename().string()));
	loading_box->show();

	load_map(directory);

	loading_box->close();
	delete loading_box;
}

/// Load MPQ will extract all files from the archive in a user specified location
void HiveWE::load_mpq() {
	QSettings settings;

	// Choose an MPQ
	QString file_name = QFileDialog::getOpenFileName(this, "Open File",
													 settings.value("openDirectory", QDir::current().path()).toString(),
													 "Warcraft III Scenario (*.w3m *.w3x)");

	if (file_name.isEmpty()) {
		return;
	}

	settings.setValue("openDirectory", file_name);

	fs::path mpq_path = file_name.toStdWString();

	mpq::MPQ mpq;
	bool opened = mpq.open(mpq_path);
	if (!opened) {
		const auto message = std::format("Opening the map archive failed. It might be opened in another program.\nError Code {}", GetLastError());
		QMessageBox::critical(this, "Opening map failed", QString::fromStdString(message));
		return;
	}

	fs::path unpack_location = QFileDialog::getExistingDirectory(
								   this, "Choose Unpacking Location",
								   settings.value("openDirectory", QDir::current().path()).toString(),
								   QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks)
								   .toStdString();

	if (unpack_location.empty()) {
		return;
	}

	fs::path final_directory = unpack_location / mpq_path.stem();

	try {
		fs::create_directory(final_directory);
	} catch (std::filesystem::filesystem_error& e) {
		QMessageBox::critical(this, "Error creating directory", "Failed to create the directory to unpack into with error:\n" + QString::fromStdString(e.what()), QMessageBox::StandardButton::Ok, QMessageBox::StandardButton::Ok);
		return;
	}

	bool unpacked = mpq.unpack(final_directory);
	if (!unpacked) {
		QMessageBox::critical(this, "Unpacking failed", "There was an error unpacking the archive.");
		std::println("{}", GetLastError());
		return;
	}

	load_map(final_directory);
}

void HiveWE::save() {
	emit saving_initiated();
	map->save(map->filesystem_path);
};

void HiveWE::save_as() {
	QSettings settings;
	const QString directory = settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->name);

	fs::path file_name = QFileDialog::getExistingDirectory(this, "Choose Save Location",
														   settings.value("openDirectory", QDir::current().path()).toString(),
														   QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks)
							 .toStdString();

	if (file_name.empty()) {
		return;
	}

	emit saving_initiated();
	if (fs::exists(file_name) && fs::equivalent(file_name, map->filesystem_path)) {
		map->save(map->filesystem_path);
	} else {
		fs::create_directories(file_name / map->name);

		hierarchy.map_directory = file_name / map->name;
		map->save(file_name / map->name);
	}

	setWindowTitle("HiveWE 0.11 - " + QString::fromStdString(map->filesystem_path.string()));
}

void HiveWE::export_mpq() {
	QSettings settings;
	const QString directory = settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->filesystem_path.filename().string());
	const QString selected_file = QFileDialog::getSaveFileName(this, "Export Map to MPQ", directory, "Warcraft III Scenario (*.w3x)");

	if (selected_file.isEmpty()) {
		return;
	}

#ifdef _WIN32
	const std::wstring file_name = selected_file.toStdWString();
#else
	const std::string file_name = selected_file.toStdString();
#endif
	const fs::path file_path(file_name);

	fs::remove(file_path);

	emit saving_initiated();
	map->save(map->filesystem_path);

	uint64_t file_count = std::distance(fs::recursive_directory_iterator{ map->filesystem_path }, {});

	HANDLE handle;
	bool open = SFileCreateArchive(file_name.c_str(), MPQ_CREATE_LISTFILE | MPQ_CREATE_ATTRIBUTES, file_count, &handle);
	if (!open) {
		QMessageBox::critical(this, "Exporting failed", "There was an error creating the archive.");
		std::println("{}", GetLastError());
		return;
	}

	for (const auto& entry : fs::recursive_directory_iterator(map->filesystem_path)) {
		if (entry.is_regular_file()) {
			bool success = SFileAddFileEx(handle, entry.path().c_str(), entry.path().lexically_relative(map->filesystem_path).string().c_str(), MPQ_FILE_COMPRESS, MPQ_COMPRESSION_ZLIB, MPQ_COMPRESSION_NEXT_SAME);
			if (!success) {
				std::println("Error {} adding file {}", GetLastError(), entry.path().string());
			}
		}
	}
	SFileCompactArchive(handle, nullptr, false);
	SFileCloseArchive(handle);
}

void HiveWE::play_test() {
	emit saving_initiated();
	if (!map->save(map->filesystem_path)) {
		return;
	}

	const QString warcraft_path = hivewe::play_test_executable_path(hierarchy.root_directory);
	if (warcraft_path.isEmpty()) {
		QMessageBox::critical(this, "Play Test failed", "Could not find a Warcraft III executable in the configured Warcraft directory.");
		return;
	}

	QSettings settings;
	if (!QProcess::startDetached(warcraft_path, hivewe::play_test_arguments(map->filesystem_path, settings.value("testArgs").toString()))) {
		QMessageBox::critical(this, "Play Test failed", "Could not start Warcraft III.");
	}
}

bool HiveWE::save_rendered_frame(const QString& file_path) {
	return ui.widget->save_rendered_frame(file_path);
}

bool HiveWE::smoke_render_modes(const QString& artifact_directory) {
	return ui.widget->smoke_render_modes(artifact_directory);
}

bool HiveWE::smoke_pick_rendered_objects() {
	return ui.widget->smoke_pick_rendered_objects();
}

void HiveWE::closeEvent(QCloseEvent* event) {
	int choice = QMessageBox::question(this, "Do you want to quit?", "Are you sure you want to quit?", QMessageBox::Yes | QMessageBox::No, QMessageBox::No);

	if (choice == QMessageBox::Yes) {
		QApplication::closeAllWindows();
		event->accept();
	} else {
		event->ignore();
	}
}

void HiveWE::resizeEvent(QResizeEvent* event) {
	QMainWindow::resizeEvent(event);
	QTimer::singleShot(0, [&] { save_window_state(); });
}

void HiveWE::moveEvent(QMoveEvent* event) {
	QMainWindow::moveEvent(event);
	QTimer::singleShot(0, [&] { save_window_state(); });
}

void HiveWE::switch_warcraft() {
	QSettings settings;
	fs::path directory;
	do {
		directory = QFileDialog::getExistingDirectory(this, "Select Warcraft Directory", "/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks).toStdWString();
		if (directory == "") {
			directory = settings.value("warcraftDirectory").toString().toStdString();
		}
	} while (!hierarchy.open_casc(directory));

	if (directory != hierarchy.warcraft_directory) {
		settings.setValue("warcraftDirectory", QString::fromStdString(directory.string()));
	}
}

// ToDo move to terrain class?
void HiveWE::import_heightmap() {
	QMessageBox::information(this, "Heightmap information", "Will read the red channel and map this onto the range -16 to +16");
	QSettings settings;
	const QString directory = settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->filesystem_path.filename().string());

	QString file_name = QFileDialog::getOpenFileName(this, "Open Heightmap Image", directory);

	if (file_name == "") {
		return;
	}

	const DecodedImage image = decode_image_file(file_name.toStdString());
	if (!image.valid()) {
		QMessageBox::warning(this, "Invalid Image", "Could not decode the selected heightmap image.");
		return;
	}

	if (image.width != map->terrain.width || image.height != map->terrain.height) {
		QMessageBox::warning(this, "Incorrect Image Size", QString("Image Size: %1x%2 does not match terrain size: %3x%4").arg(QString::number(image.width), QString::number(image.height), QString::number(map->terrain.width), QString::number(map->terrain.height)));
		return;
	}

	for (int j = 0; j < image.height; j++) {
		for (int i = 0; i < image.width; i++) {
			map->terrain.corner_height[map->terrain.ci(i, j)] =
				(image.pixels[static_cast<std::size_t>(((image.height - 1 - j) * image.width + i) * image.channels)] - 128.f) / 8.f;
		}
	}

	map->terrain.update_ground_heights({ 0, 0, image.width, image.height });
}

void HiveWE::save_window_state() {
	QSettings settings;

	if (!isMaximized()) {
		settings.setValue("MainWindow/geometry", saveGeometry());
	}

	settings.setValue("MainWindow/maximized", isMaximized());
	settings.setValue("MainWindow/windowState", saveState());
}

void HiveWE::restore_window_state() {
	QSettings settings;

	if (settings.contains("MainWindow/windowState")) {
		restoreGeometry(settings.value("MainWindow/geometry").toByteArray());
		restoreState(settings.value("MainWindow/windowState").toByteArray());
		if (settings.value("MainWindow/maximized").toBool()) {
			showMaximized();
		} else {
			showNormal();
		}
	} else {
		showMaximized();
	}
}

void HiveWE::set_current_custom_tab(QRibbonTab* tab, QString name) {
	if (current_custom_tab == tab) {
		return;
	}

	if (current_custom_tab != nullptr) {
		emit palette_changed(tab);
	}

	remove_custom_tab();
	current_custom_tab = tab;
	ui.ribbon->addTab(tab, name);
	ui.ribbon->setCurrentIndex(ui.ribbon->count() - 1);
}

void HiveWE::remove_custom_tab() {
	for (int i = 0; i < ui.ribbon->count(); i++) {
		if (ui.ribbon->widget(i) == current_custom_tab) {
			ui.ribbon->removeTab(i);
			current_custom_tab = nullptr;
			return;
		}
	}
}

#include "moc_hivewe.cpp"
