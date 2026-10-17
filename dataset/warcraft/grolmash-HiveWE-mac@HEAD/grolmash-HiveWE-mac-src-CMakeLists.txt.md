---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/CMakeLists.txt"
ref: HEAD
---

target_sources(HiveWE_lib PUBLIC FILE_SET yeet TYPE CXX_MODULES FILES
	"types.ixx"
	"map_global.ixx"
	"globals.ixx"
	"rendering/render_backend.ixx"
	"rendering/map_renderer.ixx"

	"base/map/map.ixx"
	"base/binary_reader.ixx"
	"base/doodad.ixx"
	"base/doodads.ixx"
	"base/binary_writer.ixx"
	"base/hierarchy.ixx"
	"base/render_manager.ixx"
	"base/game_cameras.ixx"
	"base/gameplay_constants.ixx"
	"base/imports.ixx"
	"base/map_info.ixx"
	"base/pathing_map.ixx"
	"base/regions.ixx"
	"base/camera.ixx"
	"base/world_undo_manager.ixx"
	"base/window_handler.ixx"
	"base/resource_manager.ixx"
	"base/shadow_map.ixx"
	"base/sounds.ixx"
	"base/trigger_strings.ixx"
	"base/physics.ixx"
	"base/terrain_undo.ixx"
	"base/units_undo.ixx"
	"base/pathing_undo.ixx"
	"base/doodads_undo.ixx"
	"base/units.ixx"
	"base/terrain.ixx"

	"base/triggers/triggers.ixx"

	"file_formats/blp.ixx"
	"file_formats/casc.ixx"
	"file_formats/ini.ixx"
	"file_formats/json.ixx"
	"file_formats/mdx/mdx.ixx"

	"file_formats/mpq.ixx"
	"file_formats/slk.ixx"

	"resources/cliff_mesh.ixx"
	"resources/gpu_texture.ixx"
	"resources/ground_texture.ixx"

	"resources/texture.ixx"
	"resources/pathing_texture.ixx"
	"resources/qicon_resource.ixx"
	"resources/editable_mesh.ixx"
	"resources/particle_emitter2_simulation.ixx"
	"resources/skinned_mesh/render_node.ixx"
	"resources/skinned_mesh/skeletal_model_instance.ixx"
	"resources/skinned_mesh.ixx"

	"models/tree/ability_tree_model.ixx"
	"models/tree/base_tree_model.ixx"
	"models/tree/buff_tree_model.ixx"
	"models/tree/destructible_tree_model.ixx"
	"models/tree/doodad_tree_model.ixx"
	"models/tree/item_tree_model.ixx"
	"models/tree/unit_tree_model.ixx"
	"models/tree/upgrade_tree_model.ixx"

	"models/list/ability_list_model.ixx"
	"models/list/base_list_model.ixx"
	"models/list/destructible_list_model.ixx"
	"models/list/doodad_list_model.ixx"
	"models/list/item_list_model.ixx"
	"models/list/unit_list_model.ixx"
	"models/list/upgrade_list_model.ixx"
	"models/list/buff_list_model.ixx"

	"models/table_model.ixx"

	"object_editor/slk_conversions.ixx"

	"custom_widgets/aspect_ratio_pixmap_label.ixx"
	"custom_widgets/color_button.ixx"

	"custom_widgets/qribbon.ixx"
	"custom_widgets/unit_selector.ixx"
	"custom_widgets/flow_layout.ixx"

	"utilities/timer.ixx"
	"utilities/task_pool.ixx"
	"utilities/image_decoding.ixx"
	"utilities/utilities.ixx"
	"utilities/icon_utilities.ixx"
	"utilities/modification_tables.ixx" 
	"utilities/unordered_map.ixx"

	"utilities/no_init_allocator.ixx"
	"utilities/math_operations.ixx"
	"utilities/rects.ixx"
	
	"test.ixx"
	"object_editor/ability_list_editor.ixx"
)

if(HIVEWE_BUILD_OPENGL_RENDERER)
	target_sources(HiveWE_lib PUBLIC FILE_SET yeet TYPE CXX_MODULES FILES
		"rendering/opengl_render_backend.ixx"
		"rendering/opengl_texture_resources.ixx"
		"rendering/opengl_editable_mesh_resources.ixx"
		"rendering/opengl_shader.ixx"
		"rendering/opengl_shape_resources.ixx"
		"rendering/opengl_cliff_mesh_resources.ixx"
		"rendering/opengl_terrain_buffer_resources.ixx"
		"rendering/opengl_terrain_overlay_resources.ixx"
		"rendering/opengl_terrain_texture_array_resources.ixx"
		"rendering/opengl_terrain_renderer.ixx"
		"rendering/opengl_selection_overlay_renderer.ixx"
		"rendering/opengl_map_renderer.ixx"
		"rendering/map_renderer_factory.ixx"
		"rendering/opengl_particle_emitter2_renderer.ixx"
		"rendering/opengl_skinned_mesh_resources.ixx"
	)
