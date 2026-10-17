---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "plans/v2-gui-architecture.md"
ref: HEAD
---

# v2.0 GUI Architecture — Double-Click EXE with Premium UX

## Technology Choice: PySide6 (Qt for Python)

**Why Qt over alternatives:**

| Option | Native Look | Bundle Size | Widget Richness | Threading | License |
|---|---|---|---|---|---|
| **PySide6** | ✅ True native | +40MB | ✅ Tables, trees, tabs, drag-drop | ✅ QThread | LGPL |
| tkinter | ❌ Dated 90s look | 0MB (bundled) | ❌ Minimal | ❌ Fragile | PSF |
| CustomTkinter | ⚠️ Better tkinter | +2MB | ⚠️ Limited | ❌ Fragile | MIT |
| wxPython | ✅ Native | +30MB | ✅ Good | ✅ | LGPL |
| Electron | ⚠️ Web-ish | +150MB | ✅ Full web | ✅ | MIT |
| Flet/Flutter | ⚠️ Mobile-first | +80MB | ⚠️ Immature | ❌ | Apache |

**PySide6 wins:** professional native Windows look, dark theme built-in, QThread for background conversion, QTreeWidget for model lists, QWebEngineView for HTML reports, drag-and-drop via QMimeData.

---

## Window Layout

```
┌──────────────────────────────────────────────────────────────┐
│  WC3 → SC2 Model Converter v2.0              [_] [□] [X]    │
├──────────────────────────────────────────────────────────────┤
│  [Convert]  [Batch]  [History]  [Settings]  [About]          │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─ Model Queue ─────────────────────────────┐  ┌─ Preview ─┐│
│  │ ┌──────────────────────────────────────┐  │  │           ││
│  │ │ Name          │ Status   │ Warnings  │  │  │  Model    ││
│  │ │──────────────────────────────────────│  │  │  preview  ││
│  │ │ Footman.mdx   │ ✓ Done   │ 0         │  │  │  image    ││
│  │ │ Rifleman.mdx  │ ⏳ 45%   │ 2 ⚠      │  │  │           ││
│  │ │ Knight.mdx    │ ☐ Queued │ —         │  │  │  or        ││
│  │ │ Gryphon.mdx   │ ✗ Failed │ 1 error   │  │  │  HTML     ││
│  │ │                                      │  │  │  report   ││
│  │ └──────────────────────────────────────┘  │  │           ││
│  │                                          │  │           ││
│  │  [+ Add Models]  [Add Folder]  [Clear]   │  │           ││
│  │  [▸ Convert All]  [■ Stop]               │  │           ││
│  └──────────────────────────────────────────┘  └───────────┘│
│                                                              │
│  ┌─ Conversion Log ─────────────────────────────────────────┐│
│  │  12:34:01  INFO     Parsing Footman.mdx (v800)...        ││
│  │  12:34:02  INFO     4 textures resolved                  ││
│  │  12:34:05  WARNING  tex[2] alpha inverted — auto-fixed   ││
│  │  12:34:10  INFO     Baking 7 animations (30 fps)         ││
│  │  12:34:25  SUCCESS  Footman.m3 — 2.1 MB, 0 warnings     ││
│  │  12:34:25  INFO     Parsing Rifleman.mdx (v800)...       ││
│  └──────────────────────────────────────────────────────────┘│
│                                                              │
│  ████████████████████░░░░  2/4 models  |  Overall 52%        │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## Tab Breakdown

### Tab 1: Convert (Single/Batch)

**Left panel — Model Queue:**
- QTreeWidget with columns: Name, Status (icon), Progress bar, Warnings, Output Size
- Right-click context menu: Remove, Retry, Show Output, Show Report
- Drag-and-drop zone: accepts `.mdx` files and folders
- [+ Add Models] opens file dialog (multi-select `.mdx`)
- [Add Folder] opens directory picker, recursively finds `.mdx`
- [Clear] removes completed items, keeps queued/running
- [▸ Convert All] starts processing the queue
- [■ Stop] gracefully cancels (finishes current model, skips rest)

**Right panel — Preview/Report:**
- When a model is selected: shows the conversion report (HTML rendered via QWebEngineView or QTextBrowser)
- When no model is selected: shows the Naaru example preview or drag-drop instructions
- Error models: shows the error details with suggested fixes

**Bottom — Log:**
- QPlainTextEdit with monospace font, read-only
- Color-coded entries: white=INFO, yellow=WARNING, red=ERROR, green=SUCCESS
- Auto-scrolls to bottom
- [Save Log] button exports to `.txt`

**Overall progress bar:**
- Shows X/Y models processed + percentage
- Determinate progress for current model conversion

### Tab 2: Batch (Directory/MPQ)

```
┌─ Batch Source ───────────────────────────────────────────────┐
│  ○ Convert a folder of .mdx files                            │
│    Folder: [C:\My Models\Human\]           [Browse...]       │
│    ☑ Include subdirectories                                 │
│                                                              │
│  ○ Extract from WC3 MPQ archive                              │
│    MPQ file: [C:\Games\WC3\war3.mpq\]       [Browse...]      │
│    Filter: [Units\Human\*\]                                │
│    ☑ Auto-extract textures                                  │
│                                                              │
│  Output: [C:\Converted\]                   [Browse...]      │
│  Scale:  [0.05]  ☑ Auto-estimate                            │
│                                                              │
│  Found: 47 .mdx files  |  [▸ Convert All]                   │
└──────────────────────────────────────────────────────────────┘
```

### Tab 3: History

- Table of all past conversions (stored in `%APPDATA%/wc3toSC2/history.json`)
- Columns: Date, Model, Duration, Warnings, Errors, Output Path
- Click row to view that conversion's report
- [Clear History] button
- Export history as CSV

### Tab 4: Settings

```
┌─ Blender ────────────────────────────────────────────────────┐
│  Path: [C:\Program Files\Blender Foundation\Blender 4.4\...] │
│        [Auto-Detect]  [Browse...]                            │
│  Status: ✅ Blender 4.4.3 found with m3studio addon          │
│                                                              │
│  ☐ Download & manage Blender automatically (recommended)    │
└──────────────────────────────────────────────────────────────┘

