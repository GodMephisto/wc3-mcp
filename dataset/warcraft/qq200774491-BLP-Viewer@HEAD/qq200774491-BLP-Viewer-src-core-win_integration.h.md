---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/win_integration.h"
ref: HEAD
---

#pragma once

#include <string>

#include <Windows.h>

/**
 * @brief  Returns the absolute path of the running executable.
 * @return Native wide-character path; empty if GetModuleFileNameW fails.
 */
std::wstring get_app_path();

/**
 * @brief  Returns the directory that contains the running executable.
 * @return Parent directory of get_app_path(); empty if the path is at the root.
 */
std::wstring get_app_dir();

/** @brief Returns true when the HKCU shell association for .blp points to this viewer. */
bool is_blp_associated();

/**
 * @brief  Write HKCU registry entries to associate .blp with this viewer.
 * @param  appPath   Absolute path to the viewer executable.
 * @param  outError  Optional; receives a Chinese error description on failure.
 * @return true on success; false if any registry write fails.
 */
bool register_blp_association(const std::wstring& appPath, std::string* outError);

/** @brief Returns true when the HKCU shell association for .png points to this viewer. */
bool is_png_associated();

/**
 * @brief  Write HKCU registry entries to associate .png with this viewer.
 * @param  appPath   Absolute path to the viewer executable.
 * @param  outError  Optional; receives a Chinese error description on failure.
 * @return true on success; false if any registry write fails.
 */
bool register_png_association(const std::wstring& appPath, std::string* outError);

/** @brief Returns true when the HKCU shell association for .tga points to this viewer. */
bool is_tga_associated();

/**
 * @brief  Write HKCU registry entries to associate .tga with this viewer.
 * @param  appPath   Absolute path to the viewer executable.
 * @param  outError  Optional; receives a Chinese error description on failure.
 * @return true on success; false if any registry write fails.
 */
bool register_tga_association(const std::wstring& appPath, std::string* outError);

/** @brief Returns true when the HKCU shell association for .dds points to this viewer. */
bool is_dds_associated();

/** @brief Associate .dds files with this viewer for the current user. */
bool register_dds_association(const std::wstring& appPath, std::string* outError);

/** @brief Returns true when the BLP shell thumbnail handler CLSID is registered in HKCU. */
bool is_thumbnail_registered();

/**
 * @brief  Load a DLL and call one of its exported entry points.
 * @param  dllPath   Absolute path to the DLL.
 * @param  entry     Exported function name (e.g. "DllRegisterServer").
 * @param  outError  Optional; receives a Chinese error description on failure.
 * @return true when the entry point is found and returns S_OK.
 */
bool call_dll_entry(const std::wstring& dllPath, const char* entry, std::string* outError);
