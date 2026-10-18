---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_map_frame_renderer.cpp"
ref: HEAD
---

#include "rendering/metal_map_frame_renderer.h"

#include "rendering/metal_map_renderer.h"

namespace hive::metal {

namespace {

class MetalFrameRenderer final : public IMapFrameRenderer {
  public:
	void invalidate_resources() override {
		renderer.invalidate_resources();
	}

	bool render(const FrameContext& frame_context, const MetalMapFrame& frame) override {
		return renderer.render(frame_context, frame);
	}

	bool render_models(const FrameContext& frame_context, const MetalModelFrame& frame) override {
		return renderer.render_models(frame_context, frame);
	}

	std::optional<std::uint32_t> pick_model_id(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		const glm::vec2 mouse_position
	) override {
		return renderer.pick_model_id(frame_context, model_frame, camera, mouse_position);
	}

	bool capture_model_picking_frame(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		std::vector<std::uint8_t>& bgra_pixels,
		int& width,
		int& height
	) override {
		return renderer.capture_model_picking_frame(frame_context, model_frame, camera, bgra_pixels, width, height);
	}

	const std::string& last_error() const override {
		return renderer.last_error();
	}

	const MetalTextureFallbackStats& last_texture_fallback_stats() const override {
		return renderer.last_texture_fallback_stats();
	}

  private:
	MetalMapRenderer renderer;
};

} // namespace

std::unique_ptr<IModelFrameRenderer> make_model_frame_renderer() {
	return std::make_unique<MetalFrameRenderer>();
}

std::unique_ptr<IMapFrameRenderer> make_map_frame_renderer() {
	return std::make_unique<MetalFrameRenderer>();
}

} // namespace hive::metal
