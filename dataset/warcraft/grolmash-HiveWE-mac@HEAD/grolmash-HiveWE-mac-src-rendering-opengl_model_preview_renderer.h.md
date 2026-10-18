---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/opengl_model_preview_renderer.h"
ref: HEAD
---

#pragma once

#include "rendering/model_preview_renderer.h"

#include <glad/glad.h>

#include <cstdint>
#include <memory>
#include <type_traits>

import MDX;
import OpenGLEditableMeshResources;
import OpenGLTextureResources;
import OpenGLParticleEmitter2Renderer;
import ResourceManager;
import OpenGLShader;

class OpenGLModelPreviewRenderer final : public IModelPreviewRenderer {
  public:
	void initialize() override {
		shader_sd = resource_manager.load<OpenGLShader>({ "data/shaders/editable_mesh_sd.vert", "data/shaders/editable_mesh_sd.frag" }).value();
		shader_hd = resource_manager.load<OpenGLShader>({ "data/shaders/editable_mesh_hd.vert", "data/shaders/editable_mesh_hd.frag" }).value();
		particle_renderer.init();
	}

	void render_model(
		const EditableMesh& mesh,
		const SkeletalModelInstance& skeleton,
		const ModelPreviewRenderView& view,
		const int team_color_index
	) override {
		glEnable(GL_BLEND);
		glDepthMask(true);
		glEnable(GL_DEPTH_TEST);

		shader_sd->use();
		render_pass(mesh, true, false, team_color_index, skeleton, view.projection_view, view.camera_forward);
		shader_hd->use();
		render_pass(mesh, true, true, team_color_index, skeleton, view.projection_view, view.camera_forward);

		glEnable(GL_BLEND);
		glDepthMask(false);

		shader_sd->use();
		render_pass(mesh, false, false, team_color_index, skeleton, view.projection_view, view.camera_forward);
		shader_hd->use();
		render_pass(mesh, false, true, team_color_index, skeleton, view.projection_view, view.camera_forward);

		glDepthMask(true);

		particle_renderer.render(*mesh.mdx, mesh.textures, skeleton, view.projection_view, view.camera_right, view.camera_up, view.camera_forward);
	}

  private:
	std::shared_ptr<OpenGLShader> shader_sd;
	std::shared_ptr<OpenGLShader> shader_hd;
	OpenGLEditableMeshResources mesh_resources;
	OpenGLTextureResources texture_resources;
	OpenGLParticleEmitter2Renderer particle_renderer;

	template <typename GeosetT>
	auto layers_for_geoset(const EditableMesh& mesh, const GeosetT& geoset) const
		-> const std::remove_reference_t<decltype(mesh.mdx->materials[0].layers)>* {
		if (!mesh.mdx) {
			return nullptr;
		}

		if (geoset.material_id < 0) {
			return nullptr;
		}

		const std::size_t material_index = static_cast<std::size_t>(geoset.material_id);
		if (material_index >= mesh.mdx->materials.size()) {
			return nullptr;
		}

		const auto& layers = mesh.mdx->materials[material_index].layers;
		return layers.empty() ? nullptr : &layers;
	}

	void render_pass(
		const EditableMesh& mesh,
		const bool opaque_pass,
		const bool render_hd,
		const int team_color_index,
		const SkeletalModelInstance& skeleton,
		const glm::mat4& projection_view,
		const glm::vec3 light_direction
	) {
		if (mesh.mdx->geosets.empty()) {
			return;
		}

		const auto& resources = mesh_resources.buffers_for(mesh);
		glBindVertexArray(resources.vao);

		glUniformMatrix4fv(0, 1, false, &projection_view[0][0]);
		glUniform3fv(3, 1, &light_direction.x);
		glUniform1i(9, team_color_index);
		glUniformMatrix4fv(11, mesh.mdx->bones.size(), false, &skeleton.world_matrices[0][0][0]);

		for (const auto& geoset_entry : mesh.geosets) {
			const auto* layers = layers_for_geoset(mesh, geoset_entry);
			if (layers == nullptr) {
				continue;
			}

			const bool geoset_is_opaque = ((*layers)[0].blend_mode == 0 || (*layers)[0].blend_mode == 1);
			if (geoset_is_opaque != opaque_pass) {
				continue;
			}

			glm::vec3 geoset_color(1.f);
			float geoset_anim_visibility = 1.0f;
			if (geoset_entry.geoset_anim && skeleton.sequence_index >= 0) {
				geoset_color = skeleton.get_geoset_animation_color(*geoset_entry.geoset_anim);
				geoset_anim_visibility = skeleton.get_geoset_animation_visiblity(*geoset_entry.geoset_anim);
			}

			for (const auto& layer : *layers) {
				if ((layer.shader == mdx::ShaderType::HD) != render_hd) {
					continue;
				}

				float layer_visibility = 1.0f;
				if (skeleton.sequence_index >= 0) {
					layer_visibility = skeleton.get_layer_visiblity(layer);
				}

				const float final_alpha = layer_visibility * geoset_anim_visibility;
				if (!opaque_pass && final_alpha <= 0.01f) {
					continue;
				}

				const glm::vec4 layer_color = glm::vec4(geoset_color, final_alpha);

				glUniform1f(1, layer.blend_mode == 1 ? 0.75f : 0.01f);
				glUniform1i(2, !(layer.shading_flags & 0x1));
				glUniform4fv(8, 1, &layer_color[0]);

				const uint32_t fallback_tex_id = layer.textures.empty() ? 0u : layer.textures.front().id;
				const uint32_t replaceable = fallback_tex_id < mesh.mdx->textures.size()
					? mesh.mdx->textures[fallback_tex_id].replaceable_id
					: 0u;
				glUniform1i(10, replaceable == 1 || replaceable == 2);

				switch (layer.blend_mode) {
					case 0:
					case 1:
						glBlendFunc(GL_ONE, GL_ZERO);
						break;
					case 2:
						glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
						break;
					case 3:
					case 4:
						glBlendFunc(GL_SRC_ALPHA, GL_ONE);
						break;
					case 5:
						glBlendFunc(GL_ZERO, GL_SRC_COLOR);
						break;
					case 6:
						glBlendFunc(GL_DST_COLOR, GL_SRC_COLOR);
						break;
				}

				if (layer.shading_flags & 0x10) {
					glDisable(GL_CULL_FACE);
				} else {
					glEnable(GL_CULL_FACE);
				}
				if (layer.shading_flags & 0x40) {
					glDisable(GL_DEPTH_TEST);
				} else {
					glEnable(GL_DEPTH_TEST);
				}
				if (opaque_pass) {
					glDepthMask(!(layer.shading_flags & 0x80));
				}

				// SD path: albedo only. HD path: albedo/normal/orm/emissive/team-color.
				const std::size_t slot_count = render_hd ? 5 : 1;
				for (std::size_t slot = 0; slot < slot_count; slot++) {
					const uint32_t tex_id = slot < layer.textures.size() ? layer.textures[slot].id : fallback_tex_id;
					if (tex_id < mesh.textures.size() && mesh.textures[tex_id]) {
						glBindTextureUnit(static_cast<GLuint>(slot), texture_resources.texture_for(*mesh.textures[tex_id]));
					} else {
						glBindTextureUnit(static_cast<GLuint>(slot), 0);
					}
				}

				glDrawElementsBaseVertex(
					GL_TRIANGLES,
					geoset_entry.indices,
					GL_UNSIGNED_SHORT,
					reinterpret_cast<void*>(static_cast<uintptr_t>(geoset_entry.base_index) * sizeof(uint16_t)),
					geoset_entry.base_vertex
				);
			}
		}
	}
};