┌─ Defaults ───────────────────────────────────────────────────┐
│  Default scale:      [0.05                                   ]│
│  Particle rate:      [1.0                                    ]│
│  Particle size:      [1.0                                    ]│
│  Output directory:   [./out\]               [Browse...]      │
│  Texture path:       [Assets\Textures\]                      │
│                                                              │
│  ☑ Auto-detect inverted alpha                               │
│  ☑ Auto-estimate scale                                      │
│  ☑ Fuzzy-match animation names                              │
│  ☑ Generate HTML conversion report                          │
│  ☑ Generate SC2 actor XML                                   │
│  ☐ Generate normal maps (slow)                              │
└──────────────────────────────────────────────────────────────┘

┌─ Theme ──────────────────────────────────────────────────────┐
│  ○ Dark    ○ Light    ○ System                               │
│  ○ Compact log    ○ Verbose log                              │
└──────────────────────────────────────────────────────────────┘
```

### Tab 5: About

- Version number + changelog link
- GitHub repo link
- Credits (m3studio by Solstice245)
- Check for updates button

---

## Threading Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────┐
│   Main Thread    │     │  Worker Threads   │     │  Subprocess │
│   (Qt GUI)       │     │  (QThreadPool)    │     │  (Blender)  │
├─────────────────┤     ├──────────────────┤     ├─────────────┤
│ • Render UI      │     │ • Parse MDX       │     │ • Build M3  │
│ • Handle input   │◄───►│ • Convert texture │     │ • Export .m3│
│ • Update progress│signal│ • Write config    │     │             │
│ • Show log       │ slot │ • Launch Blender  │────►│             │
│ • Display report │     │ • Validate output  │     │             │
└─────────────────┘     └──────────────────┘     └─────────────┘
```

