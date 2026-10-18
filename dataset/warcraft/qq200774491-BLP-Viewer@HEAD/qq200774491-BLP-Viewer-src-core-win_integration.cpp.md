---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/win_integration.cpp"
ref: HEAD
---

#include "win_integration.h"

#include <Shlwapi.h>
#include <ShlObj.h>
#include <filesystem>

#pragma comment(lib, "Shlwapi.lib")

namespace {

const wchar_t* kViewerProgId    = L"BLPViewer.File";
const wchar_t* kThumbnailClsid  = L"{27A35239-0B87-4085-8944-463B440D162F}";
const wchar_t* kThumbnailHandler = L"{E357FCCD-A995-4576-B01F-234630154E96}";

bool reg_set_string(HKEY hKey, const wchar_t* valueName, const wchar_t* data) {
    return RegSetValueExW(hKey, valueName, 0, REG_SZ,
                          reinterpret_cast<const BYTE*>(data),
                          static_cast<DWORD>((wcslen(data) + 1) * sizeof(wchar_t))) == ERROR_SUCCESS;
}

bool create_key_and_set(const wchar_t* subKey, const wchar_t* value) {
    HKEY hKey = nullptr;
    std::wstring fullKey = std::wstring(L"Software\\Classes\\") + subKey;
    if (RegCreateKeyExW(HKEY_CURRENT_USER, fullKey.c_str(), 0, nullptr,
                        REG_OPTION_NON_VOLATILE, KEY_WRITE, nullptr,
                        &hKey, nullptr) != ERROR_SUCCESS) {
        return false;
    }
    const bool ok = reg_set_string(hKey, nullptr, value);
    RegCloseKey(hKey);
    return ok;
}

bool is_ext_associated(const wchar_t* ext) {
    wchar_t buffer[256] = {};
    DWORD size = ARRAYSIZE(buffer);
    HRESULT hr = AssocQueryStringW(ASSOCF_NONE, ASSOCSTR_PROGID,
                                   ext, nullptr, buffer, &size);
    if (SUCCEEDED(hr)) {
        return _wcsicmp(buffer, kViewerProgId) == 0;
    }

    std::wstring extKey = std::wstring(L"Software\\Classes\\") + ext;
    HKEY hKey = nullptr;
    if (RegOpenKeyExW(HKEY_CURRENT_USER, extKey.c_str(), 0, KEY_READ, &hKey) == ERROR_SUCCESS) {
        wchar_t val[256] = {};
        DWORD valSize = sizeof(val);
        DWORD type = 0;
        if (RegQueryValueExW(hKey, nullptr, nullptr, &type,
                             reinterpret_cast<BYTE*>(val), &valSize) == ERROR_SUCCESS) {
            RegCloseKey(hKey);
            return _wcsicmp(val, kViewerProgId) == 0;
        }
        RegCloseKey(hKey);
    }
    return false;
}

bool register_ext_association(const std::wstring& appPath,
                              const wchar_t* ext,
                              const wchar_t* contentType,
                              std::string* outError) {
    std::wstring extKey = std::wstring(L"Software\\Classes\\") + ext;
    HKEY hKey = nullptr;
    if (RegCreateKeyExW(HKEY_CURRENT_USER, extKey.c_str(), 0, nullptr,
                        REG_OPTION_NON_VOLATILE, KEY_WRITE, nullptr,
                        &hKey, nullptr) != ERROR_SUCCESS) {
        if (outError) *outError = "写入注册表失败";
        return false;
    }
    reg_set_string(hKey, nullptr, kViewerProgId);
    reg_set_string(hKey, L"PerceivedType", L"image");
    if (contentType && *contentType) {
        reg_set_string(hKey, L"Content Type", contentType);
    }
    RegCloseKey(hKey);

    std::wstring openWithKey = std::wstring(ext) + L"\\OpenWithProgids";
    create_key_and_set(openWithKey.c_str(), L"");
    {
        HKEY openWith = nullptr;
        std::wstring fullOpenWithKey = std::wstring(L"Software\\Classes\\") + openWithKey;
        if (RegOpenKeyExW(HKEY_CURRENT_USER, fullOpenWithKey.c_str(), 0, KEY_WRITE, &openWith) == ERROR_SUCCESS) {
            RegSetValueExW(openWith, kViewerProgId, 0, REG_SZ, reinterpret_cast<const BYTE*>(L""), sizeof(wchar_t));
            RegCloseKey(openWith);
        }
    }

    create_key_and_set(L"BLPViewer.File", L"BLP Viewer Image");

    std::wstring iconValue = appPath + L",0";
    create_key_and_set(L"BLPViewer.File\\DefaultIcon", iconValue.c_str());

    std::wstring cmdValue = L"\"" + appPath + L"\" \"%1\"";
    create_key_and_set(L"BLPViewer.File\\shell\\open\\command", cmdValue.c_str());

    std::wstring exeName = std::filesystem::path(appPath).filename().wstring();
    std::wstring appKey = L"Applications\\" + exeName + L"\\shell\\open\\command";
    create_key_and_set(appKey.c_str(), cmdValue.c_str());

    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, nullptr, nullptr);
    return true;
}

} // namespace

