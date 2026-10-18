---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/model_editor/model_editor_metal_widget.cpp"
ref: HEAD
---

#include "model_editor_metal_widget.h"

#if HIVEWE_USE_METAL_RUNTIME

#include <algorithm>
#include <cmath>
#include <exception>
#include <fstream>
#include <optional>
#include <print>
#include <utility>

#include <QByteArray>
#include <QComboBox>
#include <QDesktopServices>
#include <QFileDialog>
#include <QFrame>
#include <QHBoxLayout>
#include <QKeyEvent>
#include <QLabel>
#include <QMessageBox>
#include <QMouseEvent>
#include <QPaintEvent>
#include <QPushButton>
#include <QResizeEvent>
#include <QSignalBlocker>
#include <QShowEvent>
#include <QStandardPaths>
#include <QUrl>
#include <QVBoxLayout>
#include <QWheelEvent>

#include <glm/gtc/quaternion.hpp>

#include "rendering/metal_map_frame_renderer.h"
#include "rendering/metal_map_frame_builders.h"
#include "rendering/metal_particle_emitter1_model_frame.h"
#include "std_module_compat.h"

namespace {
constexpr float k_min_preview_radius = 64.0f;
constexpr float k_preview_radius_padding = 1.1f;

QString model_file_stem(const mdx::MDX& model) {
	QString name = QString::fromStdString(model.name);
	if (name.isEmpty()) {
		name = QStringLiteral("model");
	}
	name.replace('/', '_');
	name.replace('\\', '_');
	name.replace(':', '_');
	return name;
}

bool finite_vec3(const glm::vec3& value) {
	return std::isfinite(value.x) && std::isfinite(value.y) && std::isfinite(value.z);
}

bool valid_extent_bounds(const mdx::Extent& extent) {
	return finite_vec3(extent.minimum) && finite_vec3(extent.maximum)
		&& extent.minimum.x <= extent.maximum.x && extent.minimum.y <= extent.maximum.y && extent.minimum.z <= extent.maximum.z;
}

float preview_radius_for_extent(const mdx::Extent& extent) {
	float radius = std::isfinite(extent.bounds_radius) && extent.bounds_radius > 0.0f ? extent.bounds_radius : 0.0f;
	if (valid_extent_bounds(extent)) {
		radius = std::max(radius, glm::length(extent.maximum - extent.minimum) * 0.5f);
	}
	return std::max(radius * k_preview_radius_padding, k_min_preview_radius);
}

float preview_center_z_for_extent(const mdx::Extent& extent) {
	if (!valid_extent_bounds(extent)) {
		return 0.0f;
	}
	return extent.minimum.z + (extent.maximum.z - extent.minimum.z) * 0.5f;
}
} // namespace

ModelEditorMetalWidget::ModelEditorMetalWidget(
	QWidget* parent,
	std::shared_ptr<mdx::MDX> model,
	std::optional<std::pair<int, std::string>> replaceable_id_override
)
	: QWidget(parent),
	  mdx(std::move(model)),
	  replaceable_id_override(std::move(replaceable_id_override)),
	  renderer(hive::metal::make_model_frame_renderer()) {
	setAttribute(Qt::WA_NativeWindow);
	setAttribute(Qt::WA_PaintOnScreen);
	setAttribute(Qt::WA_NoSystemBackground);
	setAttribute(Qt::WA_OpaquePaintEvent);
	setAutoFillBackground(false);
	setMouseTracking(true);
	setFocus();
	setFocusPolicy(Qt::WheelFocus);

	elapsed_timer.start();
	rebuild_mesh();
	initialize_controls();
	populate_controls();

	frame_timer.setInterval(16);
	connect(&frame_timer, &QTimer::timeout, this, [this] {
		update();
	});
	frame_timer.start();
}

ModelEditorMetalWidget::~ModelEditorMetalWidget() = default;

bool ModelEditorMetalWidget::ensure_surface() {
	if (surface_initialized) {
		return true;
	}

	surface_initialized = surface.initialize(reinterpret_cast<void*>(winId()));
	if (surface_initialized) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}

	return surface_initialized;
}

