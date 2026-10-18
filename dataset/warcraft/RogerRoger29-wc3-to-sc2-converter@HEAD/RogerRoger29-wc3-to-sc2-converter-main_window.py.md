---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "main_window.py"
ref: HEAD
---

"""WC3 -> SC2 Model Converter - PySide6 GUI Application.
"""
from __future__ import annotations
import os, sys, json, time, threading
from typing import Optional, List, Dict, Any
from dataclasses import dataclass, field
from datetime import datetime

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from PySide6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QTabWidget, QTreeWidget, QTreeWidgetItem, QPushButton, QLabel,
    QPlainTextEdit, QProgressBar, QFileDialog, QCheckBox, QLineEdit,
    QGroupBox, QSplitter, QHeaderView, QStyleFactory, QScrollArea, QComboBox, QMenu, QFrame,
)
from PySide6.QtCore import Qt, QThread, Signal, QObject, QTimer, QSettings
from PySide6.QtGui import QColor, QFont, QTextCharFormat, QTextCursor, QPixmap, QPalette, QKeySequence, QShortcut

import mdx as mdxlib, diagnostics, healer, discovery, fuzzy_anims, actor_gen, preview
import auto_updater, blender_manager

SCRIPT_DIR = getattr(sys, '_MEIPASS', os.path.dirname(os.path.abspath(__file__)))

STYLE = """
QMainWindow{background:#1a1b26}
QPushButton{background:#3b4261;color:#c0caf5;border:1px solid #565f89;border-radius:8px;padding:8px 18px;font-size:12px}
QPushButton:hover{background:#565f89;border-color:#7aa2f7}
QPushButton:pressed{background:#24283b}
QPushButton:disabled{background:#24283b;color:#565f89;border-color:#3b4261}
QPushButton#cvt{background:qlineargradient(x1:0,y1:0,x2:1,y2:0,stop:0 #7aa2f7,stop:1 #9ece6a);color:#1a1b26;font-weight:bold;border:none;font-size:13px}
QPushButton#cvt:hover{background:qlineargradient(x1:0,y1:0,x2:1,y2:0,stop:0 #89b4fa,stop:1 #a6e3a1)}
QPushButton#cvt:disabled{background:#3b4261;color:#565f89}
QPushButton#setup{background:qlineargradient(x1:0,y1:0,x2:1,y2:0,stop:0 #e0af68,stop:1 #f9e2af);color:#1a1b26;font-weight:bold;padding:10px;font-size:12px;border:none}
QPushButton#setup:hover{background:qlineargradient(x1:0,y1:0,x2:1,y2:0,stop:0 #fab387,stop:1 #ffe0b0)}
QTreeWidget{border:1px solid #3b4261;border-radius:8px;outline:none;background:#24283b;alternate-background-color:#1f2335}
QTreeWidget::item{padding:5px 10px;min-height:26px;border-bottom:1px solid #3b4261}
QTreeWidget::item:selected{background:#3b4261;color:#c0caf5}
QTreeWidget::item:hover{background:#2f3348}
QHeaderView::section{background:#1f2335;color:#7aa2f7;border:none;padding:8px 10px;font-weight:bold;font-size:11px}
QProgressBar{border:1px solid #3b4261;border-radius:6px;background:#1f2335;text-align:center;color:#c0caf5;height:18px;font-size:11px}
QProgressBar::chunk{background:qlineargradient(x1:0,y1:0,x2:1,y2:0,stop:0 #7aa2f7,stop:1 #9ece6a);border-radius:5px}
QTabWidget::pane{border:1px solid #3b4261;border-radius:8px}
QTabBar::tab{background:#1f2335;color:#565f89;padding:12px 28px;margin-right:3px;border-top-left-radius:8px;border-top-right-radius:8px;font-size:12px}
QTabBar::tab:selected{background:#24283b;color:#7aa2f7;border-bottom:2px solid #e0af68}
QTabBar::tab:hover{color:#c0caf5}
QGroupBox{font-weight:bold;border:1px solid #3b4261;border-radius:10px;margin-top:16px;padding-top:20px;color:#a9b1d6}
QGroupBox::title{subcontrol-origin:margin;left:16px;padding:0 8px;color:#e0af68;font-size:13px}
QLineEdit,QComboBox{background:#1f2335;border:1px solid #3b4261;border-radius:6px;padding:8px 10px;color:#c0caf5;font-size:12px}
QLineEdit:focus,QComboBox:hover{border-color:#7aa2f7}
QComboBox::drop-down{border:none;width:24px}
QComboBox QAbstractItemView{background:#24283b;color:#c0caf5;border:1px solid #3b4261;border-radius:6px;selection-background-color:#3b4261}
QCheckBox{color:#c0caf5;spacing:10px;font-size:12px}
QCheckBox::indicator{width:18px;height:18px;border:2px solid #565f89;border-radius:4px;background:#1f2335}
QCheckBox::indicator:checked{background:#7aa2f7;border-color:#7aa2f7}
QScrollArea{border:none}
QScrollBar:vertical{background:#1a1b26;width:8px;border-radius:4px;margin:2px}
QScrollBar::handle:vertical{background:#3b4261;border-radius:4px;min-height:30px}
QScrollBar::handle:vertical:hover{background:#565f89}
QScrollBar::add-line:vertical,QScrollBar::sub-line:vertical{height:0}
QSplitter::handle{background:#3b4261;width:3px}
QToolTip{background:#24283b;color:#c0caf5;border:1px solid #565f89;border-radius:6px;padding:6px 10px;font-size:11px}
QMenu{background:#24283b;color:#c0caf5;border:1px solid #3b4261;border-radius:8px;padding:6px}
QMenu::item{padding:8px 28px;border-radius:4px}
QMenu::item:selected{background:#3b4261}
QMenu::separator{height:1px;background:#3b4261;margin:4px 8px}
QLabel#preview{background:#1f2335;border:1px solid #3b4261;border-radius:10px}
"""