**Key rules:**
- MDX parsing and texture conversion run on worker threads (CPU-bound, parallelizable)
- Blender runs as a subprocess (can't run inside Qt thread)
- Worker threads emit signals: `progress_update(model, pct)`, `log_message(level, text)`, `model_done(model, result)`, `model_error(model, error)`
- Main thread connects signals to UI updates
- Queue management: process models sequentially through Blender, textures in parallel
- Maximum 1 Blender instance at a time (can't parallelize Blender)

---

## Drag-and-Drop Zones

Two drop zones:
1. **Model queue panel** — accepts `.mdx` files and folders (adds to queue)
2. **Entire window** — accepts `.mdx` files (if dropped anywhere, adds to queue)

```python
class DropZoneWidget(QWidget):
    def __init__(self):
        self.setAcceptDrops(True)
    
    def dragEnterEvent(self, event):
        if event.mimeData().hasUrls():
            event.acceptProposedAction()
            self.setStyleSheet("border: 2px dashed #4CAF50;")
    
    def dropEvent(self, event):
        for url in event.mimeData().urls():
            path = url.toLocalFile()
            if os.path.isdir(path):
                self.add_folder(path)
            elif path.lower().endswith('.mdx'):
                self.add_model(path)
```

---

## Model Queue Data Model

```python
@dataclass
class ModelJob:
    mdx_path: str
    status: str          # 'queued', 'parsing', 'converting_textures', 
                          # 'blender', 'done', 'failed', 'cancelled'
    progress: int        # 0-100
    warnings: list[str]
    errors: list[str]
    output_path: str | None
    report_html: str | None
    duration_seconds: float | None
    texture_count: int
    animation_count: int
    
    # Per-model overrides (from UI)
    scale_override: float | None
    particle_rate_override: float | None
    output_dir_override: str | None
```

---

## Conversion Worker

```python
class ConversionWorker(QObject):
    """Runs on a QThread. Processes one model at a time."""
    
    progress = Signal(str, int)       # model_name, pct
    log = Signal(str, str)            # level, message
    done = Signal(str, ModelResult)   # model_name, result
    error = Signal(str, str)          # model_name, error_message
    
    def run(self, job: ModelJob):
        try:
            # Stage 1: Parse MDX (fast)
            self.log.emit("INFO", f"Parsing {job.mdx_path}")
            mdx_data = mdxlib.parse(job.mdx_path)
            
            # Stage 2: Diagnostics
            self.log.emit("INFO", "Running pre-flight checks...")
            report = diagnostics.run_checks(mdx_data, job.mdx_path)
            for w in report.warnings:
                self.log.emit("WARNING", w)
            
            # Stage 3: Convert textures (parallel internally)
            self.log.emit("INFO", f"Converting {report.texture_count} textures...")
            tex.convert_all(...)
            
            # Stage 4: Launch Blender (subprocess, blocking)
            self.log.emit("INFO", "Launching Blender...")
            # ... subprocess with progress parsing
            
            # Stage 5: Validate output
            self.log.emit("INFO", "Validating output...")
            
            self.done.emit(job.mdx_path, result)
        except Exception as e:
            self.error.emit(job.mdx_path, str(e))
```

---

## Bundle Strategy for PyInstaller

```
wc3toSC2.spec produces: wc3toSC2.exe (~55 MB compressed, ~150 MB extracted)

Contents:
  python312.dll          # Embedded Python runtime
  PySide6/               # Qt widgets, core, gui (~40 MB)
  numpy/                 # ~20 MB
  PIL/                   # ~5 MB
  mdx.py, blp.py, ...    # Our code
  build_m3.py            # Loose file for Blender
  assets/                # Icons, report template, presets
  
Does NOT bundle:
  Blender                # User provides or auto-download
  m3studio addon         # User installs once in Blender
```

---

## Implementation Priority for the GUI

| Step | What | Effort |
|---|---|---|
| 1 | Main window + tab bar + dark theme | Foundation |
| 2 | Model queue widget (QTreeWidget + drag-drop) | Core UX |
| 3 | Conversion worker thread + signals | Engine |
| 4 | Log panel with color coding | Visibility |
| 5 | Per-model progress bars | Feedback |
| 6 | Preview/report panel (HTML render) | Polish |
| 7 | Settings tab (Blender detection, defaults) | Config |
| 8 | History tab (persistent JSON log) | Data |
| 9 | Batch tab (folder picker, MPQ extract) | Power |
| 10 | Auto-update check | Maintenance |
