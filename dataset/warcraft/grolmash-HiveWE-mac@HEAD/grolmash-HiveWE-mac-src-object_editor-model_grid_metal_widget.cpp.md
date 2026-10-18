---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/object_editor/model_grid_metal_widget.cpp"
ref: HEAD
---

#include "model_grid_metal_widget.h"

#if HIVEWE_USE_METAL_RUNTIME

#include <algorithm>
#include <cmath>
#include <cctype>
#include <optional>
#include <print>
#include <utility>

#include <QHelpEvent>
#include <QMouseEvent>
#include <QPaintEvent>
#include <QImage>
#include <QLabel>
#include <QPainter>
#include <QResizeEvent>
#include <QShowEvent>
#include <QToolTip>
#include <QWheelEvent>

#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/quaternion.hpp>

#include "rendering/metal_map_frame_renderer.h"
#include "rendering/metal_map_frame_builders.h"
#include "rendering/metal_particle_emitter1_model_frame.h"
#include "std_module_compat.h"
import MDX;
import ResourceManager;

namespace {
constexpr float k_fov_deg = 50.f;
constexpr float k_near = 0.05f;
constexpr float k_far = 2000.f;
constexpr float k_min_preview_radius = 64.0f;
constexpr float k_preview_radius_padding = 1.1f;

const char* category_label(const ModelCategory category) {
	switch (category) {
		case ModelCategory::Abilities:
			return "Abilities";
		case ModelCategory::Buildings:
			return "Buildings";
		case ModelCategory::Doodads:
			return "Doodads";
		case ModelCategory::Environment:
			return "Environment";
		case ModelCategory::Objects:
			return "Objects";
		case ModelCategory::SharedModels:
			return "Shared Models";
		case ModelCategory::Units:
			return "Units";
		case ModelCategory::Map:
			return "Map";
		default:
			return "";
	}
}

QString css_rgb(const QColor& color) {
	return QStringLiteral("rgb(%1, %2, %3)").arg(color.red()).arg(color.green()).arg(color.blue());
}

std::string lowercase_copy(std::string s) {
	for (char& c : s) {
		c = static_cast<char>(std::tolower(static_cast<unsigned char>(c)));
	}
	return s;
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

ModelGridMetalWidget::ModelGridMetalWidget(const std::vector<ModelEntry>& entries, QWidget* parent)
	: QWidget(parent),
	  renderer(hive::metal::make_model_frame_renderer()) {
	all_cells.reserve(entries.size());
	for (const auto& entry : entries) {
		PreviewCell cell;
		cell.path = entry.path;
		cell.category = entry.category;
		cell.replaceable_id_override = entry.replaceable_id_override;
		all_cells.push_back(std::move(cell));
	}

	category_mask.set();

	setAttribute(Qt::WA_NativeWindow);
	setAttribute(Qt::WA_PaintOnScreen);
	setAttribute(Qt::WA_NoSystemBackground);
	setAttribute(Qt::WA_OpaquePaintEvent);
	setAutoFillBackground(false);
	setMouseTracking(false);
	setFocusPolicy(Qt::WheelFocus);

	elapsed_timer.start();
	initialize_header_labels();

	frame_timer.setInterval(16);
	connect(&frame_timer, &QTimer::timeout, this, [this] {
		update();
	});
	frame_timer.start();
}

ModelGridMetalWidget::~ModelGridMetalWidget() = default;

bool ModelGridMetalWidget::ensure_surface() {
	if (surface_initialized) {
		return true;
	}

	surface_initialized = surface.initialize(reinterpret_cast<void*>(winId()));
	if (surface_initialized) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}

	return surface_initialized;
}

bool ModelGridMetalWidget::smoke_capture_preview(std::vector<std::uint8_t>& captured_bgra, int& captured_width, int& captured_height) {
	return smoke_capture_preview_at_scroll(0, captured_bgra, captured_width, captured_height);
}

bool ModelGridMetalWidget::smoke_capture_preview_at_scroll(
	const int requested_scroll_offset_y,
	std::vector<std::uint8_t>& captured_bgra,
	int& captured_width,
	int& captured_height
) {
	if (width() <= 0 || height() <= 0) {
		resize(cell_size * 2, cell_size + header_height);
	}
	columns = std::max(1, width() / cell_size);
	rebuild_layout();
	scroll_offset_y = std::clamp(requested_scroll_offset_y, 0, max_scroll_offset());
	sync_header_labels();

	if (smoke_capture_mode && !draw_frame()) {
		return false;
	}
	return draw_frame(&captured_bgra, &captured_width, &captured_height) && !captured_bgra.empty();
}

bool ModelGridMetalWidget::draw_frame(std::vector<std::uint8_t>* captured_bgra, int* captured_width, int* captured_height) {
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

	MetalSurface::Frame surface_frame = surface.begin_frame(0.15f, 0.15f, 0.15f, 1.0f);
	if (!surface_frame) {
		return false;
	}

	last_smoke_stats = {};
	last_smoke_stats.scroll_offset_y = scroll_offset_y;
	last_smoke_stats.content_height = content_height_px();
	const double scale = surface_frame.scale() > 0.0 ? surface_frame.scale() : 1.0;
	const int drawable_width = surface_frame.drawable_width();
	const int drawable_height = surface_frame.drawable_height();
	bool render_ok = true;

	auto make_context = [&](const int x, const int y, const int w, const int h, const bool load_existing) {
		const int px = static_cast<int>(std::floor(static_cast<double>(x) * scale));
		const int py = static_cast<int>(std::floor(static_cast<double>(y) * scale));
		const int pw = static_cast<int>(std::ceil(static_cast<double>(w) * scale));
		const int ph = static_cast<int>(std::ceil(static_cast<double>(h) * scale));
		const int sx = std::clamp(px, 0, std::max(0, drawable_width - 1));
		const int sy = std::clamp(py, 0, std::max(0, drawable_height - 1));
		const int ex = std::clamp(px + pw, 0, drawable_width);
		const int ey = std::clamp(py + ph, 0, drawable_height);

		return hive::metal::FrameContext {
			.device = surface_frame.device_opaque(),
			.command_queue = surface_frame.command_queue_opaque(),
			.command_buffer = surface_frame.command_buffer_opaque(),
			.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
			.drawable_width = drawable_width,
			.drawable_height = drawable_height,
			.scale = scale,
			.viewport_x = px,
			.viewport_y = py,
			.viewport_width = std::max(1, pw),
			.viewport_height = std::max(1, ph),
			.scissor_x = sx,
			.scissor_y = sy,
			.scissor_width = std::max(1, ex - sx),
			.scissor_height = std::max(1, ey - sy),
			.load_existing_render_target = load_existing,
		};
	};

	const int view_top = scroll_offset_y;
	const int view_bottom = scroll_offset_y + height();
	bool rendered_anything = false;

	for (const auto& row : layout) {
		if (row.kind != LayoutRow::Kind::Cells) {
			continue;
		}
		if (row.y_top + row.height <= view_top || row.y_top >= view_bottom) {
			continue;
		}

		const int row_y_screen = row.y_top - scroll_offset_y;
		for (int c = 0; c < row.visible_count; ++c) {
			const int cell_idx = visible_indices[row.visible_offset + c];
			auto& cell = all_cells[cell_idx];
			if (!cell.loaded && !cell.load_failed) {
				load_cell(cell);
			}
			if (cell.load_failed) {
				if (last_smoke_stats.failed_cell_count == 0) {
					last_smoke_stats.first_failed_path = cell.path;
					last_smoke_stats.first_failed_error = cell.load_error_message;
				}
				++last_smoke_stats.failed_cell_count;
				continue;
			}
			if (!cell.loaded || !cell.mesh) {
				continue;
			}
			++last_smoke_stats.loaded_cell_count;

			cell.skeleton.update_location(glm::vec3(0.f), glm::quat(), glm::vec3(1.f));
			cell.skeleton.update(delta);

			const int cell_x = c * cell_size;
			const int cell_y = row_y_screen;
			const int clip_left = std::max(cell_x, 0);
			const int clip_top = std::max(cell_y, 0);
			const int clip_right = std::min(cell_x + cell_size, width());
			const int clip_bottom = std::min(cell_y + cell_size, height());
			if (clip_right <= clip_left || clip_bottom <= clip_top) {
				continue;
			}

			const glm::vec3 direction = glm::normalize(glm::vec3 {-1.f, 1.f, -0.5f});
			const glm::vec3 up = {0.f, 0.f, 1.f};
			const glm::vec3 eye = cell.fit_position - direction * cell.fit_distance;
			const glm::mat4 view = glm::lookAt(eye, cell.fit_position, up);
			const glm::mat4 projection = glm::perspective(glm::radians(k_fov_deg), 1.f, k_near, k_far);
			const glm::vec3 camera_right = glm::normalize(glm::cross(direction, up));
			const glm::vec3 camera_up = glm::normalize(glm::cross(camera_right, direction));

			hive::metal::MetalModelFrame model_frame;
			model_frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(projection * view);
			model_frame.light_direction = direction;
			model_frame.models = hive::metal::make_model_preview_cpu_frame(
				cell.mesh,
				cell.skeleton,
				cell.fit_position,
				direction,
				cell.fit_distance,
				glm::vec3(1.0f),
				1
			);
			model_frame.particles =
				hive::metal::make_particle_emitter2_preview_cpu_frame(cell.mesh, cell.skeleton, camera_right, camera_up, direction);

			auto frame_context = make_context(cell_x, cell_y, cell_size, cell_size, rendered_anything);
			frame_context.scissor_x = static_cast<int>(std::floor(static_cast<double>(clip_left) * scale));
			frame_context.scissor_y = static_cast<int>(std::floor(static_cast<double>(clip_top) * scale));
			frame_context.scissor_width = std::max(1, static_cast<int>(std::ceil(static_cast<double>(clip_right - clip_left) * scale)));
			frame_context.scissor_height = std::max(1, static_cast<int>(std::ceil(static_cast<double>(clip_bottom - clip_top) * scale)));

			if (!renderer->render_models(frame_context, model_frame)) {
				std::println("Metal model grid renderer error: {}", renderer->last_error());
				render_ok = false;
				continue;
			}
			if (last_smoke_stats.rendered_cell_count == 0) {
				last_smoke_stats.first_rendered_path = cell.path;
				last_smoke_stats.first_rendered_x = cell_x;
				last_smoke_stats.first_rendered_y = cell_y;
				last_smoke_stats.first_rendered_width = cell_size;
				last_smoke_stats.first_rendered_height = cell_size;
				last_smoke_stats.first_rendered_fit_distance = cell.fit_distance;
				last_smoke_stats.first_rendered_fit_z = cell.fit_position.z;
			}
			hive::metal::accumulate_model_frame_stats(last_smoke_stats.frame, model_frame);
			++last_smoke_stats.rendered_cell_count;
			rendered_anything = true;
		}
	}

	if (!rendered_anything) {
		hive::metal::MetalModelFrame empty_frame;
		const auto frame_context = make_context(0, 0, width(), height(), false);
		if (!renderer->render_models(frame_context, empty_frame)) {
			std::println("Metal model grid clear error: {}", renderer->last_error());
			render_ok = false;
		}
	}

	std::vector<std::uint8_t> frame_pixels;
	const bool present_ok = surface.end_frame(std::move(surface_frame), captured_bgra != nullptr ? &frame_pixels : nullptr);
	if (present_ok && captured_bgra != nullptr) {
		paint_capture_overlay(frame_pixels, drawable_width, drawable_height, scale);
		*captured_bgra = std::move(frame_pixels);
		if (captured_width != nullptr) {
			*captured_width = drawable_width;
		}
		if (captured_height != nullptr) {
			*captured_height = drawable_height;
		}
	}

	return render_ok && present_ok;
}

void ModelGridMetalWidget::paint_capture_overlay(
	std::vector<std::uint8_t>& captured_bgra,
	const int drawable_width,
	const int drawable_height,
	const double scale
) const {
	const std::size_t expected_size = static_cast<std::size_t>(std::max(0, drawable_width))
		* static_cast<std::size_t>(std::max(0, drawable_height)) * 4u;
	if (drawable_width <= 0 || drawable_height <= 0 || captured_bgra.size() < expected_size) {
		return;
	}

	QImage image(captured_bgra.data(), drawable_width, drawable_height, drawable_width * 4, QImage::Format_ARGB32);
	if (image.isNull()) {
		return;
	}

	QPainter painter(&image);
	const double paint_scale = scale > 0.0 ? scale : 1.0;
	painter.scale(paint_scale, paint_scale);

	QFont header_font = font();
	header_font.setBold(true);
	if (header_font.pointSizeF() > 0.0) {
		header_font.setPointSizeF(header_font.pointSizeF() + 15.0);
	} else if (header_font.pixelSize() > 0) {
		header_font.setPixelSize(header_font.pixelSize() + 20);
	}
	painter.setFont(header_font);

	const QColor text_color = palette().color(QPalette::WindowText);
	const QColor sep_color = palette().color(QPalette::Mid);
	const QColor background_color(38, 38, 38);
	constexpr QColor warning_text_color = QColorConstants::DarkRed;
	const int view_top = scroll_offset_y;
	const int view_bottom = scroll_offset_y + height();

	for (const auto& row : layout) {
		if (row.y_top + row.height <= view_top || row.y_top >= view_bottom) {
			continue;
		}

		if (row.kind == LayoutRow::Kind::Header) {
			const int y = row.y_top - scroll_offset_y;
			painter.fillRect(QRect(0, y, width(), row.height), background_color);
			const QRect text_rect(20, y, std::max(0, width() - 16), row.height);
			painter.setPen(text_color);
			painter.drawText(text_rect, Qt::AlignVCenter | Qt::AlignLeft, QString::fromUtf8(category_label(row.category)));
			painter.setPen(sep_color);
			painter.drawLine(0, y + row.height - 1, width(), y + row.height - 1);
			continue;
		}

		if (row.kind != LayoutRow::Kind::Cells) {
			continue;
		}

		const int row_y_screen = row.y_top - scroll_offset_y;
		painter.setPen(warning_text_color);
		for (int c = 0; c < row.visible_count; ++c) {
			const int cell_idx = visible_indices[row.visible_offset + c];
			if (!all_cells[cell_idx].load_failed) {
				continue;
			}
			const QRect rect(c * cell_size, row_y_screen, cell_size, cell_size);
			painter.drawText(rect, Qt::AlignCenter, QStringLiteral("Error"));
		}
	}
}

void ModelGridMetalWidget::initialize_header_labels() {
	QFont header_font = font();
	header_font.setBold(true);
	if (header_font.pointSizeF() > 0.0) {
		header_font.setPointSizeF(header_font.pointSizeF() + 15.0);
	} else if (header_font.pixelSize() > 0) {
		header_font.setPixelSize(header_font.pixelSize() + 20);
	}

	const QString text_color = css_rgb(palette().color(QPalette::WindowText));
	const QString sep_color = css_rgb(palette().color(QPalette::Mid));
	const QString background_color = QStringLiteral("rgb(38, 38, 38)");

	for (std::size_t i = 0; i < header_labels.size(); ++i) {
		auto* label = new QLabel(QString::fromUtf8(category_label(static_cast<ModelCategory>(i))), this);
		label->setAlignment(Qt::AlignVCenter | Qt::AlignLeft);
		label->setAttribute(Qt::WA_NativeWindow);
		label->setAttribute(Qt::WA_TransparentForMouseEvents);
		label->setAutoFillBackground(false);
		label->setContentsMargins(20, 0, 0, 0);
		label->setFont(header_font);
		label->setStyleSheet(QStringLiteral("QLabel { background-color: %1; color: %2; border-bottom: 1px solid %3; }")
								 .arg(background_color, text_color, sep_color));
		label->hide();
		header_labels[i] = label;
	}
}

void ModelGridMetalWidget::sync_header_labels() {
	for (auto* label : header_labels) {
		if (label) {
			label->hide();
		}
	}
	for (auto* label : failed_cell_labels) {
		if (label) {
			label->hide();
		}
	}
	if (smoke_capture_mode) {
		return;
	}

	auto ensure_failed_cell_label = [this](const std::size_t index) {
		while (failed_cell_labels.size() <= index) {
			auto* label = new QLabel(QStringLiteral("Error"), this);
			label->setAlignment(Qt::AlignCenter);
			label->setAttribute(Qt::WA_NativeWindow);
			label->setAttribute(Qt::WA_TransparentForMouseEvents);
			label->setAutoFillBackground(false);
			QFont error_font = font();
			error_font.setBold(true);
			label->setFont(error_font);
			label->setStyleSheet(QStringLiteral("QLabel { background-color: transparent; color: %1; }")
									 .arg(css_rgb(QColorConstants::DarkRed)));
			label->hide();
			failed_cell_labels.push_back(label);
		}
		return failed_cell_labels[index];
	};

	const int view_top = scroll_offset_y;
	const int view_bottom = scroll_offset_y + height();
	std::size_t failed_label_index = 0;

	for (const auto& row : layout) {
		if (row.y_top + row.height <= view_top || row.y_top >= view_bottom) {
			continue;
		}

		if (row.kind == LayoutRow::Kind::Header) {
			const std::size_t category_index = static_cast<std::size_t>(row.category);
			if (category_index >= header_labels.size() || !header_labels[category_index]) {
				continue;
			}

			auto* label = header_labels[category_index];
			label->setGeometry(0, row.y_top - scroll_offset_y, width(), row.height);
			label->raise();
			label->show();
			continue;
		}

		if (row.kind != LayoutRow::Kind::Cells) {
			continue;
		}

		const int row_y_screen = row.y_top - scroll_offset_y;
		for (int c = 0; c < row.visible_count; ++c) {
			const int cell_idx = visible_indices[row.visible_offset + c];
			const auto& cell = all_cells[cell_idx];
			if (!cell.load_failed) {
				continue;
			}

			auto* label = ensure_failed_cell_label(failed_label_index++);
			label->setGeometry(c * cell_size, row_y_screen, cell_size, cell_size);
			label->setToolTip(QString::fromStdString(cell.load_error_message));
			label->raise();
			label->show();
		}
	}
}

void ModelGridMetalWidget::set_smoke_capture_mode(const bool enabled) {
	smoke_capture_mode = enabled;
	if (smoke_capture_mode) {
		frame_timer.stop();
	} else if (!frame_timer.isActive()) {
		elapsed_timer.start();
		frame_timer.start();
	}
	if (surface_initialized) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}
	sync_header_labels();
}

