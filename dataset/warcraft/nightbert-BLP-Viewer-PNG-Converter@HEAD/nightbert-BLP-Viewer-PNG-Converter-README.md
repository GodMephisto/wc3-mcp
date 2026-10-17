---
type: vendor-doc
source_repo: "https://github.com/nightbert/BLP-Viewer-PNG-Converter"
source_path: "README.md"
ref: HEAD
---

# BLP Viewer & PNG Converter

Small Python GUI to view World of Warcraft .blp textures and convert them to .png.
Uses Pillow's built-in BLP support.

## Features
- Choose source folder (recursive)
- Preview BLP files
- Path list: folder path on one line, files slightly indented below
- Convert a single file, a selected folder, or all files
- Folder structure is preserved during batch conversion

![Preview with pink background](preview.png)

## Requirements
- Python 3.9+
- Tkinter (Debian/Ubuntu: `sudo apt install python3-tk`)

## How to run
This project is provided in three formats:
- `blp2png.py` (Python script)
- Windows `.exe`
- Linux `.AppImage`

### Option A: Run the Python script
1) Create and activate a virtual environment:
```bash
python3 -m venv .venv
source .venv/bin/activate
```

2) Install dependencies:
```bash
pip install -r requirements.txt
```

3) Start the app:
```bash
python3 blp2png.py
```

### Option B: Windows .exe
Download the `BLP-Viewer-PNG-Converter-0.1.0-x86_64.exe` and run it (no install needed).

### Option C: Linux AppImage
1) Make it executable:
```bash
chmod +x BLP-Viewer-PNG-Converter-0.1.0-x86_64.AppImage
```

2) Run it:
```bash
./BLP-Viewer-PNG-Converter-0.1.0-x86_64.AppImage
```
