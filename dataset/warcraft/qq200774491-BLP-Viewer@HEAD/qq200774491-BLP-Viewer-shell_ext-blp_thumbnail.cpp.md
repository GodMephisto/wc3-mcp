---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "shell_ext/blp_thumbnail.cpp"
ref: HEAD
---

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <windows.h>
#include <ShlObj.h>
#include <thumbcache.h>
#include <shlwapi.h>
#include <objidl.h>
#include <wrl/client.h>

#include <algorithm>
#include <cmath>
#include <cstring>
#include <limits>
#include <new>
#include <vector>

#include "blp/blp_codec.h"
#include "core/dds_codec.h"

#define STB_IMAGE_IMPLEMENTATION
#define STBI_ONLY_TGA
#define STBI_ONLY_PNG
#define STBI_NO_STDIO
#include "stb_image.h"

#pragma comment(lib, "gdi32.lib")
#pragma comment(lib, "ole32.lib")
#pragma comment(lib, "shell32.lib")
#pragma comment(lib, "shlwapi.lib")

using Microsoft::WRL::ComPtr;

namespace {

const CLSID CLSID_BlpThumbnailProvider = {
    0x27A35239,
    0x0B87,
    0x4085,
    {0x89, 0x44, 0x46, 0x3B, 0x44, 0x0D, 0x16, 0x2F}};

const wchar_t* kThumbnailClsid   = L"{27A35239-0B87-4085-8944-463B440D162F}";
const wchar_t* kThumbnailHandler = L"{E357FCCD-A995-4576-B01F-234630154E96}";
const wchar_t* kBlpProgId        = L"BLPViewer.File";

HINSTANCE g_hInstance = nullptr;
long      g_cDllRef   = 0;

HRESULT read_stream(IStream* stream, std::vector<uint8_t>& outBytes) {
    if (!stream) return E_INVALIDARG;

    STATSTG stat = {};
    HRESULT hr   = stream->Stat(&stat, STATFLAG_NONAME);
    if (FAILED(hr)) return hr;
    if (stat.cbSize.HighPart != 0) return E_FAIL;

    const ULONG size = stat.cbSize.LowPart;
    if (size == 0) return E_FAIL;

    LARGE_INTEGER zero = {};
    stream->Seek(zero, STREAM_SEEK_SET, nullptr);

    outBytes.resize(size);
    ULONG read = 0;
    hr = stream->Read(outBytes.data(), size, &read);
    if (FAILED(hr) || read != size) return E_FAIL;
    return S_OK;
}

bool is_blp_bytes(const std::vector<uint8_t>& bytes) {
    if (bytes.size() < 4) return false;
    return std::memcmp(bytes.data(), "BLP1", 4) == 0 ||
           std::memcmp(bytes.data(), "BLP2", 4) == 0;
}

inline uint8_t premultiply(uint8_t channel, uint8_t alpha) {
    return static_cast<uint8_t>((static_cast<unsigned int>(channel) * alpha + 127) / 255);
}

inline uint8_t clamp_to_byte(float value) {
    if (value <= 0.0f)   return 0;
    if (value >= 255.0f) return 255;
    return static_cast<uint8_t>(value + 0.5f);
}

std::vector<uint8_t> rgba_to_bgra_premultiplied(const uint8_t* rgba, size_t pixelCount) {
    std::vector<uint8_t> bgra(pixelCount * 4);
    for (size_t i = 0; i < pixelCount; ++i) {
        const size_t  idx = i * 4;
        const uint8_t a   = rgba[idx + 3];
        bgra[idx + 0] = premultiply(rgba[idx + 2], a);
        bgra[idx + 1] = premultiply(rgba[idx + 1], a);
        bgra[idx + 2] = premultiply(rgba[idx + 0], a);
        bgra[idx + 3] = a;
    }
    return bgra;
}

std::vector<uint8_t> scale_rgba_to_bgra_premultiplied(const uint8_t* rgba,
                                                       int srcW, int srcH,
                                                       int dstW, int dstH) {
    if (!rgba || srcW <= 0 || srcH <= 0 || dstW <= 0 || dstH <= 0) return {};
    const size_t pixelCount = static_cast<size_t>(dstW) * static_cast<size_t>(dstH);
    if (pixelCount > (std::numeric_limits<size_t>::max() / 4)) return {};

    if (srcW == dstW && srcH == dstH) {
        return rgba_to_bgra_premultiplied(rgba,
            static_cast<size_t>(srcW) * static_cast<size_t>(srcH));
    }

    std::vector<uint8_t> out(pixelCount * 4);
    const float scaleX = static_cast<float>(srcW) / static_cast<float>(dstW);
    const float scaleY = static_cast<float>(srcH) / static_cast<float>(dstH);

    for (int y = 0; y < dstH; ++y) {
        float srcY = (static_cast<float>(y) + 0.5f) * scaleY - 0.5f;
        int   y0   = static_cast<int>(std::floor(srcY));
        int   y1   = y0 + 1;
        float wy   = srcY - static_cast<float>(y0);
        if (y0 < 0)        { y0 = 0;       y1 = 0;       wy = 0.0f; }
        else if (y1 >= srcH) { y1 = srcH - 1; y0 = y1;   wy = 0.0f; }

        for (int x = 0; x < dstW; ++x) {
            float srcX = (static_cast<float>(x) + 0.5f) * scaleX - 0.5f;
            int   x0   = static_cast<int>(std::floor(srcX));
            int   x1   = x0 + 1;
            float wx   = srcX - static_cast<float>(x0);
            if (x0 < 0)        { x0 = 0;       x1 = 0;       wx = 0.0f; }
            else if (x1 >= srcW) { x1 = srcW - 1; x0 = x1;   wx = 0.0f; }

            const float w00 = (1.0f - wx) * (1.0f - wy);
            const float w10 =         wx  * (1.0f - wy);
            const float w01 = (1.0f - wx) *         wy;
            const float w11 =         wx  *         wy;

            const uint8_t* p00 = rgba + (static_cast<size_t>(y0) * srcW + x0) * 4;
            const uint8_t* p10 = rgba + (static_cast<size_t>(y0) * srcW + x1) * 4;
            const uint8_t* p01 = rgba + (static_cast<size_t>(y1) * srcW + x0) * 4;
            const uint8_t* p11 = rgba + (static_cast<size_t>(y1) * srcW + x1) * 4;

            const float a00 = static_cast<float>(p00[3]);
            const float a10 = static_cast<float>(p10[3]);
            const float a01 = static_cast<float>(p01[3]);
            const float a11 = static_cast<float>(p11[3]);

            const float r = w00 * static_cast<float>(p00[0]) * (a00 / 255.0f)
                          + w10 * static_cast<float>(p10[0]) * (a10 / 255.0f)
                          + w01 * static_cast<float>(p01[0]) * (a01 / 255.0f)
                          + w11 * static_cast<float>(p11[0]) * (a11 / 255.0f);
            const float g = w00 * static_cast<float>(p00[1]) * (a00 / 255.0f)
                          + w10 * static_cast<float>(p10[1]) * (a10 / 255.0f)
                          + w01 * static_cast<float>(p01[1]) * (a01 / 255.0f)
                          + w11 * static_cast<float>(p11[1]) * (a11 / 255.0f);
            const float b = w00 * static_cast<float>(p00[2]) * (a00 / 255.0f)
                          + w10 * static_cast<float>(p10[2]) * (a10 / 255.0f)
                          + w01 * static_cast<float>(p01[2]) * (a01 / 255.0f)
                          + w11 * static_cast<float>(p11[2]) * (a11 / 255.0f);
            const float a = w00 * a00 + w10 * a10 + w01 * a01 + w11 * a11;

            const size_t outIdx = (static_cast<size_t>(y) * dstW + x) * 4;
            out[outIdx + 0] = clamp_to_byte(b);
            out[outIdx + 1] = clamp_to_byte(g);
            out[outIdx + 2] = clamp_to_byte(r);
            out[outIdx + 3] = clamp_to_byte(a);
        }
    }
    return out;
}

HBITMAP create_thumbnail_bitmap_from_rgba(const uint8_t* rgba, int width, int height, UINT cx) {
    if (!rgba || width <= 0 || height <= 0 || cx == 0) return nullptr;

    const double scale = std::min(static_cast<double>(cx) / width,
                                  static_cast<double>(cx) / height);
    const int destW = std::max(1, static_cast<int>(width  * scale));
    const int destH = std::max(1, static_cast<int>(height * scale));

    BITMAPINFO destInfo              = {};
    destInfo.bmiHeader.biSize        = sizeof(BITMAPINFOHEADER);
    destInfo.bmiHeader.biWidth       = destW;
    destInfo.bmiHeader.biHeight      = -destH;
    destInfo.bmiHeader.biPlanes      = 1;
    destInfo.bmiHeader.biBitCount    = 32;
    destInfo.bmiHeader.biCompression = BI_RGB;

    void*  destBits = nullptr;
    HBITMAP hbmp = CreateDIBSection(nullptr, &destInfo, DIB_RGB_COLORS, &destBits, nullptr, 0);
    if (!hbmp) return nullptr;

    const std::vector<uint8_t> bgra =
        scale_rgba_to_bgra_premultiplied(rgba, width, height, destW, destH);
    if (bgra.empty()) { DeleteObject(hbmp); return nullptr; }

    std::memcpy(destBits, bgra.data(), bgra.size());
    return hbmp;
}

bool try_decode_blp(const std::vector<uint8_t>& bytes, UINT cx, HBITMAP* outBmp) {
    if (!outBmp || !is_blp_bytes(bytes)) return false;

    auto image = blpcodec::decode(bytes.data(), bytes.size(), nullptr);
    if (!image) return false;

    HBITMAP hbmp = create_thumbnail_bitmap_from_rgba(
        image->rgba.data(), static_cast<int>(image->width), static_cast<int>(image->height), cx);
    if (!hbmp) return false;

    *outBmp = hbmp;
    return true;
}

bool try_decode_dds(const std::vector<uint8_t>& bytes, UINT cx, HBITMAP* outBmp) {
    if (!outBmp || !ddscodec::is_dds(bytes.data(), bytes.size())) return false;

    auto image = ddscodec::decode(bytes.data(), bytes.size(), nullptr);
    if (!image) return false;

    HBITMAP hbmp = create_thumbnail_bitmap_from_rgba(
        image->rgba.data(), static_cast<int>(image->width), static_cast<int>(image->height), cx);
    if (!hbmp) return false;

    *outBmp = hbmp;
    return true;
}

bool try_decode_raster(const std::vector<uint8_t>& bytes, UINT cx, HBITMAP* outBmp) {
    if (!outBmp) return false;
    if (bytes.size() > static_cast<size_t>(std::numeric_limits<int>::max())) return false;

    int      width = 0, height = 0, comp = 0;
    stbi_uc* data = stbi_load_from_memory(bytes.data(), static_cast<int>(bytes.size()),
                                          &width, &height, &comp, 4);
    if (!data) return false;

    HBITMAP hbmp = create_thumbnail_bitmap_from_rgba(data, width, height, cx);
    stbi_image_free(data);
    if (!hbmp) return false;

    *outBmp = hbmp;
    return true;
}

bool set_registry_string(HKEY root, const wchar_t* subKey,
                          const wchar_t* name, const wchar_t* value) {
    HKEY key = nullptr;
    if (RegCreateKeyExW(root, subKey, 0, nullptr, 0, KEY_SET_VALUE, nullptr, &key, nullptr) !=
        ERROR_SUCCESS) {
        return false;
    }
    const DWORD size   = static_cast<DWORD>((wcslen(value) + 1) * sizeof(wchar_t));
    const LONG  result = RegSetValueExW(key, name, 0, REG_SZ,
                                        reinterpret_cast<const BYTE*>(value), size);
    RegCloseKey(key);
    return result == ERROR_SUCCESS;
}

class BlpThumbnailProvider : public IInitializeWithStream, public IThumbnailProvider {
public:
    BlpThumbnailProvider() : ref_(1) { InterlockedIncrement(&g_cDllRef); }
    ~BlpThumbnailProvider()          { InterlockedDecrement(&g_cDllRef); }

    IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv) override {
        if (!ppv) return E_POINTER;
        *ppv = nullptr;
        if (IsEqualIID(riid, IID_IUnknown) || IsEqualIID(riid, IID_IInitializeWithStream)) {
            *ppv = static_cast<IInitializeWithStream*>(this);
        } else if (IsEqualIID(riid, IID_IThumbnailProvider)) {
            *ppv = static_cast<IThumbnailProvider*>(this);
        } else {
            return E_NOINTERFACE;
        }
        AddRef();
        return S_OK;
    }

    IFACEMETHODIMP_(ULONG) AddRef() override {
        return static_cast<ULONG>(InterlockedIncrement(&ref_));
    }

    IFACEMETHODIMP_(ULONG) Release() override {
        const long value = InterlockedDecrement(&ref_);
        if (value == 0) delete this;
        return static_cast<ULONG>(value);
    }

    IFACEMETHODIMP Initialize(IStream* stream, DWORD) override {
        if (!stream)  return E_INVALIDARG;
        if (stream_)  return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
        stream_ = stream;
        return S_OK;
    }

    IFACEMETHODIMP GetThumbnail(UINT cx, HBITMAP* phbmp, WTS_ALPHATYPE* pdwAlpha) override {
        if (!phbmp || !pdwAlpha) return E_POINTER;
        *phbmp    = nullptr;
        *pdwAlpha = WTSAT_UNKNOWN;

        std::vector<uint8_t> bytes;
        HRESULT hr = read_stream(stream_.Get(), bytes);
        if (FAILED(hr)) return hr;

        HBITMAP hbmp = nullptr;
        if (!try_decode_blp(bytes, cx, &hbmp) &&
            !try_decode_dds(bytes, cx, &hbmp) &&
            !try_decode_raster(bytes, cx, &hbmp)) {
            return E_FAIL;
        }
        if (!hbmp) return E_FAIL;

        *phbmp    = hbmp;
        *pdwAlpha = WTSAT_ARGB;
        return S_OK;
    }

private:
    long            ref_;
    ComPtr<IStream> stream_;
};