std::wstring get_app_path() {
    wchar_t buf[MAX_PATH] = {};
    GetModuleFileNameW(nullptr, buf, MAX_PATH);
    return buf;
}

std::wstring get_app_dir() {
    return std::filesystem::path(get_app_path()).parent_path().wstring();
}

bool is_blp_associated() {
    return is_ext_associated(L".blp");
}

bool register_blp_association(const std::wstring& appPath, std::string* outError) {
    return register_ext_association(appPath, L".blp", L"image/blp", outError);
}

bool is_png_associated() {
    return is_ext_associated(L".png");
}

bool register_png_association(const std::wstring& appPath, std::string* outError) {
    return register_ext_association(appPath, L".png", L"image/png", outError);
}

bool is_tga_associated() {
    return is_ext_associated(L".tga");
}

bool register_tga_association(const std::wstring& appPath, std::string* outError) {
    return register_ext_association(appPath, L".tga", L"image/x-tga", outError);
}

bool is_dds_associated() {
    return is_ext_associated(L".dds");
}

bool register_dds_association(const std::wstring& appPath, std::string* outError) {
    return register_ext_association(appPath, L".dds", L"image/vnd-ms.dds", outError);
}

bool is_thumbnail_registered() {
    std::wstring key = std::wstring(L"Software\\Classes\\.blp\\ShellEx\\") + kThumbnailHandler;
    HKEY hKey = nullptr;
    if (RegOpenKeyExW(HKEY_CURRENT_USER, key.c_str(), 0, KEY_READ, &hKey) != ERROR_SUCCESS) {
        return false;
    }

    wchar_t val[256] = {};
    DWORD valSize = sizeof(val);
    DWORD type = 0;
    bool result = false;
    if (RegQueryValueExW(hKey, nullptr, nullptr, &type,
                         reinterpret_cast<BYTE*>(val), &valSize) == ERROR_SUCCESS) {
        result = (_wcsicmp(val, kThumbnailClsid) == 0);
    }
    RegCloseKey(hKey);
    return result;
}

bool call_dll_entry(const std::wstring& dllPath, const char* entry, std::string* outError) {
    HMODULE module = LoadLibraryW(dllPath.c_str());
    if (!module) {
        if (outError) *outError = "加载缩略图 DLL 失败";
        return false;
    }

    auto* func = reinterpret_cast<HRESULT(STDAPICALLTYPE*)(void)>(
        GetProcAddress(module, entry));
    if (!func) {
        if (outError) *outError = "找不到注册入口";
        FreeLibrary(module);
        return false;
    }

    HRESULT hr = func();
    FreeLibrary(module);
    if (FAILED(hr)) {
        if (outError) *outError = "注册调用失败";
        return false;
    }
    return true;
}
