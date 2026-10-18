---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_surface.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <memory>
#include <vector>

class MetalSurface final {
public:
	class Frame final {
	public:
		Frame();
		~Frame();

		Frame(Frame&&) noexcept;
		Frame& operator=(Frame&&) noexcept;

		Frame(const Frame&) = delete;
		Frame& operator=(const Frame&) = delete;

		[[nodiscard]] explicit operator bool() const;
		[[nodiscard]] int drawable_width() const;
		[[nodiscard]] int drawable_height() const;
		[[nodiscard]] double scale() const;

		// Opaque Objective-C objects for Objective-C++ renderers:
		// id<MTLDevice>, id<MTLCommandQueue>, id<MTLCommandBuffer>,
		// id<MTLTexture>, and MTLRenderPassDescriptor* respectively.
		[[nodiscard]] void* device_opaque() const;
		[[nodiscard]] void* command_queue_opaque() const;
		[[nodiscard]] void* command_buffer_opaque() const;
		[[nodiscard]] void* drawable_color_texture_opaque() const;
		[[nodiscard]] void* depth_texture_opaque() const;
		[[nodiscard]] void* render_pass_descriptor_opaque() const;

	private:
		friend class MetalSurface;

		struct Impl;
		explicit Frame(std::unique_ptr<Impl> impl);

		std::unique_ptr<Impl> impl_;
	};

	MetalSurface();
	~MetalSurface();

	MetalSurface(const MetalSurface&) = delete;
	MetalSurface& operator=(const MetalSurface&) = delete;

	[[nodiscard]] bool initialize(void* nativeView);
	void resize(int width, int height, double scale);
	[[nodiscard]] void* device_opaque() const;
	[[nodiscard]] void* command_queue_opaque() const;
	[[nodiscard]] int drawable_width() const;
	[[nodiscard]] int drawable_height() const;
	[[nodiscard]] double scale() const;
	[[nodiscard]] Frame begin_frame(float clear_r, float clear_g, float clear_b, float clear_a);
	[[nodiscard]] bool end_frame(Frame&& frame);
	[[nodiscard]] bool end_frame(Frame&& frame, std::vector<std::uint8_t>* bgra8_pixels);
	[[nodiscard]] bool draw_clear(float r, float g, float b, float a);

private:
	struct Impl;
	std::unique_ptr<Impl> impl_;
};
