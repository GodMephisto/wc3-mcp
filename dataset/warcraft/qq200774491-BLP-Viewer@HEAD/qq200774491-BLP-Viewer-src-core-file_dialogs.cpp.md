---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/file_dialogs.cpp"
ref: HEAD
---

#include "file_dialogs.h"

#include <ShObjIdl.h>
#include <commdlg.h>
#include <wrl/client.h>

using Microsoft::WRL::ComPtr;

namespace {

void init_com() {
    static bool initialized = false;
    if (!initialized) {
        CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);
        initialized = true;
    }
}

} // namespace

std::vector<std::wstring> open_file_dialog(HWND hwnd, const wchar_t* filter, bool multiSelect) {
    init_com();
    std::vector<std::wstring> results;

    ComPtr<IFileOpenDialog> pDialog;
    HRESULT hr = CoCreateInstance(CLSID_FileOpenDialog, nullptr, CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(pDialog.GetAddressOf()));
    if (FAILED(hr)) return results;

    DWORD options = 0;
    pDialog->GetOptions(&options);
    options |= FOS_FORCEFILESYSTEM;
    if (multiSelect) options |= FOS_ALLOWMULTISELECT;
    pDialog->SetOptions(options);

    COMDLG_FILTERSPEC filterSpec = {};
    filterSpec.pszName = L"图片文件";
    filterSpec.pszSpec = filter ? filter : L"*.*";
    pDialog->SetFileTypes(1, &filterSpec);

    hr = pDialog->Show(hwnd);
    if (SUCCEEDED(hr)) {
        if (multiSelect) {
            ComPtr<IShellItemArray> pItems;
            hr = pDialog->GetResults(pItems.GetAddressOf());
            if (SUCCEEDED(hr)) {
                DWORD count = 0;
                pItems->GetCount(&count);
                for (DWORD i = 0; i < count; ++i) {
                    ComPtr<IShellItem> pItem;
                    if (SUCCEEDED(pItems->GetItemAt(i, pItem.GetAddressOf()))) {
                        PWSTR path = nullptr;
                        if (SUCCEEDED(pItem->GetDisplayName(SIGDN_FILESYSPATH, &path))) {
                            results.push_back(path);
                            CoTaskMemFree(path);
                        }
                    }
                }
            }
        } else {
            ComPtr<IShellItem> pItem;
            hr = pDialog->GetResult(pItem.GetAddressOf());
            if (SUCCEEDED(hr)) {
                PWSTR path = nullptr;
                if (SUCCEEDED(pItem->GetDisplayName(SIGDN_FILESYSPATH, &path))) {
                    results.push_back(path);
                    CoTaskMemFree(path);
                }
            }
        }
    }

    return results;
}

std::wstring open_folder_dialog(HWND hwnd) {
    init_com();

    ComPtr<IFileOpenDialog> pDialog;
    HRESULT hr = CoCreateInstance(CLSID_FileOpenDialog, nullptr, CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(pDialog.GetAddressOf()));
    if (FAILED(hr)) return {};

    DWORD options = 0;
    pDialog->GetOptions(&options);
    pDialog->SetOptions(options | FOS_PICKFOLDERS | FOS_FORCEFILESYSTEM);

    hr = pDialog->Show(hwnd);
    std::wstring result;
    if (SUCCEEDED(hr)) {
        ComPtr<IShellItem> pItem;
        hr = pDialog->GetResult(pItem.GetAddressOf());
        if (SUCCEEDED(hr)) {
            PWSTR path = nullptr;
            if (SUCCEEDED(pItem->GetDisplayName(SIGDN_FILESYSPATH, &path))) {
                result = path;
                CoTaskMemFree(path);
            }
        }
    }

    return result;
}