class BlpClassFactory : public IClassFactory {
public:
    BlpClassFactory()  { InterlockedIncrement(&g_cDllRef); }
    ~BlpClassFactory() { InterlockedDecrement(&g_cDllRef); }

    IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv) override {
        if (!ppv) return E_POINTER;
        *ppv = nullptr;
        if (IsEqualIID(riid, IID_IUnknown) || IsEqualIID(riid, IID_IClassFactory)) {
            *ppv = static_cast<IClassFactory*>(this);
            AddRef();
            return S_OK;
        }
        return E_NOINTERFACE;
    }

    IFACEMETHODIMP_(ULONG) AddRef() override {
        return static_cast<ULONG>(InterlockedIncrement(&ref_));
    }

    IFACEMETHODIMP_(ULONG) Release() override {
        const long value = InterlockedDecrement(&ref_);
        if (value == 0) delete this;
        return static_cast<ULONG>(value);
    }

    IFACEMETHODIMP CreateInstance(IUnknown* pUnkOuter, REFIID riid, void** ppv) override {
        if (pUnkOuter) return CLASS_E_NOAGGREGATION;
        auto* provider = new (std::nothrow) BlpThumbnailProvider();
        if (!provider)  return E_OUTOFMEMORY;
        HRESULT hr = provider->QueryInterface(riid, ppv);
        provider->Release();
        return hr;
    }

    IFACEMETHODIMP LockServer(BOOL lock) override {
        if (lock) InterlockedIncrement(&g_cDllRef);
        else      InterlockedDecrement(&g_cDllRef);
        return S_OK;
    }