void ModelEditorMetalWidget::rebuild_mesh() {
	mesh.reset();
	if (!mdx) {
		return;
	}

	try {
		auto preview_model = std::make_shared<mdx::MDX>(*mdx);
		mesh = std::make_shared<SkinnedMesh>(preview_model, replaceable_id_override);
		skeleton = SkeletalModelInstance(preview_model);
		SkeletalModelInstance::pick_preview_sequence(skeleton, *preview_model);
		skeleton.update_location(glm::vec3(0.0f), glm::quat(), glm::vec3(1.0f));
		skeleton.update(0.0);
		recenter_camera();
	} catch (const std::exception& error) {
		std::println("Metal model editor preview setup failed: {}", error.what());
		mesh.reset();
	}
}

void ModelEditorMetalWidget::recenter_camera() {
	if (!mesh || !mesh->mdx) {
		return;
	}

	mdx::Extent extent = mesh->mdx->extent;
	if (!mesh->mdx->sequences.empty() && skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size()) {
		extent = mesh->mdx->sequences[skeleton.sequence_index].extent;
	}

	const float radius = preview_radius_for_extent(extent);
	camera.distance = radius / std::sin(camera.fov_rad * 0.5f);
	camera.position.z = preview_center_z_for_extent(extent);
	camera.update(0.0);
}

void ModelEditorMetalWidget::initialize_controls() {
	controls_panel = new QFrame(this);
	controls_panel->setAttribute(Qt::WA_NativeWindow);
	controls_panel->setAttribute(Qt::WA_StyledBackground);
	controls_panel->setObjectName(QStringLiteral("modelEditorMetalControls"));
	controls_panel->setStyleSheet(QStringLiteral(
		"QFrame#modelEditorMetalControls { background-color: rgba(38, 38, 38, 230); border: 1px solid rgb(72, 72, 72); }"
		"QLabel { color: rgb(255, 254, 255); }"
	));

	auto* layout = new QVBoxLayout(controls_panel);
	layout->setContentsMargins(12, 10, 12, 10);
	layout->setSpacing(8);

	auto* title = new QLabel(QStringLiteral("General"), controls_panel);
	QFont title_font = title->font();
	title_font.setBold(true);
	title->setFont(title_font);
	layout->addWidget(title);

	fps_label = new QLabel(controls_panel);
	model_name_label = new QLabel(controls_panel);
	geometry_label = new QLabel(controls_panel);
	extent_label = new QLabel(controls_panel);
	extent_label->setWordWrap(true);
	layout->addWidget(fps_label);
	layout->addWidget(model_name_label);
	layout->addWidget(geometry_label);
	layout->addWidget(extent_label);

	auto* button_row = new QHBoxLayout;
	button_row->setSpacing(6);
	auto* edit_mdl_button = new QPushButton(QStringLiteral("Edit MDL"), controls_panel);
	auto* save_mdx_button = new QPushButton(QStringLiteral("Save MDX"), controls_panel);
	auto* recenter_button = new QPushButton(QStringLiteral("Recenter"), controls_panel);
	button_row->addWidget(edit_mdl_button);
	button_row->addWidget(save_mdx_button);
	button_row->addWidget(recenter_button);
	layout->addLayout(button_row);

	auto* animation_title = new QLabel(QStringLiteral("Animation"), controls_panel);
	animation_title->setFont(title_font);
	layout->addWidget(animation_title);

	sequence_combo = new QComboBox(controls_panel);
	sequence_info_label = new QLabel(controls_panel);
	current_frame_label = new QLabel(controls_panel);
	sequence_info_label->setWordWrap(true);
	layout->addWidget(sequence_combo);
	layout->addWidget(sequence_info_label);
	layout->addWidget(current_frame_label);

	connect(edit_mdl_button, &QPushButton::clicked, this, &ModelEditorMetalWidget::open_mdl_in_text_editor);
	connect(save_mdx_button, &QPushButton::clicked, this, &ModelEditorMetalWidget::save_mdx);
	connect(recenter_button, &QPushButton::clicked, this, &ModelEditorMetalWidget::recenter_camera);
	connect(sequence_combo, QOverload<int>::of(&QComboBox::currentIndexChanged), this, &ModelEditorMetalWidget::set_sequence);

	layout_controls();
	controls_panel->raise();
}