bool ModelGridMetalWidget::event(QEvent* event) {
	if (event->type() == QEvent::ToolTip) {
		const auto* help = dynamic_cast<QHelpEvent*>(event);
		if (help == nullptr) {
			return QWidget::event(event);
		}

		const int cell_index = cell_index_at(help->pos());
		if (cell_index >= 0) {
			const auto& cell = all_cells[cell_index];
			if (cell.load_failed && !cell.load_error_message.empty()) {
				QToolTip::showText(help->globalPos(), QString::fromStdString(cell.load_error_message), this);
				return true;
			}
		}
		QToolTip::hideText();
		event->ignore();
		return true;
	}
	return QWidget::event(event);
}

void ModelGridMetalWidget::load_cell(PreviewCell& cell) const {
	try {
		auto result = resource_manager.load<SkinnedMesh>(
			cell.path,
			model_grid_skinned_mesh_cache_identifier(cell.replaceable_id_override),
			cell.replaceable_id_override
		);
		if (!result) {
			cell.load_failed = true;
			cell.load_error_message = result.error();
			return;
		}

		cell.mesh = *result;
		cell.skeleton = SkeletalModelInstance(cell.mesh->mdx);
		SkeletalModelInstance::pick_preview_sequence(cell.skeleton, *cell.mesh->mdx);

		mdx::Extent extent = cell.mesh->mdx->extent;
		if (!cell.mesh->mdx->sequences.empty() && cell.skeleton.sequence_index >= 0
			&& static_cast<std::size_t>(cell.skeleton.sequence_index) < cell.mesh->mdx->sequences.size()) {
			extent = cell.mesh->mdx->sequences[cell.skeleton.sequence_index].extent;
		}
		const float radius = preview_radius_for_extent(extent);
		const float fov_rad = glm::radians(k_fov_deg);
		cell.fit_distance = radius / std::sin(fov_rad * 0.5f);
		cell.fit_position = glm::vec3(0.f, 0.f, preview_center_z_for_extent(extent));
		cell.loaded = true;
	} catch (const std::exception& e) {
		cell.load_failed = true;
		cell.load_error_message = e.what();
	} catch (...) {
		cell.load_failed = true;
		cell.load_error_message = "Unknown error loading model";
	}
}