endif()

target_sources(HiveWE PRIVATE
	"main.cpp"
	"smoke/smoke_render.cpp"
	"smoke/smoke_save_reload.cpp"
	"main_window/hivewe.rc"
)

target_sources(HiveWE_lib PRIVATE
	"main_window/hivewe.cpp"
	"main_window/map_canvas_widget.cpp"
	"main_window/playtest_launcher.cpp"
	"main_window/playtest_launcher.h"
	"main_window/main_ribbon.cpp"

	"base/map/resize.cpp"
	"base/map/unused_files.cpp"
	"base/triggers/gui.cpp"
	"base/triggers/map_script.cpp"

	"brush/brush.cpp"
	"brush/doodad_brush.cpp"
	"brush/pathing_brush.cpp"
	"brush/terrain_brush.cpp"
	"brush/terrain_operators.cpp"
	"brush/unit_brush.cpp"
	
	"file_formats/mdx/mdl_reader.cpp"
	"file_formats/mdx/mdl_writer.cpp"
	"file_formats/mdx/mdx_reader.cpp"
	"file_formats/mdx/mdx_writer.cpp"
	"file_formats/mdx/optimizer.cpp"
	"file_formats/mdx/utilities.cpp"
	"file_formats/mdx/validator.cpp"


	"menus/doodad_palette.cpp"
	"menus/map_info_editor.cpp"
	"menus/minimap.cpp"
	"menus/palette.cpp"
	"menus/pathing_palette.cpp"
	"menus/settings_editor.cpp"
	"menus/terrain_palette.cpp"
	"menus/unit_palette.cpp"
	"menus/tile_pather.cpp"
	"menus/tile_picker.cpp"
	"menus/tile_setter.cpp"
	"menus/gameplay_constants_editor.cpp"
	"menus/gameplay_constants_editor.h"

	"asset_manager/asset_manager.cpp"
	"asset_manager/asset_manager.h"

	"models/single_model.cpp"
	
	"custom_widgets/aspect_ratio_pixmap_label.h"
	"custom_widgets/color_button.h"

	"trigger_editor/trigger_editor.cpp"
	"trigger_editor/search_window.cpp"
	"trigger_editor/jass_editor.cpp"
	"trigger_editor/jass_tokenizer.cpp"
	"trigger_editor/trigger_explorer.cpp"
	"trigger_editor/variable_editor.cpp"
	"trigger_editor/trigger_model.cpp"

	"model_editor/model_editor_camera.cpp"
	"model_editor/model_editor.cpp"

	"object_editor/icon_view.cpp"
	"object_editor/model_view.cpp"
	"object_editor/object_editor.cpp"
	"object_editor/global_search.cpp"

	
 )

if(HIVEWE_BUILD_OPENGL_RENDERER)
	target_sources(HiveWE_lib PRIVATE
		"model_editor/model_editor_glwidget.cpp"
		"object_editor/model_grid_glwidget.cpp"
		"rendering/opengl_model_preview_renderer.h"
		"qt_imgui/imgui_renderer.cpp"
		"qt_imgui/qt_imgui.cpp"
	)
endif()

if(APPLE AND HIVEWE_ENABLE_METAL)
	target_sources(HiveWE_lib PRIVATE
		"model_editor/model_editor_metal_widget.h"
		"model_editor/model_editor_metal_widget.cpp"
		"object_editor/model_grid_metal_widget.h"
		"object_editor/model_grid_metal_widget.cpp"
		"rendering/metal_map_renderer.h"
		"rendering/metal_map_renderer.mm"
		"rendering/metal_map_frame_renderer.h"
		"rendering/metal_map_frame_renderer.cpp"
		"rendering/metal_model_frame_stats.h"
		"rendering/metal_particle_emitter1_model_frame.h"
		"rendering/metal_surface.h"
		"rendering/metal_surface.mm"
	)

	set_source_files_properties("rendering/metal_map_renderer.mm" PROPERTIES
		COMPILE_OPTIONS "-fobjc-arc"
	)
	set_source_files_properties("rendering/metal_surface.mm" PROPERTIES
		COMPILE_OPTIONS "-fobjc-arc"
	)

	target_compile_options(HiveWE_lib PRIVATE
		$<$<COMPILE_LANGUAGE:OBJCXX>:-fobjc-arc>
	)
endif()