private:
    long ref_ = 1;
};

} // namespace

extern "C" BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID) {
    if (reason == DLL_PROCESS_ATTACH) {
        g_hInstance = hModule;
        DisableThreadLibraryCalls(hModule);
    }
    return TRUE;
}

STDAPI DllCanUnloadNow(void) {
    return (g_cDllRef == 0) ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void** ppv) {
    if (!IsEqualCLSID(rclsid, CLSID_BlpThumbnailProvider)) {
        return CLASS_E_CLASSNOTAVAILABLE;
    }
    auto* factory = new (std::nothrow) BlpClassFactory();
    if (!factory) return E_OUTOFMEMORY;
    HRESULT hr = factory->QueryInterface(riid, ppv);
    factory->Release();
    return hr;
}

STDAPI DllRegisterServer(void) {
    wchar_t modulePath[MAX_PATH] = {};
    if (!GetModuleFileNameW(g_hInstance, modulePath, MAX_PATH)) {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    wchar_t clsidKey[MAX_PATH] = {};
    wsprintfW(clsidKey, L"Software\\Classes\\CLSID\\%s", kThumbnailClsid);
    if (!set_registry_string(HKEY_CURRENT_USER, clsidKey, nullptr, L"BLP Thumbnail Provider"))
        return E_FAIL;

    wchar_t inprocKey[MAX_PATH] = {};
    wsprintfW(inprocKey, L"%s\\InprocServer32", clsidKey);
    if (!set_registry_string(HKEY_CURRENT_USER, inprocKey, nullptr, modulePath))          return E_FAIL;
    if (!set_registry_string(HKEY_CURRENT_USER, inprocKey, L"ThreadingModel", L"Apartment")) return E_FAIL;

    wchar_t shellExKey[MAX_PATH] = {};
    wsprintfW(shellExKey, L"Software\\Classes\\.blp\\ShellEx\\%s", kThumbnailHandler);
    if (!set_registry_string(HKEY_CURRENT_USER, shellExKey, nullptr, kThumbnailClsid)) return E_FAIL;

    wchar_t sfaKey[MAX_PATH] = {};
    wsprintfW(sfaKey, L"Software\\Classes\\SystemFileAssociations\\.blp\\ShellEx\\%s",
              kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, sfaKey, nullptr, kThumbnailClsid);

    wchar_t progKey[MAX_PATH] = {};
    wsprintfW(progKey, L"Software\\Classes\\%s\\ShellEx\\%s", kBlpProgId, kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, progKey, nullptr, kThumbnailClsid);

    wchar_t tgaShellExKey[MAX_PATH] = {};
    wsprintfW(tgaShellExKey, L"Software\\Classes\\.tga\\ShellEx\\%s", kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, tgaShellExKey, nullptr, kThumbnailClsid);

    wchar_t tgaSfaKey[MAX_PATH] = {};
    wsprintfW(tgaSfaKey, L"Software\\Classes\\SystemFileAssociations\\.tga\\ShellEx\\%s",
              kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, tgaSfaKey, nullptr, kThumbnailClsid);

    wchar_t pngShellExKey[MAX_PATH] = {};
    wsprintfW(pngShellExKey, L"Software\\Classes\\.png\\ShellEx\\%s", kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, pngShellExKey, nullptr, kThumbnailClsid);

    wchar_t pngSfaKey[MAX_PATH] = {};
    wsprintfW(pngSfaKey, L"Software\\Classes\\SystemFileAssociations\\.png\\ShellEx\\%s",
              kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, pngSfaKey, nullptr, kThumbnailClsid);

    wchar_t ddsShellExKey[MAX_PATH] = {};
    wsprintfW(ddsShellExKey, L"Software\\Classes\\.dds\\ShellEx\\%s", kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, ddsShellExKey, nullptr, kThumbnailClsid);

    wchar_t ddsSfaKey[MAX_PATH] = {};
    wsprintfW(ddsSfaKey, L"Software\\Classes\\SystemFileAssociations\\.dds\\ShellEx\\%s",
              kThumbnailHandler);
    set_registry_string(HKEY_CURRENT_USER, ddsSfaKey, nullptr, kThumbnailClsid);

    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, nullptr, nullptr);
    return S_OK;
}

