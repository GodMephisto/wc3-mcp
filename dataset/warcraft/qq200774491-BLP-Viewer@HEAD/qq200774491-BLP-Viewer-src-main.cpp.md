---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/main.cpp"
ref: HEAD
---

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <Windows.h>
#include <shellapi.h>
#include <d3d11.h>

#include "imgui.h"
#include "imgui_impl_win32.h"
#include "imgui_impl_dx11.h"

#include "app.h"
#include "style.h"
#include "ui_main.h"
#include "batch_convert.h"
#include "win_integration.h"
#include "utils.h"

#include <algorithm>
#include <filesystem>
#include <string>
#include <vector>

extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

static AppState  g_state;
static DX11State g_dx;

static void sanitize_window_rect(AppSettings& settings) {
    constexpr int kDefaultX = 100;
    constexpr int kDefaultY = 100;
    constexpr int kDefaultW = 1200;
    constexpr int kDefaultH = 760;
    constexpr int kMinW     = 640;
    constexpr int kMinH     = 420;

    if (settings.windowW <= 0 || settings.windowH <= 0 ||
        settings.windowW > 10000 || settings.windowH > 10000) {
        settings.windowW = kDefaultW;
        settings.windowH = kDefaultH;
    }
    if (settings.windowW < kMinW) settings.windowW = kMinW;
    if (settings.windowH < kMinH) settings.windowH = kMinH;

    auto centerOnPrimaryMonitor = [&]() {
        POINT   origin  = {0, 0};
        HMONITOR primary = MonitorFromPoint(origin, MONITOR_DEFAULTTOPRIMARY);
        MONITORINFO mi  = {};
        mi.cbSize        = sizeof(mi);
        if (!GetMonitorInfoW(primary, &mi)) {
            settings.windowX = kDefaultX;
            settings.windowY = kDefaultY;
            settings.windowW = kDefaultW;
            settings.windowH = kDefaultH;
            return;
        }
        const int workW = mi.rcWork.right - mi.rcWork.left;
        const int workH = mi.rcWork.bottom - mi.rcWork.top;
        if (settings.windowW > workW) settings.windowW = workW;
        if (settings.windowH > workH) settings.windowH = workH;
        settings.windowX = mi.rcWork.left + (workW - settings.windowW) / 2;
        settings.windowY = mi.rcWork.top  + (workH - settings.windowH) / 2;
    };

    // -32000 is a common minimized-window coordinate on Windows.
    constexpr int MINIMIZED_WINDOW_COORD = -30000;
    if (settings.windowX <= MINIMIZED_WINDOW_COORD || settings.windowY <= MINIMIZED_WINDOW_COORD) {
        centerOnPrimaryMonitor();
        return;
    }

    RECT windowRect = {
        settings.windowX,
        settings.windowY,
        settings.windowX + settings.windowW,
        settings.windowY + settings.windowH
    };
    HMONITOR monitor = MonitorFromRect(&windowRect, MONITOR_DEFAULTTONULL);
    if (!monitor) {
        centerOnPrimaryMonitor();
        return;
    }

    MONITORINFO mi = {};
    mi.cbSize       = sizeof(mi);
    if (!GetMonitorInfoW(monitor, &mi)) {
        centerOnPrimaryMonitor();
        return;
    }

    const int workW = mi.rcWork.right - mi.rcWork.left;
    const int workH = mi.rcWork.bottom - mi.rcWork.top;
    if (settings.windowW > workW) settings.windowW = workW;
    if (settings.windowH > workH) settings.windowH = workH;
    if (settings.windowX < mi.rcWork.left) settings.windowX = mi.rcWork.left;
    if (settings.windowY < mi.rcWork.top)  settings.windowY = mi.rcWork.top;
    if (settings.windowX + settings.windowW > mi.rcWork.right)
        settings.windowX = mi.rcWork.right - settings.windowW;
    if (settings.windowY + settings.windowH > mi.rcWork.bottom)
        settings.windowY = mi.rcWork.bottom - settings.windowH;
}

static void save_window_rect(AppSettings& settings, HWND hWnd) {
    RECT rect       = {};
    bool validRect  = false;

    WINDOWPLACEMENT placement = {};
    placement.length = sizeof(placement);
    if (GetWindowPlacement(hWnd, &placement)) {
        rect      = placement.rcNormalPosition;
        validRect = (rect.right > rect.left && rect.bottom > rect.top);
    }
    if (!validRect && GetWindowRect(hWnd, &rect)) {
        validRect = (rect.right > rect.left && rect.bottom > rect.top);
    }
    if (!validRect) return;

    settings.windowX = rect.left;
    settings.windowY = rect.top;
    settings.windowW = rect.right  - rect.left;
    settings.windowH = rect.bottom - rect.top;
}

