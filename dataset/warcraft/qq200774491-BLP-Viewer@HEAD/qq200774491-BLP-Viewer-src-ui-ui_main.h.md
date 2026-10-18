---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/ui/ui_main.h"
ref: HEAD
---

#pragma once

#include <string>
#include <vector>
#include <set>
#include <unordered_map>
#include <atomic>
#include <cstdint>

#include "imgui.h"
#include "blp_api.h"
#include "image_io.h"
#include "image_view.h"
#include "settings.h"
#include "thumbnail_cache.h"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <Windows.h>

struct BlpMipEntry {
    int      index  = 0;  ///< Zero-based mip level index.
    uint32_t offset = 0;  ///< Byte offset of the mip data within the BLP file.
    uint32_t size   = 0;  ///< Byte size of the mip data segment.
};

struct ConvertItemResult {
    std::string inputPath;   ///< Source file path (UTF-8).
    std::string outputPath;  ///< Destination file path (UTF-8).
    bool        success = false;
    std::string error;       ///< Non-empty when success is false.
};

/// Central application state shared across all UI panels.
struct AppState {
    HWND                hwnd          = nullptr; ///< Main window handle.
    ID3D11Device*       device        = nullptr; ///< Non-owning; owned by DX11State.
    ID3D11DeviceContext* deviceContext = nullptr; ///< Non-owning; owned by DX11State.

    AppSettings settings;

    float leftPanelWidth = 320.0f; ///< Current left-panel splitter width in pixels.

    std::vector<std::string>                   fileList;
    std::set<std::string>                      fileSet;
    std::unordered_map<std::string, std::string> relativePathMap;
    int selectedFileIndex = -1;

    int  outputFormat = 0;  ///< 0=BLP 1=PNG 2=JPG 3=BMP 4=TGA 5=original 6=DDS
    int  quality      = DEFAULT_IMAGE_QUALITY;
    bool overwrite    = false;
    bool recursive    = true;
    char inputDirBuf[1024]  = {};
    char outputDirBuf[1024] = {};

    std::atomic<float> convertProgress{0.0f};
    std::atomic<bool>  converting{false};
    std::vector<std::string>       logMessages;
    std::vector<ConvertItemResult> lastConvertResults;
    int  lastConvertTotal   = 0;
    int  lastConvertSuccess = 0;
    int  lastConvertFailed  = 0;
    bool showAboutPopup     = false;

    ImageViewer          imageViewer;
    std::string          currentPreviewPath;
    std::vector<uint8_t> currentBlpBytes;
    std::vector<uint8_t> originalFileBytes;
    ImageMeta            currentMeta;
    ImageMeta            originalMeta;
    bool currentIsBlp    = false;
    int  currentMipIndex = 0;
    bool hasOriginalBackup = false;
    bool previewAdjusted   = false;
    bool previewUseAlpha   = false;

    std::string          convertedFromPath;
    std::vector<uint8_t> convertedFromBytes;
    std::string          convertedToPath;

    std::vector<uint8_t> previewOriginalRGBA;
    std::vector<uint8_t> previewAdjustedRGBA;
    int previewOrigW = 0;
    int previewOrigH = 0;
    int previewAdjW  = 0;
    int previewAdjH  = 0;

    std::vector<BlpMipEntry> mipEntries;
    int selectedMipIndex = 0;

    int   resizeWidth        = 0;
    int   resizeHeight       = 0;
    bool  resizeLockAspect   = true;
    bool  resizeAspectSyncing = false;
    float previewCanvasBg[3] = {0.878f, 0.894f, 0.922f};

    int targetSizeMode = 0; ///< Preview-toolbar save size: 0=resizeW/H, 1=64, 2=128.

    int   batchSizeMode    = 0;    ///< 0=original 1=64×64 2=128×128 3=custom
    int   batchWidth       = 256;
    int   batchHeight      = 256;
    bool  batchLockAspect  = true;
    float batchAspect      = 1.0f; ///< W/H ratio captured when lock-aspect turns on.
    int   batchResizeMethod = 1;   ///< 0=Stretch 1=CenterTransparent

    int            rightViewMode = 1; ///< 0=thumbnail-grid 1=preview
    ThumbnailCache thumbCache;

    std::string overlayImagePath;             ///< Overlay image UTF-8 path.
    int         overlayAnchor     = 4;        ///< Anchor9 index (4 = Center).
    int         overlayMarginPx   = 0;
    int         overlayScalePct   = 100;
    float       overlayOpacity    = 1.0f;
    int         borderThicknessPx = 4;
    float       borderColor[4]    = {0.0f, 0.0f, 0.0f, 1.0f}; ///< RGBA.
    bool        borderExpandCanvas = false;
    int         composeOpMode      = 0; ///< 0=layer-composite 1=border

    BlpApi blpApi;
    int    zoomPercent = 100;

    bool blpAssociated       = false;
    bool pngAssociated       = false;
    bool tgaAssociated       = false;
    bool ddsAssociated       = false;
    bool thumbnailRegistered = false;

    float dpiScale = 1.0f;
};

/**
 * @brief  Render the full application UI into the current ImGui frame.
 * @param  state  Mutable application state shared across all panels.
 */
void render_main_ui(AppState& state);

/**
 * @brief  Render the application menu bar (File, Edit, View, Tools, Help).
 * @param  state  Mutable application state.
 */
void render_menu_bar(AppState& state);

/**
 * @brief  Render the status bar at the bottom of the main window.
 * @param  state  Mutable application state.
 */
void render_status_bar(AppState& state);
