---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/settings.h"
ref: HEAD
---

#pragma once

#include <string>

constexpr int OUTPUT_FORMAT_ORIGINAL = 5;
constexpr int OUTPUT_FORMAT_DDS      = 6;
constexpr int OUTPUT_FORMAT_MAX      = OUTPUT_FORMAT_DDS;
constexpr int DEFAULT_IMAGE_QUALITY  = 90;

/// Application settings persisted to an INI file in the user's roaming AppData.
struct AppSettings {
    int   windowX          = 100;       ///< Left edge of the main window, in screen pixels.
    int   windowY          = 100;       ///< Top edge of the main window, in screen pixels.
    int   windowW          = 1200;      ///< Width of the main window, in screen pixels.
    int   windowH          = 760;       ///< Height of the main window, in screen pixels.
    float splitterPos      = 340.0f;    ///< Left-panel width at the splitter divider, in pixels.

    int  outputFormat      = 0;         ///< 0=BLP 1=PNG 2=JPG 3=BMP 4=TGA 5=original 6=DDS.
    int  quality           = DEFAULT_IMAGE_QUALITY; ///< JPEG/BLP quality, 1-100.
    bool overwrite         = false;     ///< Overwrite existing output files when true.
    bool recursive         = true;      ///< Process subdirectories recursively when true.

    int  batchSizeMode     = 0;         ///< Resize mode: 0=original 1=64 2=128 3=custom.
    int  batchWidth        = 256;       ///< Custom batch output width in pixels.
    int  batchHeight       = 256;       ///< Custom batch output height in pixels.
    bool batchLockAspect   = true;      ///< Preserve aspect ratio during batch resize when true.
    int  batchResizeMethod = 1;         ///< Resize algorithm: 0=stretch 1=fit-with-padding.

    std::string lastInputDir;           ///< Most-recently used input directory (UTF-8).
    std::string lastOutputDir;          ///< Most-recently used output directory (UTF-8).

    /**
     * @brief  Loads settings from the INI file; silently no-ops if the file is missing.
     */
    void load();

    /**
     * @brief  Saves the current settings to the INI file, creating directories as needed.
     */
    void save() const;

    /**
     * @brief  Returns the absolute path to the settings INI file.
     * @return UTF-8 path inside %APPDATA%\blp_viewer\; falls back to "settings.ini" on error.
     */
    static std::string settings_path();
};