void ModelEditorMetalWidget::populate_controls() {
	if (!controls_panel || !sequence_combo) {
		return;
	}

	{
		const QSignalBlocker blocker(sequence_combo);
		sequence_combo->clear();
		if (mesh && mesh->mdx) {
			for (std::size_t i = 0; i < mesh->mdx->sequences.size(); ++i) {
				sequence_combo->addItem(QString::fromStdString(mesh->mdx->sequences[i].name), static_cast<int>(i));
			}
			sequence_combo->setEnabled(!mesh->mdx->sequences.empty());
			if (skeleton.sequence_index >= 0 && skeleton.sequence_index < sequence_combo->count()) {
				sequence_combo->setCurrentIndex(skeleton.sequence_index);
			}
		} else {
			sequence_combo->setEnabled(false);
		}
	}

	update_controls();
}

void ModelEditorMetalWidget::update_controls() {
	if (!controls_panel || !mesh || !mesh->mdx) {
		return;
	}

	if (fps_label) {
		const double fps = delta > 0.0 ? 1.0 / delta : 0.0;
		fps_label->setText(QStringLiteral("FPS: %1").arg(fps, 0, 'f', 2));
	}
	if (model_name_label) {
		model_name_label->setText(QStringLiteral("name: %1").arg(QString::fromStdString(mesh->mdx->name)));
	}
	if (geometry_label) {
		std::size_t vertices = 0;
		std::size_t triangles = 0;
		for (const auto& geoset : mesh->mdx->geosets) {
			vertices += geoset.vertices.size();
			triangles += geoset.faces.size() / 3;
		}
		geometry_label->setText(QStringLiteral("Vertices: %1  Triangles: %2").arg(vertices).arg(triangles));
	}
	if (extent_label) {
		const auto& e = mesh->mdx->extent;
		extent_label->setText(
			QStringLiteral("Extents: min %1, %2, %3  max %4, %5, %6")
				.arg(e.minimum.x, 0, 'f', 2)
				.arg(e.minimum.y, 0, 'f', 2)
				.arg(e.minimum.z, 0, 'f', 2)
				.arg(e.maximum.x, 0, 'f', 2)
				.arg(e.maximum.y, 0, 'f', 2)
				.arg(e.maximum.z, 0, 'f', 2)
		);
	}

	const bool has_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size();
	if (has_sequence) {
		const auto& sequence = mesh->mdx->sequences[static_cast<std::size_t>(skeleton.sequence_index)];
		if (sequence_info_label) {
			sequence_info_label->setText(
				QStringLiteral("Start frame: %1  End frame: %2  Looping: %3")
					.arg(sequence.start_frame)
					.arg(sequence.end_frame)
					.arg(sequence.flags == mdx::Sequence::Flags::looping ? QStringLiteral("true") : QStringLiteral("false"))
			);
		}
		if (current_frame_label) {
			current_frame_label->setText(QStringLiteral("Current frame: %1").arg(skeleton.current_frame));
		}
	} else {
		if (sequence_info_label) {
			sequence_info_label->setText(QStringLiteral("No animation sequences"));
		}
		if (current_frame_label) {
			current_frame_label->clear();
		}
	}
}

void ModelEditorMetalWidget::layout_controls() {
	if (!controls_panel) {
		return;
	}
	if (smoke_capture_mode) {
		controls_panel->hide();
		return;
	}

	const int panel_width = std::min(std::max(width() - 24, 220), 360);
	controls_panel->setFixedWidth(panel_width);
	controls_panel->adjustSize();
	const int panel_height = std::min(controls_panel->sizeHint().height(), std::max(height() - 24, 0));
	controls_panel->setGeometry(12, 12, panel_width, panel_height);
	controls_panel->raise();
	controls_panel->show();
}

