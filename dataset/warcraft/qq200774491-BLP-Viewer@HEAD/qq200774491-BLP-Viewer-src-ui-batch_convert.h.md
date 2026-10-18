---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/batch_convert.h"
ref: HEAD
---

#pragma once

#include <string>
#include <vector>

struct AppState;
struct RgbaImage;

// File scanning helpers shared with the resource-list tab.
/**
 * @brief  Add a list of file or directory paths to the resource list.
 * @param  state  Application state; fileList, fileSet and selectedFileIndex are updated.
 * @param  paths  Candidate UTF-8 paths; directories are expanded via add_folder_files.
 */
void add_files(AppState& state, const std::vector<std::string>& paths);

/**
 * @brief  Recursively (or not) scan a folder and add supported files to the resource list.
 * @param  state      Application state.
 * @param  folder     UTF-8 path to the root folder.
 * @param  recursive  When true, walk subdirectories.
 */
void add_folder_files(AppState& state, const std::string& folder, bool recursive);

// Batch output helpers shared with the compose tab.
/**
 * @brief  Convert a format-index to the canonical lowercase extension string.
 * @param  formatIndex  0=blp 1=png 2=jpg 3=bmp 4=tga 6=dds. Original-format mode
 *                      is resolved per input file and must not be passed here.
 * @return Lowercase format string, e.g. "blp".
 */
std::string normalized_format_str(int formatIndex);

/**
 * @brief  Build the output file path for one input file.
 * @param  state      Application state (outputDirBuf, relativePathMap).
 * @param  inputPath  Absolute UTF-8 source path.
 * @param  format     Target format extension, e.g. "blp".
 * @param  overwrite  When false, a numeric suffix is added if the path exists.
 * @return Absolute UTF-8 output path.
 */
std::string build_output_path(const AppState& state, const std::string& inputPath,
                               const std::string& format, bool overwrite);

/**
 * @brief  Ensure an output directory is set; fills outputDirBuf from the source dir on single-file jobs.
 * @param  state       Application state.
 * @param  inputPaths  List of source paths for this batch.
 * @return true when an output directory is available (or was resolved).
 */
bool ensure_output_dir(AppState& state, const std::vector<std::string>& inputPaths);

/**
 * @brief  Resize image in-place to the batch target size if one is selected.
 * @param  state  Application state (batchSizeMode, batchWidth/Height, batchResizeMethod).
 * @param  image  Image to resize; left unchanged when batchSizeMode == 0.
 */
void apply_batch_output_size(const AppState& state, RgbaImage& image);

// Shared UI widgets (also used by the compose tab).
/**
 * @brief  Render the shared "输出格式" button group, updating state.outputFormat.
 * @param  state  Application state.
 */
void draw_output_format_selector(AppState& state);

/**
 * @brief  Render the shared "输出尺寸" controls, updating batch size state.
 * @param  state  Application state.
 */
void draw_output_size_selector(AppState& state);

// Batch-convert tab UI.
/**
 * @brief  Render the "批量转换" tab body inside the left-panel TabBar.
 * @param  state  Application state.
 */
void render_batch_convert_tab(AppState& state);

// Execution entry points.
/**
 * @brief  Run batch conversion of all files currently in the resource list.
 * @param  state  Application state.
 */
void run_convert_all_from_ui(AppState& state);

/**
 * @brief  Run batch conversion for a specific set of input paths.
 * @param  state       Application state.
 * @param  inputPaths  Source files to convert.
 */
void run_convert_for_paths(AppState& state, const std::vector<std::string>& inputPaths);
