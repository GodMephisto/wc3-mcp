---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/model_preview_renderer.h"
ref: HEAD
---

#pragma once

#include <glm/glm.hpp>

import EditableMesh;
import SkeletalModelInstance;

struct ModelPreviewRenderView {
	glm::mat4 projection_view;
	glm::vec3 camera_right;
	glm::vec3 camera_up;
	glm::vec3 camera_forward;
};

class IModelPreviewRenderer {
  public:
	virtual ~IModelPreviewRenderer() = default;

	virtual void initialize() = 0;
	virtual void render_model(
		const EditableMesh& mesh,
		const SkeletalModelInstance& skeleton,
		const ModelPreviewRenderView& view,
		int team_color_index
	) = 0;
};