void ModelGridMetalWidget::rebuild_layout() {
	layout.clear();
	visible_indices.clear();
	if (columns <= 0) {
		sync_header_labels();
		return;
	}

	std::vector<std::vector<int>> by_category(static_cast<std::size_t>(ModelCategory::Count));
	for (int i = 0; i < static_cast<int>(all_cells.size()); ++i) {
		const auto& cell = all_cells[i];
		const std::size_t category_index = static_cast<std::size_t>(cell.category);
		if (!category_mask.test(category_index)) {
			continue;
		}
		if (!search_query.empty()) {
			const std::string stem = lowercase_copy(cell.path.stem().string());
			if (stem.find(search_query) == std::string::npos) {
				continue;
			}
		}
		by_category[category_index].push_back(i);
	}

	int y_cursor = 0;
	for (std::size_t category_index = 0; category_index < static_cast<std::size_t>(ModelCategory::Count); ++category_index) {
		auto& bucket = by_category[category_index];
		if (bucket.empty()) {
			continue;
		}

		LayoutRow header;
		header.kind = LayoutRow::Kind::Header;
		header.category = static_cast<ModelCategory>(category_index);
		header.y_top = y_cursor;
		header.height = header_height;
		layout.push_back(header);
		y_cursor += header_height;

		const int offset = static_cast<int>(visible_indices.size());
		visible_indices.insert(visible_indices.end(), bucket.begin(), bucket.end());

		const int total = static_cast<int>(bucket.size());
		int remaining = total;
		int row_index = 0;
		while (remaining > 0) {
			const int this_row = std::min(remaining, columns);
			LayoutRow row;
			row.kind = LayoutRow::Kind::Cells;
			row.category = static_cast<ModelCategory>(category_index);
			row.visible_offset = offset + row_index * columns;
			row.visible_count = this_row;
			row.y_top = y_cursor;
			row.height = cell_size;
			layout.push_back(row);
			y_cursor += cell_size;
			remaining -= this_row;
			++row_index;
		}
	}

	sync_header_labels();
}