@dataclass
class ModelJob:
    mdx_path: str = ""; model_name: str = ""; status: str = "ready"; progress: int = 0
    warnings: list = field(default_factory=list); errors: list = field(default_factory=list)
    output_path: str = ""; report_html: str = ""; preview_path: str = ""
    scale: float = 0.05; particle_rate: float = 1.0; mdx_data: dict = None
    texture_count: int = 0; animation_count: int = 0; start_time: float = 0.0
    _lock: object = field(default_factory=threading.Lock)
    def to_dict(self): return {"mdx_path":self.mdx_path,"model_name":self.model_name,"scale":self.scale,"particle_rate":self.particle_rate}
    @staticmethod
    def from_dict(d): return ModelJob(mdx_path=d.get("mdx_path",""),model_name=d.get("model_name",""),scale=d.get("scale",0.05),particle_rate=d.get("particle_rate",1.0))

class ConversionSignals(QObject):
    progress=Signal(str,int); log_message=Signal(str,str); status_change=Signal(str,str)
    job_done=Signal(str,bool,str); report_ready=Signal(str,str)

class ConversionWorker(QObject):
    def __init__(self,blender=""): super().__init__(); self.signals=ConversionSignals(); self._blender=blender; self._cancelled=False
    def cancel(self): self._cancelled=True
    def convert(self,job,mdx_data,tex_map,out_dir,build_cfg):
        if self._cancelled: return
        self.signals.status_change.emit(job.mdx_path,"checking")
        report=diagnostics.run_checks(mdx_data,job.mdx_path); job.warnings=[w.message for w in report.warnings]; job.errors=[e.message for e in report.errors]
        job.report_html=report.to_html(); self.signals.report_ready.emit(job.mdx_path,report.to_html())
        for w in report.warnings: self.signals.log_message.emit("WARNING",w.message)
        for e in report.errors: self.signals.log_message.emit("ERROR",e.message)
        if job.errors: job.status="failed"; self.signals.job_done.emit(job.mdx_path,False,""); return
        if report.auto_fixable_count>0:
            r=healer.apply_all_fixes(mdx_data,mdx_data.get("geosets",[]),mdx_data.get("bones",[]),mdx_data.get("helpers",[]),mdx_data.get("particles",[]))
            for f in r["fixes_applied"]: self.signals.log_message.emit("INFO","  OK "+f)
            mdx_data=r["data"]
        self.signals.status_change.emit(job.mdx_path,"textures"); self.signals.progress.emit(job.mdx_path,30)
        self.signals.status_change.emit(job.mdx_path,"blender"); self.signals.progress.emit(job.mdx_path,60)
        try:
            import subprocess
            cfg_path=os.path.join(out_dir,"_build_config.json"); json.dump(build_cfg,open(cfg_path,"w",encoding="utf-8"),indent=1)
            blender=self._blender or discovery.find_blender() or blender_manager.get_managed_blender_path() or "blender"
            build_py=os.path.join(SCRIPT_DIR,"build_m3.py")
            proc=subprocess.run([blender,"--background","--factory-startup","--python",build_py,"--",cfg_path],capture_output=True,text=True,timeout=300)
            self.signals.progress.emit(job.mdx_path,90)
            if "EXPORT_FAILED" in proc.stdout or proc.returncode!=0: self.signals.log_message.emit("ERROR","Blender failed:\n"+proc.stderr[-800:]); job.status="failed"; self.signals.job_done.emit(job.mdx_path,False,""); return
            for l in proc.stdout.splitlines():
                if any(k in l for k in ("anim '","mat[","particle[","EXPORT_OK","baked","BUILD_DONE")): self.signals.log_message.emit("INFO","  "+l.strip())
            self.signals.progress.emit(job.mdx_path,100); job.status="done"; job.output_path=build_cfg["out"]; self.signals.job_done.emit(job.mdx_path,True,build_cfg["out"])
        except Exception as ex: self.signals.log_message.emit("ERROR",str(ex)); job.status="failed"; self.signals.job_done.emit(job.mdx_path,False,"")

