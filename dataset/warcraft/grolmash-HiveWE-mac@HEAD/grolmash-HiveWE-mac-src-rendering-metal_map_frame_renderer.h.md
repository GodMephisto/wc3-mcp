---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_map_frame_renderer.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <memory>
#include <optional>
#include <string>
#include <vector>

#include <glm/glm.hpp>

namespace hive::metal {

struct CameraUniforms;
struct FrameContext;
struct MetalMapFrame;
struct MetalModelFrame;
struct MetalTextureFallbackStats;
struct ModelCpuFrame;

class IModelFrameRenderer {
  public:
	virtual ~IModelFrameRenderer() = default;

	virtual void invalidate_resources() = 0;

	[[nodiscard]]
	virtual bool render_models(const FrameContext& frame_context, const MetalModelFrame& frame) = 0;

	[[nodiscard]]
	virtual std::optional<std::uint32_t> pick_model_id(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		glm::vec2 mouse_position
	) = 0;

	[[nodiscard]]
	virtual bool capture_model_picking_frame(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		std::vector<std::uint8_t>& bgra_pixels,
		int& width,
		int& height
	) = 0;

	[[nodiscard]]
	virtual const std::string& last_error() const = 0;

	[[nodiscard]]
	virtual const MetalTextureFallbackStats& last_texture_fallback_stats() const = 0;
};

class IMapFrameRenderer : public IModelFrameRenderer {
  public:
	~IMapFrameRenderer() override = default;

	[[nodiscard]]
	virtual bool render(const FrameContext& frame_context, const MetalMapFrame& frame) = 0;
};

[[nodiscard]]
std::unique_ptr<IModelFrameRenderer> make_model_frame_renderer();

[[nodiscard]]
std::unique_ptr<IMapFrameRenderer> make_map_frame_renderer();

} // namespace hive::metal