void ModelEditorMetalWidget::set_smoke_capture_mode(const bool enabled) {
	smoke_capture_mode = enabled;
	if (smoke_capture_mode) {
		frame_timer.stop();
	} else if (!frame_timer.isActive()) {
		elapsed_timer.start();
		frame_timer.start();
	}
	if (controls_panel) {
		controls_panel->setVisible(!smoke_capture_mode && isVisible());
	}
	if (surface_initialized) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}
	layout_controls();
}

void ModelEditorMetalWidget::open_mdl_in_text_editor() {
	if (!mdx) {
		return;
	}

	const QByteArray mdl = QByteArray::fromStdString(mdx->to_mdl());
	const QString path = QStandardPaths::writableLocation(QStandardPaths::TempLocation) + "/" + model_file_stem(*mdx) + ".mdl";
	std::ofstream file(path.toStdString(), std::ios::binary);
	if (!file) {
		QMessageBox::warning(this, QStringLiteral("Error"), QStringLiteral("Could not open temporary MDL file for writing."));
		return;
	}
	file.write(mdl.constData(), static_cast<std::streamsize>(mdl.size()));
	file.close();
	QDesktopServices::openUrl(QUrl::fromLocalFile(path));
}

void ModelEditorMetalWidget::save_mdx() {
	if (!mdx) {
		return;
	}

	const QString file_name = QFileDialog::getSaveFileName(
		this,
		QStringLiteral("Save MDX"),
		QStandardPaths::writableLocation(QStandardPaths::TempLocation),
		QStringLiteral("MDX (*.mdx *.MDX)")
	);
	if (file_name.isEmpty()) {
		return;
	}

	const auto mdx_data = mdx->to_mdx();
	std::ofstream file(file_name.toStdString(), std::ios::binary);
	if (!file) {
		QMessageBox::warning(this, QStringLiteral("Error"), QStringLiteral("Could not open MDX file for writing."));
		return;
	}
	file.write(reinterpret_cast<const char*>(mdx_data.buffer.data()), static_cast<std::streamsize>(mdx_data.buffer.size()));
}

void ModelEditorMetalWidget::set_sequence(const int index) {
	if (!mesh || !mesh->mdx || index < 0 || static_cast<std::size_t>(index) >= mesh->mdx->sequences.size()) {
		return;
	}

	skeleton.set_sequence(index);
	update_controls();
	update();
}

bool ModelEditorMetalWidget::smoke_capture_preview(std::vector<std::uint8_t>& captured_bgra, int& captured_width, int& captured_height) {
	if (width() <= 0 || height() <= 0) {
		resize(512, 512);
	}
	if (height() > 0) {
		camera.aspect_ratio = static_cast<float>(width()) / static_cast<float>(height());
		camera.update(delta);
	}

	if (smoke_capture_mode && !draw_frame()) {
		return false;
	}
	return draw_frame(&captured_bgra, &captured_width, &captured_height) && !captured_bgra.empty();
}