int ModelGridMetalWidget::content_height_px() const {
	if (layout.empty()) {
		return 0;
	}
	const auto& last = layout.back();
	return last.y_top + last.height;
}

int ModelGridMetalWidget::max_scroll_offset() const {
	return std::max(0, content_height_px() - height());
}

void ModelGridMetalWidget::emit_layout_change() {
	emit content_height_changed(content_height_px());
	emit scroll_changed(scroll_offset_y);
}

void ModelGridMetalWidget::showEvent(QShowEvent* event) {
	QWidget::showEvent(event);
	ensure_surface();
	sync_header_labels();
}

void ModelGridMetalWidget::resizeEvent(QResizeEvent* event) {
	QWidget::resizeEvent(event);

	const int new_columns = std::max(1, width() / cell_size);
	if (new_columns != columns || layout.empty()) {
		columns = new_columns;
		rebuild_layout();
	}
	scroll_offset_y = std::clamp(scroll_offset_y, 0, max_scroll_offset());
	emit_layout_change();

	if (ensure_surface()) {
		surface.resize(width(), height(), smoke_capture_mode ? 1.0 : devicePixelRatioF());
	}
	sync_header_labels();
}

void ModelGridMetalWidget::paintEvent(QPaintEvent*) {
	sync_header_labels();
	if (smoke_capture_mode) {
		return;
	}
	draw_frame();
	sync_header_labels();
}