STDAPI DllUnregisterServer(void) {
    wchar_t clsidKey[MAX_PATH] = {};
    wsprintfW(clsidKey, L"Software\\Classes\\CLSID\\%s", kThumbnailClsid);
    SHDeleteKeyW(HKEY_CURRENT_USER, clsidKey);

    wchar_t shellExKey[MAX_PATH] = {};
    wsprintfW(shellExKey, L"Software\\Classes\\.blp\\ShellEx\\%s", kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, shellExKey);

    wchar_t sfaKey[MAX_PATH] = {};
    wsprintfW(sfaKey, L"Software\\Classes\\SystemFileAssociations\\.blp\\ShellEx\\%s",
              kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, sfaKey);

    wchar_t progKey[MAX_PATH] = {};
    wsprintfW(progKey, L"Software\\Classes\\%s\\ShellEx\\%s", kBlpProgId, kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, progKey);

    wchar_t tgaShellExKey[MAX_PATH] = {};
    wsprintfW(tgaShellExKey, L"Software\\Classes\\.tga\\ShellEx\\%s", kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, tgaShellExKey);

    wchar_t tgaSfaKey[MAX_PATH] = {};
    wsprintfW(tgaSfaKey, L"Software\\Classes\\SystemFileAssociations\\.tga\\ShellEx\\%s",
              kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, tgaSfaKey);

    wchar_t pngShellExKey[MAX_PATH] = {};
    wsprintfW(pngShellExKey, L"Software\\Classes\\.png\\ShellEx\\%s", kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, pngShellExKey);

    wchar_t pngSfaKey[MAX_PATH] = {};
    wsprintfW(pngSfaKey, L"Software\\Classes\\SystemFileAssociations\\.png\\ShellEx\\%s",
              kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, pngSfaKey);

    wchar_t ddsShellExKey[MAX_PATH] = {};
    wsprintfW(ddsShellExKey, L"Software\\Classes\\.dds\\ShellEx\\%s", kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, ddsShellExKey);

    wchar_t ddsSfaKey[MAX_PATH] = {};
    wsprintfW(ddsSfaKey, L"Software\\Classes\\SystemFileAssociations\\.dds\\ShellEx\\%s",
              kThumbnailHandler);
    SHDeleteKeyW(HKEY_CURRENT_USER, ddsSfaKey);

    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, nullptr, nullptr);
    return S_OK;
}
