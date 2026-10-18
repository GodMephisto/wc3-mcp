---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/file_dialogs.h"
ref: HEAD
---

#pragma once

#include <string>
#include <vector>

#include <Windows.h>

/**
 * @brief  Show a native file-open dialog and return the selected paths.
 * @param  hwnd        Owner window handle; may be nullptr.
 * @param  filter      Shell filter spec string, e.g. L"*.blp;*.png".
 * @param  multiSelect Allow selecting multiple files when true.
 * @return Absolute paths of the selected files; empty on cancel.
 */
std::vector<std::wstring> open_file_dialog(HWND hwnd, const wchar_t* filter, bool multiSelect = false);

/**
 * @brief  Show a native folder-picker dialog and return the selected path.
 * @param  hwnd  Owner window handle; may be nullptr.
 * @return Absolute path of the selected folder; empty on cancel.
 */
std::wstring open_folder_dialog(HWND hwnd);