int ModelGridMetalWidget::cell_index_at(const QPoint& position) const {
	if (columns <= 0) {
		return -1;
	}
	const int x = position.x();
	const int y = position.y() + scroll_offset_y;

	for (const auto& row : layout) {
		if (y < row.y_top || y >= row.y_top + row.height) {
			continue;
		}
		if (row.kind != LayoutRow::Kind::Cells) {
			return -1;
		}
		const int column = x / cell_size;
		if (column < 0 || column >= row.visible_count) {
			return -1;
		}
		return visible_indices[row.visible_offset + column];
	}
	return -1;
}

void ModelGridMetalWidget::mousePressEvent(QMouseEvent* event) {
	if (event->button() != Qt::LeftButton) {
		return;
	}
	const int cell_index = cell_index_at(event->pos());
	if (cell_index >= 0) {
		emit clicked(all_cells[cell_index].path);
	}
}

void ModelGridMetalWidget::mouseDoubleClickEvent(QMouseEvent* event) {
	if (event->button() != Qt::LeftButton) {
		return;
	}
	const int cell_index = cell_index_at(event->pos());
	if (cell_index >= 0) {
		emit double_clicked(all_cells[cell_index].path);
	}
}

void ModelGridMetalWidget::wheelEvent(QWheelEvent* event) {
	const int dy = event->angleDelta().y();
	if (dy == 0) {
		return;
	}
	const int new_offset = std::clamp(scroll_offset_y - dy, 0, max_scroll_offset());
	if (new_offset != scroll_offset_y) {
		scroll_offset_y = new_offset;
		emit scroll_changed(scroll_offset_y);
		sync_header_labels();
		update();
	}
	event->accept();
}

void ModelGridMetalWidget::set_scroll_offset(int y) {
	const int clamped = std::clamp(y, 0, max_scroll_offset());
	if (clamped != scroll_offset_y) {
		scroll_offset_y = clamped;
		sync_header_labels();
		update();
	}
}

void ModelGridMetalWidget::set_search(const QString& query) {
	std::string q = lowercase_copy(query.toStdString());
	if (q == search_query) {
		return;
	}
	search_query = std::move(q);
	rebuild_layout();
	scroll_offset_y = std::clamp(scroll_offset_y, 0, max_scroll_offset());
	emit_layout_change();
	sync_header_labels();
	update();
}

void ModelGridMetalWidget::set_categories(const std::bitset<static_cast<std::size_t>(ModelCategory::Count)> mask) {
	if (mask == category_mask) {
		return;
	}
	category_mask = mask;
	rebuild_layout();
	scroll_offset_y = std::clamp(scroll_offset_y, 0, max_scroll_offset());
	emit_layout_change();
	sync_header_labels();
	update();
}

#include "moc_model_grid_metal_widget.cpp"

#endif
