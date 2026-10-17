---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "CHANGELOG.md"
ref: HEAD
---

# Changelog

## [Unreleased]

### Added

- End-to-end DDS support across preview, thumbnails, drag-and-drop, layer composite,
  batch conversion, single-image save, original-format output, file association,
  and Explorer thumbnails
- DDS decoding for legacy/DX10 RGB(A), DXT1/3/5, and BC1–BC5 textures; lossless
  BGRA8 DDS encoding and dedicated codec self-tests

## [1.5.0] - 2026-06-22

### Added

- **Layer composite & border batch operations** — apply image overlays (anchor, margin, scale, opacity) or solid borders (canvas-expand or inset) to all files in the resource list at once
- **Preview toolbar: save with size / alignment** — export the previewed image at a custom resolution with alignment options (stretch, center on transparent background, etc.)
- **Built-in C++ BLP1 codec** — no longer depends on external `blp_lib.dll`; statically compiled, unzip and run
- **Installer improvements** — bilingual (Chinese / English) setup wizard, desktop shortcut option, full uninstall cleanup

### Fixed

- Single-image save now supports format conversion (e.g. BLP → PNG) and correctly overwrites the source file when saving back to the same path
- Fixed decoding failures on some War3 BLP files (CMYK colour space / 3-component / grayscale JPEG payloads)

---

## [1.4.0]

- Batch conversion between BLP / PNG / JPG / BMP / TGA
- Windows Explorer thumbnail integration via `blp_thumbnail.dll` shell extension
- File association manager
- Mipmap level selection in preview
- Alpha channel overlay display

---

# 更新日志

## [未发布]

### 新功能

- DDS 端到端支持：预览、缩略图、拖放、图层合成、批量转换、单图保存、
  原始格式输出、文件关联及资源管理器缩略图均可使用 DDS
- 支持解码传统/DX10 DDS 的 RGB(A)、DXT1/3/5 与 BC1–BC5，支持无损
  BGRA8 DDS 编码，并新增独立编解码自测

## [1.5.0] - 2026-06-22

### 新功能

- **图层合成 / 边框批量处理** — 对资源列表中的全部图像一键叠加图层（锚点、边距、缩放、透明度）或加边框（扩展画布 / 内描边）
- **预览工具栏：保存尺寸 / 对齐** — 按指定分辨率和对齐方式导出当前预览图
- **编解码器完全内置（C++）** — 不再依赖外部 `blp_lib.dll`，直接解压即用
- **安装包改进** — 双语安装界面、桌面快捷方式选项、卸载时完整清理

### 修复

- 单图保存时支持格式转换（如 BLP → PNG），保存回原路径时正确覆盖原文件
- 修复部分 War3 BLP 文件解码异常（CMYK 色彩空间 / 3 通道 / 灰度 JPEG）

---

## [1.4.0]

- BLP / PNG / JPG / BMP / TGA 批量格式互转
- 通过 `blp_thumbnail.dll` Shell 扩展在资源管理器显示缩略图
- 文件关联管理器
- 预览支持 Mipmap 层级切换
- Alpha 通道叠加显示