static void handle_drop_files(HDROP hDrop) {
    UINT count = DragQueryFileW(hDrop, 0xFFFFFFFF, nullptr, 0);
    std::vector<std::string> paths;
    for (UINT i = 0; i < count; ++i) {
        wchar_t buf[MAX_PATH] = {};
        DragQueryFileW(hDrop, i, buf, MAX_PATH);
        const std::string path = fs_path_to_utf8(std::filesystem::path(buf));
        if (!path.empty()) paths.push_back(path);
    }
    DragFinish(hDrop);

    if (!paths.empty()) add_files(g_state, paths);
}

static LRESULT WINAPI WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    if (ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam))
        return true;

    switch (msg) {
    case WM_SIZE:
        if (wParam == SIZE_MINIMIZED) return 0;
        if (g_dx.device) {
            cleanup_render_target(&g_dx);
            g_dx.swapChain->ResizeBuffers(0,
                static_cast<UINT>(LOWORD(lParam)),
                static_cast<UINT>(HIWORD(lParam)),
                DXGI_FORMAT_UNKNOWN, 0);
            create_render_target(&g_dx);
        }
        return 0;

    case WM_DROPFILES:
        handle_drop_files(reinterpret_cast<HDROP>(wParam));
        return 0;

    case WM_SYSCOMMAND:
        if ((wParam & 0xfff0) == SC_KEYMENU) return 0;
        break;

    case WM_DPICHANGED: {
        RECT* suggested = reinterpret_cast<RECT*>(lParam);
        SetWindowPos(hWnd, nullptr,
                     suggested->left, suggested->top,
                     suggested->right  - suggested->left,
                     suggested->bottom - suggested->top,
                     SWP_NOZORDER | SWP_NOACTIVATE);
        const float newDpi = static_cast<float>(HIWORD(wParam)) / 96.0f;
        g_state.dpiScale   = newDpi;
        apply_imgui_style(newDpi);
        load_fonts(newDpi);
        ImGui_ImplDX11_InvalidateDeviceObjects();
        return 0;
    }

    case WM_CLOSE: {
        save_window_rect(g_state.settings, hWnd);
        g_state.settings.splitterPos     = g_state.leftPanelWidth;
        g_state.settings.outputFormat    = g_state.outputFormat;
        g_state.settings.quality         = g_state.quality;
        g_state.settings.overwrite       = g_state.overwrite;
        g_state.settings.recursive       = g_state.recursive;
        g_state.settings.batchSizeMode   = g_state.batchSizeMode;
        g_state.settings.batchWidth      = g_state.batchWidth;
        g_state.settings.batchHeight     = g_state.batchHeight;
        g_state.settings.batchLockAspect = g_state.batchLockAspect;
        g_state.settings.batchResizeMethod = g_state.batchResizeMethod;
        g_state.settings.lastInputDir    = g_state.inputDirBuf;
        g_state.settings.lastOutputDir   = g_state.outputDirBuf;
        g_state.settings.save();
        break;
    }

    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }

    return DefWindowProcW(hWnd, msg, wParam, lParam);
}

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE, LPWSTR, int nCmdShow) {
    SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

    WNDCLASSEXW wc   = {};
    wc.cbSize        = sizeof(WNDCLASSEXW);
    wc.style         = CS_CLASSDC;
    wc.lpfnWndProc   = WndProc;
    wc.hInstance     = hInstance;
    wc.hIcon         = LoadIconW(hInstance, MAKEINTRESOURCEW(101));
    wc.lpszClassName = L"BLPViewerClass";
    RegisterClassExW(&wc);

    g_state.settings.load();
    sanitize_window_rect(g_state.settings);

    HWND hwnd = CreateWindowExW(
        WS_EX_ACCEPTFILES,
        wc.lpszClassName,
        L"\x56fe\x50cf\x5feb\x901f\x5904\x7406\x5de5\x5177",
        WS_OVERLAPPEDWINDOW,
        g_state.settings.windowX, g_state.settings.windowY,
        g_state.settings.windowW, g_state.settings.windowH,
        nullptr, nullptr, hInstance, nullptr);

    DragAcceptFiles(hwnd, TRUE);

    if (!create_device_d3d(hwnd, &g_dx)) {
        cleanup_device_d3d(&g_dx);
        UnregisterClassW(wc.lpszClassName, hInstance);
        return 1;
    }

    ShowWindow(hwnd, nCmdShow);
    UpdateWindow(hwnd);

    const float dpi  = static_cast<float>(GetDpiForWindow(hwnd)) / 96.0f;
    g_state.dpiScale = dpi;

    IMGUI_CHECKVERSION();
    ImGui::CreateContext();
    ImGuiIO& io = ImGui::GetIO();
    io.ConfigFlags  |= ImGuiConfigFlags_NavEnableKeyboard;
    io.IniFilename   = nullptr;

    ImGui_ImplWin32_Init(hwnd);
    ImGui_ImplDX11_Init(g_dx.device.Get(), g_dx.deviceContext.Get());

    apply_imgui_style(dpi);
    load_fonts(dpi);

    g_state.hwnd          = hwnd;
    g_state.device        = g_dx.device.Get();
    g_state.deviceContext = g_dx.deviceContext.Get();
    g_state.leftPanelWidth   = g_state.settings.splitterPos;
    g_state.outputFormat     = g_state.settings.outputFormat;
    g_state.quality          = g_state.settings.quality;
    g_state.overwrite        = g_state.settings.overwrite;
    g_state.recursive        = g_state.settings.recursive;
    g_state.batchSizeMode    = g_state.settings.batchSizeMode;
    g_state.batchWidth       = g_state.settings.batchWidth;
    g_state.batchHeight      = g_state.settings.batchHeight;
    g_state.batchLockAspect  = g_state.settings.batchLockAspect;
    g_state.batchResizeMethod = g_state.settings.batchResizeMethod;
    g_state.batchAspect = static_cast<float>(std::max(1, g_state.batchWidth)) /
                          static_cast<float>(std::max(1, g_state.batchHeight));
    if (!g_state.settings.lastInputDir.empty()) {
        std::strncpy(g_state.inputDirBuf, g_state.settings.lastInputDir.c_str(),
                     sizeof(g_state.inputDirBuf) - 1);
    }
    if (!g_state.settings.lastOutputDir.empty()) {
        std::strncpy(g_state.outputDirBuf, g_state.settings.lastOutputDir.c_str(),
                     sizeof(g_state.outputDirBuf) - 1);
    }

    g_state.imageViewer.init(g_dx.device.Get());

    g_state.blpAssociated     = is_blp_associated();
    g_state.pngAssociated     = is_png_associated();
    g_state.tgaAssociated     = is_tga_associated();
    g_state.ddsAssociated     = is_dds_associated();
    g_state.thumbnailRegistered = is_thumbnail_registered();

    int    argc = 0;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLineW(), &argc);
    if (argv) {
        std::vector<std::string> paths;
        for (int i = 1; i < argc; ++i) {
            const std::string path = fs_path_to_utf8(std::filesystem::path(argv[i]));
            if (!path.empty()) paths.push_back(path);
        }
        LocalFree(argv);

        for (const auto& path : paths) {
            namespace fs = std::filesystem;
            try {
                fs::path fsPath = fs_path_from_utf8(path);
                if (!fs::exists(fsPath)) continue;
                if (!is_supported_file(path)) continue;
                const std::string fullPath = fs_path_to_utf8(fs::absolute(fsPath));
                if (g_state.fileSet.count(fullPath)) continue;
                g_state.fileList.push_back(fullPath);
                g_state.fileSet.insert(fullPath);
            } catch (...) {}
        }
        if (!g_state.fileList.empty()) {
            g_state.selectedFileIndex = 0;
            g_state.rightViewMode     = (g_state.fileList.size() > 1) ? 0 : 1;
        }
    }

    constexpr float clearColor[4] = {0.961f, 0.965f, 0.973f, 1.0f};
    bool done = false;

    while (!done) {
        MSG msg;
        while (PeekMessageW(&msg, nullptr, 0U, 0U, PM_REMOVE)) {
            TranslateMessage(&msg);
            DispatchMessageW(&msg);
            if (msg.message == WM_QUIT) done = true;
        }
        if (done) break;

        ImGui_ImplDX11_NewFrame();
        ImGui_ImplWin32_NewFrame();
        ImGui::NewFrame();

        render_main_ui(g_state);

        ImGui::Render();
        g_dx.deviceContext->OMSetRenderTargets(1, g_dx.renderTargetView.GetAddressOf(), nullptr);
        g_dx.deviceContext->ClearRenderTargetView(g_dx.renderTargetView.Get(), clearColor);
        ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());

        g_dx.swapChain->Present(1, 0);
    }

    g_state.thumbCache.clear();
    g_state.imageViewer.release();

    ImGui_ImplDX11_Shutdown();
    ImGui_ImplWin32_Shutdown();
    ImGui::DestroyContext();

    cleanup_device_d3d(&g_dx);
    DestroyWindow(hwnd);
    UnregisterClassW(wc.lpszClassName, hInstance);

    return 0;
}