class MainWindow(QMainWindow):
    def __init__(self,auto_files=None):
        super().__init__(); self.setWindowTitle("WC3 to SC2 Converter"); self.resize(1280,850); self.setMinimumSize(960,620)
        self.jobs:Dict[str,ModelJob]={}; self._items:Dict[str,QTreeWidgetItem]={}
        self.settings=QSettings("wc3toSC2","Converter"); self.worker=None; self.worker_thread=None
        self._settings_widget=None
        self._load_all_settings(); self._setup_theme(); self._setup_ui()
        self._restore_session(); self._check_updates()
        if auto_files:
            for f in auto_files:
                if os.path.isdir(f): self._add_folder_path(f)
                elif f.lower().endswith(".mdx"): self._add_model_path(f)
            if self.jobs: self._update_title()

    def _setup_theme(self):
        app=QApplication.instance(); app.setStyle(QStyleFactory.create("Fusion")); app.setStyleSheet(STYLE)
        p=QPalette(); p.setColor(QPalette.Window,QColor(26,27,38)); p.setColor(QPalette.WindowText,QColor(192,202,245))
        p.setColor(QPalette.Base,QColor(31,35,53)); p.setColor(QPalette.Text,QColor(192,202,245))
        p.setColor(QPalette.Button,QColor(59,66,97)); p.setColor(QPalette.ButtonText,QColor(192,202,245))
        p.setColor(QPalette.Highlight,QColor(122,162,247)); p.setColor(QPalette.HighlightedText,QColor(26,27,38))
        app.setPalette(p)

    def _setup_ui(self):
        c=QWidget(); self.setCentralWidget(c); l=QVBoxLayout(c); l.setContentsMargins(10,10,10,10)
        self.tabs=QTabWidget()
        self.tabs.addTab(self._convert_tab(),"Convert")
        self.tabs.addTab(QLabel("<div style='color:#565f89;font-size:14px;padding:40px;text-align:center'>Click to load settings</div>"),"Settings")
        self.tabs.addTab(self._about_tab(),"About")
        self.tabs.currentChanged.connect(self._on_tab_change)
        l.addWidget(self.tabs); self.statusBar().showMessage("Ready")
        QShortcut(QKeySequence("Delete"),self,self._remove_selected)
        QShortcut(QKeySequence("Return"),self,self._start)

    def _on_tab_change(self,idx):
        if idx==1 and self._settings_widget is None:
            # Build settings on main thread (Qt widgets MUST be created on main thread)
            w=self._build_settings()
            self._settings_widget=w
            self.tabs.blockSignals(True)
            self.tabs.removeTab(1)
            self.tabs.insertTab(1,w,"Settings")
            self.tabs.blockSignals(False)

    def _convert_tab(self):
        tab=QWidget(); sp=QSplitter(Qt.Horizontal)
        left=QWidget(); ll=QVBoxLayout(left); ll.setContentsMargins(0,0,8,0)
        self.welcome=QFrame(); wl=QVBoxLayout(self.welcome)
        wl.addStretch()
        icon=QLabel("<div style='text-align:center;font-size:48px;color:#7aa2f7'>WC3 to SC2</div>")
        wl.addWidget(icon)
        wl.addWidget(QLabel("<div style='text-align:center;color:#a9b1d6;margin:12px 0'>Drop <b style='color:#e0af68'>.mdx</b> files here to begin</div>"))
        wl.addWidget(QLabel("<div style='text-align:center;color:#565f89;font-size:11px'>or use the buttons below to add models</div>")); wl.addStretch()
        self.welcome.setStyleSheet("background:#24283b;border:2px dashed #3b4261;border-radius:14px;min-height:160px")
        self.welcome.setVisible(True); ll.addWidget(self.welcome)
        self.queue_tree=QTreeWidget(); self.queue_tree.setHeaderLabels(["Model","Status","Progress","Warnings"])
        self.queue_tree.setDragDropMode(self.queue_tree.DragDropMode.DropOnly); self.queue_tree.setAcceptDrops(True)
        self.queue_tree.setContextMenuPolicy(Qt.CustomContextMenu)
        self.queue_tree.customContextMenuRequested.connect(self._context_menu)
        self.queue_tree.dragEnterEvent=lambda e: e.acceptProposedAction() if e.mimeData().hasUrls() else None
        self.queue_tree.dropEvent=self._drop; self.queue_tree.itemSelectionChanged.connect(self._on_select)
        for i,c in enumerate([QHeaderView.Stretch,*[QHeaderView.ResizeToContents]*3]):
            self.queue_tree.header().setSectionResizeMode(i,c)
        self.queue_tree.setVisible(False); ll.addWidget(self.queue_tree)
        br=QHBoxLayout()
        add=QPushButton("+ Add Models"); add.setToolTip("Select .mdx files to convert"); add.clicked.connect(self._add_models); br.addWidget(add)
        fd=QPushButton("Add Folder"); fd.setToolTip("Add all .mdx files from a folder"); fd.clicked.connect(self._add_folder); br.addWidget(fd)
        rm=QPushButton("Remove"); rm.setToolTip("Remove selected models (Delete key)"); rm.clicked.connect(self._remove_selected); br.addWidget(rm)
        cl=QPushButton("Clear"); cl.setToolTip("Clear completed models from the list"); cl.clicked.connect(self._clear_done); br.addWidget(cl); br.addStretch()
        self.cvt_btn=QPushButton("Convert All"); self.cvt_btn.setObjectName("cvt"); self.cvt_btn.setToolTip("Start converting all ready models (Enter)")
        self.cvt_btn.clicked.connect(self._start); br.addWidget(self.cvt_btn)
        self.stp_btn=QPushButton("Stop"); self.stp_btn.setEnabled(False); self.stp_btn.setToolTip("Stop after the current model finishes"); self.stp_btn.clicked.connect(self._cancel); br.addWidget(self.stp_btn); ll.addLayout(br); sp.addWidget(left)
        right=QWidget(); rl=QVBoxLayout(right); rl.setContentsMargins(8,0,0,0)
        self.preview_lbl=QLabel(); self.preview_lbl.setObjectName("preview"); self.preview_lbl.setAlignment(Qt.AlignCenter); self.preview_lbl.setMinimumHeight(200)
        self.preview_lbl.setText("<div style='padding:30px'><span style='color:#565f89;font-size:14px'>Model details appear here</span></div>"); rl.addWidget(self.preview_lbl,2)
        self.log_view=QPlainTextEdit(); self.log_view.setReadOnly(True); self.log_view.setFont(QFont("Consolas",10)); self.log_view.setMaximumBlockCount(5000)
        self.log_view.setStyleSheet("QPlainTextEdit{background:#1f2335;border:1px solid #3b4261;border-radius:10px;padding:6px}"); rl.addWidget(self.log_view,3)
        self.ov_prog=QProgressBar(); self.ov_prog.setVisible(False); self.ov_prog.setTextVisible(True); self.ov_prog.setFormat("%v of %m models"); rl.addWidget(self.ov_prog)
        sp.addWidget(right); sp.setSizes([550,650])
        ml=QVBoxLayout(tab); ml.addWidget(sp); return tab

    def _build_settings(self):
        sc=QScrollArea(); sc.setWidgetResizable(True); w=QWidget(); lo=QVBoxLayout(w); lo.setSpacing(8)
        gb=QGroupBox("Blender"); gl=QVBoxLayout(gb)
        hb=QHBoxLayout(); self._be=QLineEdit(self._bp); self._be.setPlaceholderText("Auto-detect"); hb.addWidget(QLabel("Path:")); hb.addWidget(self._be)
        a=QPushButton("Auto-Detect"); a.clicked.connect(self._auto_blender); hb.addWidget(a)
        b=QPushButton("Browse..."); b.clicked.connect(lambda: self._browse(self._be,"blender.exe","Blender (*.exe);;All (*)")); hb.addWidget(b); gl.addLayout(hb)
        self._oc_btn=QPushButton("One-Click Setup"); self._oc_btn.setObjectName("setup"); self._oc_btn.clicked.connect(self._oneclick); gl.addWidget(self._oc_btn)
        self._bs=QLabel("Not checked"); gl.addWidget(self._bs); lo.addWidget(gb)
        gb2=QGroupBox("Scale & Output"); gl2=QVBoxLayout(gb2)
        self._se=self._le("Scale:",str(self._scale),gl2); self._pe=self._le("Particle rate:",str(self._prate),gl2)
        self._pse=self._le("Particle size:",str(self._psize),gl2); self._oe=self._le("Output dir:",self._od,gl2); lo.addWidget(gb2)
        gb3=QGroupBox("Animation"); gl3=QVBoxLayout(gb3)
        self._fps_cb=self._cb("FPS mode:",["Auto-detect","30","60","15","10"],self._fps_mode,gl3)
        self._squad_cb=QCheckBox("Squad quaternion interpolation"); self._squad_cb.setChecked(self._squad); gl3.addWidget(self._squad_cb)
        self._kf_cb=QCheckBox("Keyframe reduction"); self._kf_cb.setChecked(self._kf_reduce); gl3.addWidget(self._kf_cb); lo.addWidget(gb3)
        gb4=QGroupBox("Mesh & Team Color"); gl4=QVBoxLayout(gb4)
        self._lod_cb=self._cb("LOD:",["None","LOD1 (50%)","LOD1+LOD2"],str(self._lod_level),gl4)
        self._tc_cb=self._cb("Team Color:",["TEAMEMIS","UV Mask","Off"],str(self._tc_mode),gl4); lo.addWidget(gb4)
        gb6=QGroupBox("Pipeline"); gl6=QVBoxLayout(gb6)
        for attr,label in [("_mt_cb","Multi-threaded textures"),("_cache_cb","MDX parse cache"),("_aa_cb","Auto-detect inverted alpha"),
                           ("_as_cb","Auto-estimate scale"),("_fa_cb","Fuzzy-match animations"),("_ax_cb","Generate actor XML"),("_gr_cb","Generate HTML report")]:
            cb=QCheckBox(label); cb.setChecked(getattr(self,attr.replace("_cb",""),True)); setattr(self,attr,cb); gl6.addWidget(cb)
        self._nm_cb=QCheckBox("Generate normal maps"); self._nm_cb.setChecked(self._normals); gl6.addWidget(self._nm_cb)
        self._ns_e=self._le("Normal strength:",str(self._nm_strength),gl6); lo.addWidget(gb6)
        sv=QPushButton("Save Settings"); sv.clicked.connect(self._save_all); lo.addWidget(sv); lo.addStretch()
        sc.setWidget(w); return sc

    def _le(self,label,default,layout):
        hd=QHBoxLayout(); hd.addWidget(QLabel(label)); e=QLineEdit(default); hd.addWidget(e); layout.addLayout(hd); return e
    def _cb(self,label,items,default,layout):
        hd=QHBoxLayout(); hd.addWidget(QLabel(label)); cb=QComboBox(); cb.addItems(items); cb.setCurrentText(default); hd.addWidget(cb); layout.addLayout(hd); return cb

    def _about_tab(self):
        t=QWidget(); l=QVBoxLayout(t); l.setAlignment(Qt.AlignCenter); l.setSpacing(8)
        l.addWidget(QLabel("<div style='font-size:28px;color:#7aa2f7;font-weight:bold'>WC3 to SC2</div>"))
        l.addWidget(QLabel("<div style='font-size:18px;color:#e0af68'>Model Converter</div>"))
        l.addWidget(QLabel("<div style='color:#a9b1d6;margin-top:12px'>Version 3.5</div>"))
        l.addWidget(QLabel("<div style='color:#565f89;margin-top:6px'>Convert Warcraft 3 models to StarCraft 2</div>"))
        l.addWidget(QLabel("<div style='margin-top:20px'><a href='https://github.com/RogerRoger29/wc3-to-sc2-converter' style='color:#7aa2f7'>github.com/RogerRoger29/wc3-to-sc2-converter</a></div>"))
        l.addWidget(QLabel("<div style='color:#565f89;margin-top:20px;font-size:11px'>Powered by m3studio (Solstice245) | MIT License</div>"))
        return t

    def _drop(self,e):
        for u in e.mimeData().urls():
            p=u.toLocalFile()
            if os.path.isdir(p): self._add_folder_path(p)
            elif p.lower().endswith(".mdx"): self._add_model_path(p)
        e.acceptProposedAction()

    def _add_models(self):
        fs,_=QFileDialog.getOpenFileNames(self,"Select MDX models","","MDX Files (*.mdx);;All Files (*)")
        for f in fs: self._add_model_path(f)

    def _add_folder(self):
        d=QFileDialog.getExistingDirectory(self,"Select folder")
        if d: self._add_folder_path(d)

    def _add_model_path(self,p,silent=False):
        if p in self.jobs: return
        n=os.path.splitext(os.path.basename(p))[0]
        j=ModelJob(mdx_path=p,model_name=n,scale=self._scale,particle_rate=self._prate)
        self.jobs[p]=j; self._add_item(j)
        if not silent: self._log("INFO","Added: "+n)
        self._vis(); self._save_session(); self._update_title()
        def _bg_parse():
            try:
                md=mdxlib.parse(p)
                with j._lock:
                    j.mdx_data=md
                    j.texture_count=len(md.get("textures",[]))
                    j.animation_count=len(md.get("sequences",[]))
                self._log("INFO",f"  Parsed {n}: {j.texture_count} textures, {j.animation_count} anims")
            except Exception as e: self._log("ERROR",f"Parse failed for {n}: {e}")
        threading.Thread(target=_bg_parse,daemon=True).start()

    def _add_folder_path(self,d):
        c=0
        for r,_,fs in os.walk(d):
            for f in fs:
                if f.lower().endswith(".mdx"): self._add_model_path(os.path.join(r,f),silent=True); c+=1
        self._log("INFO",f"Found {c} model(s) in folder")

    def _add_item(self,j):
        it=QTreeWidgetItem(self.queue_tree); it.setText(0,j.model_name); it.setText(1,"Ready"); it.setText(2,u"\u2014"); it.setText(3,u"\u2014")
        it.setData(0,Qt.UserRole,j.mdx_path); self._items[j.mdx_path]=it

    def _find_item(self,p):
        return self._items.get(p)

    def _vis(self):
        h=self.queue_tree.topLevelItemCount()>0; self.welcome.setVisible(not h); self.queue_tree.setVisible(h)

    def _update_title(self):
        q=sum(1 for j in self.jobs.values() if j.status=="ready"); d=sum(1 for j in self.jobs.values() if j.status=="done")
        f=sum(1 for j in self.jobs.values() if j.status=="failed")
        self.setWindowTitle(f"WC3 to SC2   [ Ready: {q}   Done: {d}"+(f"   Failed: {f}]" if f else "]"))

    def _on_select(self):
        its=self.queue_tree.selectedItems()
        if not its: return
        p=its[0].data(0,Qt.UserRole); j=self.jobs.get(p)
        if not j: return
        if j.preview_path and os.path.exists(j.preview_path):
            self.preview_lbl.setPixmap(QPixmap(j.preview_path).scaled(480,480,Qt.KeepAspectRatio,Qt.SmoothTransformation))
        else:
            with j._lock:
                tc=j.texture_count; ac=j.animation_count; has_data=j.mdx_data is not None
            if has_data:
                self.preview_lbl.setText(f"<div style='padding:24px'><div style='font-size:18px;color:#e0af68;font-weight:bold'>{j.model_name}</div><div style='color:#c0caf5;margin:8px 0'>{tc} textures &bull; {ac} animations</div><div style='color:#a9b1d6'>Scale: {j.scale} &bull; Status: {j.status.title()}</div></div>")
            else:
                self.preview_lbl.setText(f"<div style='padding:24px'><div style='font-size:18px;color:#e0af68;font-weight:bold'>{j.model_name}</div><div style='color:#a9b1d6;margin:8px 0'>Parsing model data...</div></div>")

    def _remove_selected(self):
        removed=0
        for it in self.queue_tree.selectedItems():
            p=it.data(0,Qt.UserRole)
            if p in self.jobs:
                self.jobs.pop(p,None); self._items.pop(p,None); self.queue_tree.takeTopLevelItem(self.queue_tree.indexOfTopLevelItem(it)); removed+=1
        if removed: self._log("INFO",f"Removed {removed} model(s)"); self._vis(); self._save_session(); self._update_title()

    def _context_menu(self,pos):
        it=self.queue_tree.itemAt(pos)
        if not it: return
        p=it.data(0,Qt.UserRole); j=self.jobs.get(p)
        if not j: return
        menu=QMenu(self)
        if j.status=="ready": menu.addAction("Remove from queue").triggered.connect(lambda: self._remove_one(p))
        if j.status=="done" and j.output_path and os.path.exists(j.output_path):
            menu.addAction("Open output folder").triggered.connect(lambda: os.startfile(os.path.dirname(j.output_path)))
        menu.exec(self.queue_tree.viewport().mapToGlobal(pos))

    def _remove_one(self,p):
        if p in self.jobs:
            self.jobs.pop(p,None); it=self._items.pop(p,None)
            if it: self.queue_tree.takeTopLevelItem(self.queue_tree.indexOfTopLevelItem(it))
            self._vis(); self._save_session(); self._update_title()

    def _clear_done(self):
        for i in range(self.queue_tree.topLevelItemCount()-1,-1,-1):
            it=self.queue_tree.topLevelItem(i); p=it.data(0,Qt.UserRole)
            if p in self.jobs and self.jobs[p].status in ("done","failed"): self.jobs.pop(p,None); self._items.pop(p,None); self.queue_tree.takeTopLevelItem(i)
        self._vis(); self._update_title()

    def _start(self):
        q=[j for j in self.jobs.values() if j.status=="ready"]
        if not q: self._log("WARNING","No models ready. Add models first."); return
        self.cvt_btn.setEnabled(False); self.stp_btn.setEnabled(True); self.ov_prog.setVisible(True); self.ov_prog.setMaximum(len(q)); self.ov_prog.setValue(0); self._next()

    def _next(self):
        q=[j for j in self.jobs.values() if j.status=="ready"]
        if not q: self._done(); return
        j=q[0]; j.start_time=time.time(); j.status="checking"; self._upd(j); self._log("INFO","Starting: "+j.model_name); self.statusBar().showMessage(f"Converting {j.model_name}..."); self._update_title()
        waited=0
        while True:
            with j._lock:
                if j.mdx_data is not None: break
            if waited>=30: self._log("ERROR","Parse incomplete"); j.status="failed"; self._upd(j); self._next(); return
            time.sleep(0.1); waited+=0.1
        md=j.mdx_data
        if self._auto_scale: s,c=discovery.estimate_scale(md); j.scale=s; self._log("INFO",f"Scale: {s} ({c})")
        od=self._od or os.path.join(os.path.dirname(j.mdx_path),"out"); os.makedirs(od,exist_ok=True)
        bc={"mdx":j.mdx_path,"out":os.path.join(od,j.model_name+".m3"),"model_name":j.model_name,"scale":j.scale,
            "asset_texture_dir":"Assets\\Textures\\","textures":{},"anim_names":{},
            "features":{"animations":True,"attachments":True,"particles":True,"hittest":True,"camera":True},
            "particle_rate_scale":j.particle_rate,"team_color":self._tc_mode!=2,
            "fps_mode":self._fps_mode,"squad":self._squad,"keyframe_reduce":self._kf_reduce,
            "lod_level":self._lod_level,"tc_mask":self._tc_mode==1,"gen_normals":self._normals,
            "normal_strength":self._nm_strength}
        if self._fuzzy_anims and md.get("sequences"): bc["anim_names"]=fuzzy_anims.build_anim_map([s["name"] for s in md["sequences"]])
        tm={}
        for i,t in enumerate(md.get("textures",[])):
            if t.get("replaceableId") in (1,2) or not t.get("path"): continue
            tm[i]=os.path.splitext(os.path.basename(t["path"]))[0]+".dds"
        bc["textures"]={str(k):v for k,v in tm.items()}
        bl=self._get_blender()
        if bl and os.path.exists(bl): threading.Thread(target=lambda: setattr(j,"preview_path",preview.render_preview(j.mdx_path,bl) or ""),daemon=True).start()
        self.worker=ConversionWorker(bl); self.worker_thread=QThread(); self.worker.moveToThread(self.worker_thread)
        self.worker_thread.started.connect(lambda: self.worker.convert(j,md,tm,od,bc))
        self.worker.signals.progress.connect(self._op); self.worker.signals.log_message.connect(self._log)
        self.worker.signals.status_change.connect(self._os); self.worker.signals.job_done.connect(self._oj)
        self.worker.signals.report_ready.connect(self._or); self.worker_thread.finished.connect(self.worker_thread.deleteLater)
        self.worker_thread.start()

    def _cancel(self):
        if self.worker: self.worker.cancel()
        self._log("WARNING","Stopping after current model"); self.stp_btn.setEnabled(False)

    def _done(self):
        self.cvt_btn.setEnabled(True); self.stp_btn.setEnabled(False); self.ov_prog.setVisible(False)
        d=sum(1 for j in self.jobs.values() if j.status=="done"); f=sum(1 for j in self.jobs.values() if j.status=="failed")
        self._log("SUCCESS",f"Done: {d} succeeded, {f} failed"); self.statusBar().showMessage(f"Complete: {d} ok"+(f", {f} failed" if f else ""),5000); self._save_session(); self._update_title()

    def _op(self,p,pc):
        if p in self.jobs: self.jobs[p].progress=pc; self._upd(self.jobs[p])
        self.ov_prog.setValue(sum(1 for j in self.jobs.values() if j.status=="done")+(1 if pc<100 else 0))

    def _os(self,p,s):
        if p in self.jobs: self.jobs[p].status=s; self._upd(self.jobs[p])

    def _oj(self,p,ok,out):
        j=self.jobs.get(p)
        if j:
            d=time.time()-j.start_time if j.start_time else 0; self._log("SUCCESS" if ok else "ERROR",f"{'OK' if ok else 'FAIL'} {j.model_name} ({d:.1f}s)")
            if ok and self._gen_actor:
                try:
                    x=actor_gen.generate_actor_xml(j.model_name,f"Assets\\Textures\\{j.model_name}.m3",scale=j.scale)
                    with open(os.path.join(os.path.dirname(out),j.model_name+"_actor.xml"),"w") as fh: fh.write(x)
                except: pass
            if self._gen_report and j.report_html:
                try: open(os.path.join(os.path.dirname(out or j.mdx_path),j.model_name+"_report.html"),"w").write(j.report_html)
                except: pass
        self.worker_thread=None; self.worker=None; self._update_title(); self._next()

    def _or(self,p,html):
        if p in self.jobs: self.jobs[p].report_html=html

    def _log(self,lv,msg):
        ts=datetime.now().strftime("%H:%M:%S")
        f=QTextCharFormat(); f.setForeground({"ERROR":QColor("#f7768e"),"WARNING":QColor("#e0af68"),"SUCCESS":QColor("#9ece6a"),"INFO":QColor("#c0caf5")}.get(lv,QColor("#a9b1d6")))
        c=self.log_view.textCursor(); was_end=c.atEnd(); c.movePosition(QTextCursor.End); c.insertText(f"{ts}  {lv:<7} {msg}\n",f)
        if was_end: self.log_view.setTextCursor(c); self.log_view.ensureCursorVisible()

    def _upd(self,j):
        it=self._find_item(j.mdx_path)
        if it:
            st={"ready":"Ready","checking":"Checking","textures":"Textures","blender":"Blender","done":"Complete","failed":"Failed"}
            it.setText(1,st.get(j.status,j.status.title())); it.setText(2,f"{j.progress}%" if j.progress else u"\u2014")
            it.setText(3,str(len(j.warnings)) if j.warnings else u"\u2014")

    def _save_session(self):
        self.settings.setValue("session",json.dumps([j.to_dict() for j in self.jobs.values() if j.status=="ready"]))

    def _restore_session(self):
        try:
            d=json.loads(self.settings.value("session","[]"))
            for x in d:
                if os.path.exists(x.get("mdx_path","")): self._add_model_path(x["mdx_path"],silent=True)
            if self.jobs: self._log("INFO",f"Restored {len(self.jobs)} model(s)")
        except: pass

    def _check_updates(self):
        threading.Thread(target=lambda: (lambda u: self._log("INFO",f"Update available: {u['version']}") if u else None)(auto_updater.check_for_update()),daemon=True).start()

    def _load_all_settings(self):
        s=self.settings
        self._bp=s.value("blender_path",""); self._scale=float(s.value("default_scale","0.05"))
        self._prate=float(s.value("default_particle_rate","1.0")); self._psize=float(s.value("default_particle_size","1.0"))
        self._od=s.value("output_dir","./out"); self._fps_mode=s.value("fps_mode","Auto-detect")
        self._squad=s.value("squad","true")=="true"; self._kf_reduce=s.value("keyframe_reduce","true")=="true"
        self._lod_level=s.value("lod_level","0"); self._tc_mode=s.value("tc_mode","0")
        self._normals=s.value("gen_normals","false")=="true"; self._nm_strength=float(s.value("normal_strength","1.0"))
        self._multi_tex=s.value("multi_tex","true")=="true"; self._mdx_cache=s.value("mdx_cache","true")=="true"
        self._auto_alpha=s.value("auto_alpha","true")=="true"; self._auto_scale=s.value("auto_scale","true")=="true"
        self._fuzzy_anims=s.value("fuzzy_anims","true")=="true"; self._gen_actor=s.value("gen_actor","true")=="true"
        self._gen_report=s.value("gen_report","true")=="true"

    def _save_all(self):
        s=self.settings
        for k,a in [("blender_path","_be"),("default_scale","_se"),("default_particle_rate","_pe"),
                     ("default_particle_size","_pse"),("output_dir","_oe")]: s.setValue(k,getattr(self,a).text().strip())
        s.setValue("fps_mode",self._fps_cb.currentText()); s.setValue("squad",str(self._squad_cb.isChecked()).lower())
        s.setValue("keyframe_reduce",str(self._kf_cb.isChecked()).lower()); s.setValue("lod_level",self._lod_cb.currentText())
        s.setValue("tc_mode",self._tc_cb.currentText()); s.setValue("gen_normals",str(self._nm_cb.isChecked()).lower())
        s.setValue("normal_strength",self._ns_e.text().strip())
        for k,a in [("multi_tex","_mt_cb"),("mdx_cache","_cache_cb"),("auto_alpha","_aa_cb"),
                     ("auto_scale","_as_cb"),("fuzzy_anims","_fa_cb"),("gen_actor","_ax_cb"),("gen_report","_gr_cb")]:
            s.setValue(k,str(getattr(self,a).isChecked()).lower())
        self._load_all_settings(); self._log("INFO","Settings saved"); self.statusBar().showMessage("Settings saved",3000)

    def _auto_blender(self):
        p=discovery.find_blender() or blender_manager.get_managed_blender_path()
        if p and os.path.exists(p): self._be.setText(p); self._bs.setText(f"Found: {p}")
        else: self._bs.setText("Not found")

    def _browse(self,ed,title,filt):
        p,_=QFileDialog.getOpenFileName(self,title,"",filt)
        if p: ed.setText(p)

    def _oneclick(self):
        self._log("INFO","Starting one-click setup..."); self._oc_btn.setEnabled(False); self._oc_btn.setText("Downloading Blender (~300MB)...")
        import concurrent.futures
        def _done(f):
            exe=f.result()
            if exe: self._be.setText(str(exe)); self._bs.setText(f"Ready: {exe}"); self._log("SUCCESS","Blender + m3studio installed")
            else: self._bs.setText("Failed"); self._log("ERROR","Setup failed")
            self._oc_btn.setEnabled(True); self._oc_btn.setText("One-Click Setup")
        with concurrent.futures.ThreadPoolExecutor(1) as ex:
            ex.submit(lambda: blender_manager.ensure_blender_ready()).add_done_callback(_done)

    def _get_blender(self): return self._be.text().strip() or discovery.find_blender() or blender_manager.get_managed_blender_path() or "blender"

def main():
    app=QApplication(sys.argv); app.setApplicationName("wc3toSC2")
    if "--cli" in sys.argv: sys.argv.remove("--cli"); import convert; convert.main(); return
    af=[a for a in sys.argv[1:] if not a.startswith("-")]
    w=MainWindow(auto_files=af if af else None); w.show()
    if af and "--silent" in sys.argv and w.jobs: QTimer.singleShot(500,w._start)
    sys.exit(app.exec())

if __name__=="__main__": main()
