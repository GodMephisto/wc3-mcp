---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/brush/pathing_brush.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <vector>

#include "brush.h"
import Rects;

class PathingBrush: public Brush {
  public:
	enum class Operation {
		replace,
		add,
		remove
	};

	uint8_t brush_mask = 0b00000000;

	Operation operation = Operation::replace;

	PathingRect applied_area;

	PathingBrush();

	glm::ivec2 get_size() const override;
	void set_size(glm::ivec2 size) override;
	void increase_size(int size) override;
	void decrease_size(int size) override;

	void apply_begin() override;
	void apply(double frame_delta) override;
	void apply_end() override;

	void add_pathing_undo(const PathingRect& area);

  private:
	std::vector<uint8_t> old_pathing_cells_static;
};