bool ModelEditorMetalWidget::draw_frame(std::vector<std::uint8_t>* captured_bgra, int* captured_width, int* captured_height) {
	if (captured_width != nullptr) {
		*captured_width = 0;
	}
	if (captured_height != nullptr) {
		*captured_height = 0;
	}

	delta = smoke_capture_mode ? 1.0 / 30.0 : elapsed_timer.nsecsElapsed() / 1'000'000'000.0;
	elapsed_timer.start();

	if (!ensure_surface()) {
		return false;
	}

	last_smoke_stats = {};
	if (!mesh) {
		surface.draw_clear(0.3f, 0.3f, 0.3f, 1.0f);
		return captured_bgra == nullptr;
	}

	skeleton.update_location(glm::vec3(0.0f), glm::quat(), glm::vec3(1.0f));
	skeleton.update(delta);
	camera.update(delta);
	update_controls();

	MetalSurface::Frame surface_frame = surface.begin_frame(0.3f, 0.3f, 0.3f, 1.0f);
	if (!surface_frame) {
		return false;
	}
	const int frame_width = surface_frame.drawable_width();
	const int frame_height = surface_frame.drawable_height();

	hive::metal::FrameContext frame_context {
		.device = surface_frame.device_opaque(),
		.command_queue = surface_frame.command_queue_opaque(),
		.command_buffer = surface_frame.command_buffer_opaque(),
		.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
		.drawable_width = surface_frame.drawable_width(),
		.drawable_height = surface_frame.drawable_height(),
		.scale = surface_frame.scale(),
	};

	hive::metal::MetalModelFrame model_frame;
	model_frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(camera.projection_view);
	model_frame.light_direction = camera.direction;
	model_frame.models = hive::metal::make_model_preview_cpu_frame(
		mesh,
		skeleton,
		camera.position,
		camera.direction,
		camera.distance,
		glm::vec3(1.0f),
		0
	);
	model_frame.particles = hive::metal::make_particle_emitter2_preview_cpu_frame(mesh, skeleton, camera.X, camera.Y, camera.direction);

	last_smoke_stats.model_name = mesh->mdx ? mesh->mdx->name : std::string();
	last_smoke_stats.geoset_count = mesh->mdx ? mesh->mdx->geosets.size() : 0;
	last_smoke_stats.source_texture_count = mesh->mdx ? mesh->mdx->textures.size() : 0;
	last_smoke_stats.sequence_index = skeleton.sequence_index;
	last_smoke_stats.valid_sequence = mesh->mdx && skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size();
	last_smoke_stats.frame = hive::metal::summarize_model_frame(model_frame);

	bool render_ok = true;
	if (!renderer->render_models(frame_context, model_frame)) {
		std::println("Metal model editor renderer error: {}", renderer->last_error());
		render_ok = false;
	}

	std::vector<std::uint8_t> frame_pixels;
	const bool present_ok = surface.end_frame(std::move(surface_frame), captured_bgra != nullptr ? &frame_pixels : nullptr);
	if (present_ok && captured_bgra != nullptr) {
		*captured_bgra = std::move(frame_pixels);
		if (captured_width != nullptr) {
			*captured_width = frame_width;
		}
		if (captured_height != nullptr) {
			*captured_height = frame_height;
		}
	}

	return render_ok && present_ok;
}

void ModelEditorMetalWidget::showEvent(QShowEvent* event) {
	QWidget::showEvent(event);
	ensure_surface();
	layout_controls();
}

void ModelEditorMetalWidget::resizeEvent(QResizeEvent* event) {
	QWidget::resizeEvent(event);

	if (height() > 0) {
		camera.aspect_ratio = static_cast<float>(width()) / static_cast<float>(height());
		camera.update(delta);
	}
	if (ensure_surface()) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}
	layout_controls();
}

void ModelEditorMetalWidget::paintEvent(QPaintEvent*) {
	if (smoke_capture_mode) {
		return;
	}
	draw_frame();
}

void ModelEditorMetalWidget::keyPressEvent(QKeyEvent* event) {
	input_handler.keys_pressed.emplace(event->key());
	QWidget::keyPressEvent(event);
}

void ModelEditorMetalWidget::keyReleaseEvent(QKeyEvent* event) {
	input_handler.keys_pressed.erase(event->key());
	QWidget::keyReleaseEvent(event);
}

void ModelEditorMetalWidget::mouseMoveEvent(QMouseEvent* event) {
	input_handler.mouse_move_event(event);
	camera.mouse_move_event(event, input_handler);
}

void ModelEditorMetalWidget::mousePressEvent(QMouseEvent* event) {
	camera.mouse_press_event(event);
	QWidget::mousePressEvent(event);
}

void ModelEditorMetalWidget::mouseReleaseEvent(QMouseEvent* event) {
	camera.mouse_release_event(event);
	QWidget::mouseReleaseEvent(event);
}

void ModelEditorMetalWidget::wheelEvent(QWheelEvent* event) {
	camera.mouse_scroll_event(event);
}

#include "moc_model_editor_metal_widget.cpp"

#endif
