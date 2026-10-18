---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/locales/en.json"
ref: HEAD
---

{
  "app": {
    "name": "BLP Converter",
    "tagline": "Fast BLP and PNG conversion for World of Warcraft textures",
    "description": "A lightweight desktop tool for converting Blizzard BLP texture files to PNG and back. Drop files on either side of the window, BLP on the left, PNG on the right, and conversions run in parallel. Outputs land next to your source files unless you pick a custom folder in Settings.",
    "madeBy": "Made by {{author}}",
    "dropToConvert": "Drop files to convert",
    "converting": "Converting…"
  },
  "panels": {
    "toPng": {
      "hint": "Drop .blp files or folders",
      "subhint": "Exports transparent PNG"
    },
    "toBlp": {
      "hint": "Drop .png files or folders",
      "subhint": "Encodes with your compression settings"
    }
  },
  "activity": {
    "title": "Activity",
    "clear": "Clear",
    "empty": "Drop BLP or PNG files, outputs save next to originals by default."
  },
  "settings": {
    "title": "Settings",
    "close": "Close",
    "language": "Language",
    "outputFolder": "Output Folder",
    "outputFolderHint": "Optional. Leave unset to save converted files next to the originals.",
    "outputDefault": "Same folder as source (default)",
    "resetOutput": "Reset to source folder",
    "compression": "PNG to BLP Compression",
    "compressionRaw": "Uncompressed / RAW",
    "compressionDxt1": "DXT1 (opaque or 1-bit alpha)",
    "compressionDxt3": "DXT3 (sharp alpha edges)",
    "compressionDxt5": "DXT5 (smooth alpha)",
    "generateMipmaps": "Generate mipmaps for BLP output",
    "pickOutputTitle": "Select Output Directory"
  },
  "blpview": {
    "title": "Explorer Thumbnails (BLPView)",
    "install": "Install",
    "uninstall": "Uninstall",
    "restartExplorer": "Restart Explorer",
    "statusError": "Could not read BLPView status.",
    "installFailed": "Install failed",
    "uninstallFailed": "Uninstall failed",
    "restartFailed": "Could not restart Explorer",
    "loadingStatus": "Checking BLPView status…",
    "unsupportedReason": "BLPView is a Windows File Explorer shell extension. Folder thumbnails for .blp files are only available on Windows 10/11."
  },
  "conversion": {
    "start": "Converting {{count}} file(s) ({{label}})…",
    "outputSameFolder": "Output: same folder as source",
    "noFiles": "No {{format}} files found for this side.",
    "fileResult": "{{name}} -> {{path}}",
    "failed": "Conversion failed",
    "done": "Done. {{succeeded}} succeeded, {{failed}} failed.",
    "scanError": "Scan: {{error}}",
    "dropFailed": "Failed to process dropped files",
    "busy": "Already converting, wait for the current batch to finish.",
    "outputCustomFolder": "Output: {{path}}",
    "labelBlpToPng": "BLP to PNG",
    "labelPngToBlp": "PNG to BLP"
  },
  "about": {
    "title": "About"
  },
  "errorBoundary": {
    "title": "Something went wrong",
    "fallback": "An unexpected error occurred.",
    "retry": "Try again"
  }
}
