---
type: vendor-doc
source_repo: "https://github.com/nightbert/BLP-Viewer-PNG-Converter"
source_path: "blp2png.py"
ref: HEAD
---

#!/usr/bin/env python3
import os
import queue
import struct
import threading
import tkinter as tk
from typing import Optional
from tkinter import filedialog, messagebox, ttk

from PIL import Image, ImageTk, __version__ as PIL_VERSION


APP_TITLE = "BLP Viewer & PNG Converter"


class BlpApp:
    def __init__(self, root: tk.Tk) -> None:
        self.root = root
        self.root.title(APP_TITLE)
        self.root.geometry("1100x700")
        self.root.minsize(900, 600)

        self.source_dir = ""
        self.dest_dir = ""
        self.file_list: list[str] = []
        self.list_meta: list[dict[str, str]] = []

        self.current_pil: Optional[Image.Image] = None
        self.current_photo: Optional[ImageTk.PhotoImage] = None
        self.preview_bg = "#1b1b1b"
        self.placeholder_message = ""

        self.queue: queue.Queue = queue.Queue()
        self.total_to_convert = 0
        self.conversion_thread: Optional[threading.Thread] = None

        self._build_ui()

    def _build_ui(self) -> None:
        top = ttk.Frame(self.root, padding=10)
        top.pack(fill="x")

        ttk.Label(top, text="Source folder:").pack(side="left")
        self.source_var = tk.StringVar(value="")
        self.source_entry = ttk.Entry(top, textvariable=self.source_var)
        self.source_entry.pack(side="left", fill="x", expand=True, padx=(8, 8))
        self.source_entry.bind("<Return>", self._apply_source_entry)
        self.source_entry.bind("<FocusOut>", self._apply_source_entry)
        self.pick_source_btn = ttk.Button(top, text="📁 Choose folder", command=self.select_source_dir)
        self.pick_source_btn.pack(side="left")

        main = ttk.Panedwindow(self.root, orient="horizontal")
        main.pack(fill="both", expand=True, padx=10, pady=(0, 10))

        left = ttk.Frame(main, padding=(0, 0, 10, 0))
        main.add(left, weight=1)

        ttk.Label(left, text="BLP files:").pack(anchor="w")
        list_frame = ttk.Frame(left)
        list_frame.pack(fill="both", expand=True, pady=(4, 0))

        self.listbox = tk.Listbox(list_frame, exportselection=False)
        self.listbox.pack(side="left", fill="both", expand=True)
        self.listbox.bind("<<ListboxSelect>>", self.on_select)

        list_scroll = ttk.Scrollbar(list_frame, orient="vertical", command=self.listbox.yview)
        list_scroll.pack(side="right", fill="y")
        self.listbox.configure(yscrollcommand=list_scroll.set)

        right = ttk.Frame(main)
        main.add(right, weight=2)

        ttk.Label(right, text="Preview").pack(anchor="w")
        self.preview_canvas = tk.Canvas(
            right, background=self.preview_bg, highlightthickness=1, highlightbackground="#444"
        )
        self.preview_canvas.pack(fill="both", expand=True, pady=(4, 6))
        self.preview_canvas.bind("<Configure>", lambda _e: self.render_preview())

        self.info_var = tk.StringVar(value="No image loaded.")
        ttk.Label(right, textvariable=self.info_var).pack(anchor="w")

        bg_frame = ttk.Frame(right)
        bg_frame.pack(anchor="w", pady=(6, 0))
        ttk.Label(bg_frame, text="Background:").pack(side="left")

        self._add_bg_button(bg_frame, "#1b1b1b")
        self._add_bg_button(bg_frame, "#ff00ff")
        self._add_bg_button(bg_frame, "#00aa00")
        self._add_bg_button(bg_frame, "#0066ff")
        self._add_bg_button(bg_frame, "#ffffff")

        self.custom_color_var = tk.StringVar(value="#1b1b1b")
        self.custom_color_entry = ttk.Entry(bg_frame, textvariable=self.custom_color_var, width=10)
        self.custom_color_entry.pack(side="left", padx=(8, 0))
        self.custom_color_entry.bind("<Return>", lambda _e: self._apply_custom_bg())
        self.custom_color_btn = ttk.Button(bg_frame, text="Custom", command=self._apply_custom_bg)
        self.custom_color_btn.pack(side="left", padx=(6, 0))

        bottom = ttk.Frame(self.root, padding=(10, 0, 10, 10))
        bottom.pack(fill="x")

        ttk.Label(bottom, text="Destination folder:").pack(side="left")
        self.dest_var = tk.StringVar(value="")
        self.dest_entry = ttk.Entry(bottom, textvariable=self.dest_var)
        self.dest_entry.pack(side="left", fill="x", expand=True, padx=(8, 8))
        self.dest_entry.bind("<Return>", self._apply_dest_entry)
        self.dest_entry.bind("<FocusOut>", self._apply_dest_entry)
        self.pick_dest_btn = ttk.Button(bottom, text="📁 Choose destination", command=self.select_dest_dir)
        self.pick_dest_btn.pack(side="left")

        actions = ttk.Frame(self.root, padding=(10, 0, 10, 10))
        actions.pack(fill="x")

        self.convert_one_btn = ttk.Button(
            actions, text="Convert selected file", command=self.convert_selected
        )
        self.convert_one_btn.pack(side="left")
        self.convert_dir_btn = ttk.Button(
            actions, text="Convert selected folder", command=self.convert_selected_dir
        )
        self.convert_dir_btn.pack(side="left", padx=(8, 0))
        self.convert_all_btn = ttk.Button(
            actions, text="Convert all files", command=self.convert_all
        )
        self.convert_all_btn.pack(side="left", padx=(8, 0))

        self.progress = ttk.Progressbar(actions, mode="determinate")
        self.progress.pack(side="left", padx=(12, 8), fill="x", expand=True)

        self.status_var = tk.StringVar(value="")
        ttk.Label(actions, textvariable=self.status_var).pack(side="left")

    def select_source_dir(self) -> None:
        path = filedialog.askdirectory(title="Choose source folder")
        if not path:
            return
        self._set_source_dir(path)

    def select_dest_dir(self) -> None:
        path = filedialog.askdirectory(title="Choose destination folder")
        if not path:
            return
        self._set_dest_dir(path)


    def _apply_source_entry(self, _event: Optional[tk.Event] = None) -> None:
        raw = self.source_var.get().strip()
        if not raw:
            return
        path = self._normalize_dir(raw)
        if not os.path.isdir(path):
            messagebox.showerror(APP_TITLE, "Source folder does not exist.")
            self.source_var.set(self.source_dir)
            return
        if path != self.source_dir:
            self._set_source_dir(path)

    def _apply_dest_entry(self, _event: Optional[tk.Event] = None) -> None:
        raw = self.dest_var.get().strip()
        if not raw:
            self.dest_dir = ""
            self.dest_var.set("")
            return
        path = self._normalize_dir(raw)
        if not os.path.isdir(path):
            messagebox.showerror(APP_TITLE, "Destination folder does not exist.")
            self.dest_var.set(self.dest_dir)
            return
        self._set_dest_dir(path)

    def _normalize_dir(self, path: str) -> str:
        return os.path.abspath(os.path.expanduser(path))

    def _set_source_dir(self, path: str) -> None:
        self.source_dir = path
        self.source_var.set(path)
        self._load_file_list()

    def _set_dest_dir(self, path: str) -> None:
        self.dest_dir = path
        self.dest_var.set(path)

    def _load_file_list(self) -> None:
        self.file_list = []
        self.list_meta = []
        if not self.source_dir:
            return
        for root_dir, _dirs, files in os.walk(self.source_dir):
            for name in files:
                if name.lower().endswith(".blp"):
                    full = os.path.join(root_dir, name)
                    rel = os.path.relpath(full, self.source_dir)
                    self.file_list.append(rel)
        self.file_list.sort()
        self._populate_list()

        count = len(self.file_list)
        self.status_var.set(f"{count} file(s) found")
        if count == 0:
            self._clear_preview("No BLP files found.")
        else:
            self._clear_preview("Select a file to preview.")

    def on_select(self, _event: tk.Event) -> None:
        if not self.listbox.curselection():
            return
        idx = self.listbox.curselection()[0]
        if idx >= len(self.list_meta):
            return
        meta = self.list_meta[idx]
        kind = meta.get("kind")
        rel_path = meta.get("relpath", "")
        if kind == "file" and rel_path:
            full_path = os.path.join(self.source_dir, rel_path)
            self._load_preview(full_path, rel_path)
        elif kind == "dir":
            label = rel_path if rel_path else "Root folder"
            self._clear_preview(f"Folder selected: {label}")

    def _populate_list(self) -> None:
        self.listbox.delete(0, tk.END)
        self.list_meta = []
        if not self.source_dir:
            return
        dir_map: dict[str, list[str]] = {}
        for rel_path in self.file_list:
            dir_rel = os.path.dirname(rel_path)
            filename = os.path.basename(rel_path)
            dir_map.setdefault(dir_rel, []).append(filename)

        for dir_rel in sorted(dir_map.keys()):
            label = dir_rel if dir_rel else "Root folder"
            label = f"📁 {label}"
            self.listbox.insert(tk.END, label)
            self.list_meta.append({"kind": "dir", "relpath": dir_rel})
            for filename in sorted(dir_map[dir_rel]):
                self.listbox.insert(tk.END, f"  {filename}")
                rel_file = os.path.join(dir_rel, filename) if dir_rel else filename
                self.list_meta.append({"kind": "file", "relpath": rel_file})

    def _load_preview(self, full_path: str, rel_path: str) -> None:
        try:
            img = self._open_blp(full_path)
            self.current_pil = img
            info = self._format_info(rel_path, img)
            self.info_var.set(info)
        except Exception as exc:  # pragma: no cover - user-facing IO errors
            self.current_pil = None
            self.info_var.set(f"Load error: {rel_path}")
            self._clear_preview(self._format_error_message(full_path, exc))
            return

        self.render_preview()

    def _format_info(self, rel_path: str, img: Image.Image) -> str:
        try:
            size_bytes = os.path.getsize(os.path.join(self.source_dir, rel_path))
            size_kb = size_bytes / 1024
            size_txt = f"{size_kb:.1f} KB"
        except OSError:
            size_txt = "?"
        return (
            f"File: {rel_path} | Size: {img.size[0]}x{img.size[1]} | "
            f"Mode: {img.mode} | File size: {size_txt}"
        )

    def render_preview(self) -> None:
        if self.current_pil is None:
            return
        canvas_w = self.preview_canvas.winfo_width()
        canvas_h = self.preview_canvas.winfo_height()
        if canvas_w < 20 or canvas_h < 20:
            return

        img = self.current_pil.copy()
        img.thumbnail((canvas_w - 20, canvas_h - 20), Image.LANCZOS)
        self.current_photo = ImageTk.PhotoImage(img)

        self.preview_canvas.delete("all")
        self.preview_canvas.create_image(canvas_w // 2, canvas_h // 2, image=self.current_photo, anchor="center")

    def _clear_preview(self, message: str) -> None:
        self.placeholder_message = message
        self.current_pil = None
        self.current_photo = None
        self.preview_canvas.delete("all")
        canvas_w = self.preview_canvas.winfo_width()
        canvas_h = self.preview_canvas.winfo_height()
        if canvas_w < 10 or canvas_h < 10:
            self.preview_canvas.after(50, lambda: self._clear_preview(message))
            return
        self.preview_canvas.create_text(
            canvas_w // 2,
            canvas_h // 2,
            text=message,
            fill=self._contrast_text(self.preview_bg),
            width=max(200, canvas_w - 40),
            justify="center",
        )

    def convert_selected(self) -> None:
        if not self.listbox.curselection():
            messagebox.showinfo(APP_TITLE, "Please select a file.")
            return
        idx = self.listbox.curselection()[0]
        if idx >= len(self.list_meta):
            messagebox.showinfo(APP_TITLE, "Please select a file.")
            return
        meta = self.list_meta[idx]
        if meta.get("kind") != "file":
            messagebox.showinfo(APP_TITLE, "Please select a file.")
            return
        rel_path = meta.get("relpath", "")
        if not rel_path:
            messagebox.showinfo(APP_TITLE, "Please select a file.")
            return
        src_path = os.path.join(self.source_dir, rel_path)

        if self.dest_dir:
            dest_path = os.path.join(self.dest_dir, os.path.splitext(rel_path)[0] + ".png")
        else:
            default_name = os.path.splitext(os.path.basename(rel_path))[0] + ".png"
            dest_path = filedialog.asksaveasfilename(
                title="Save PNG",
                defaultextension=".png",
                initialfile=default_name,
                filetypes=[("PNG", "*.png")],
            )
        if not dest_path:
            return

        os.makedirs(os.path.dirname(dest_path), exist_ok=True)
        try:
            self._convert_file(src_path, dest_path)
        except Exception as exc:
            messagebox.showerror(APP_TITLE, f"Conversion error:\n{self._format_error_message(src_path, exc)}")
            return

        messagebox.showinfo(APP_TITLE, f"PNG saved:\n{dest_path}")

    def convert_all(self) -> None:
        if not self.file_list:
            messagebox.showinfo(APP_TITLE, "No BLP files found.")
            return
        if self.conversion_thread and self.conversion_thread.is_alive():
            messagebox.showinfo(APP_TITLE, "Conversion is already running.")
            return

        if not self.dest_dir:
            self.select_dest_dir()
        if not self.dest_dir:
            return
        self._start_batch(self.file_list)

    def convert_selected_dir(self) -> None:
        if not self.listbox.curselection():
            messagebox.showinfo(APP_TITLE, "Please select a folder.")
            return
        idx = self.listbox.curselection()[0]
        if idx >= len(self.list_meta):
            messagebox.showinfo(APP_TITLE, "Please select a folder.")
            return
        meta = self.list_meta[idx]
        if meta.get("kind") != "dir":
            messagebox.showinfo(APP_TITLE, "Please select a folder.")
            return
        rel_dir = meta.get("relpath", "")
        if rel_dir:
            prefix = rel_dir + os.sep
            files = [p for p in self.file_list if p.startswith(prefix)]
        else:
            files = list(self.file_list)
        if not files:
            messagebox.showinfo(APP_TITLE, "No BLP files found in this folder.")
            return
        if self.conversion_thread and self.conversion_thread.is_alive():
            messagebox.showinfo(APP_TITLE, "Conversion is already running.")
            return
        if not self.dest_dir:
            self.select_dest_dir()
        if not self.dest_dir:
            return
        self._start_batch(files)

    def _start_batch(self, files: list[str]) -> None:
        self.total_to_convert = len(files)
        self.progress.configure(maximum=self.total_to_convert, value=0)
        self.status_var.set(f"0/{self.total_to_convert}")

        self._set_busy(True)
        self.conversion_thread = threading.Thread(
            target=self._convert_all_worker, args=(files,), daemon=True
        )
        self.conversion_thread.start()
        self.root.after(100, self._process_queue)

    def _convert_all_worker(self, files: list[str]) -> None:
        errors: list[str] = []
        for idx, rel_path in enumerate(files, start=1):
            src_path = os.path.join(self.source_dir, rel_path)
            dest_path = os.path.join(self.dest_dir, os.path.splitext(rel_path)[0] + ".png")
            os.makedirs(os.path.dirname(dest_path), exist_ok=True)
            try:
                self._convert_file(src_path, dest_path)
            except Exception as exc:  # pragma: no cover - user data issue
                errors.append(f"{rel_path}: {self._format_error_message(src_path, exc)}")
            self.queue.put(("progress", idx))
        self.queue.put(("done", errors))

    def _process_queue(self) -> None:
        done = False
        errors: list[str] = []
        while not self.queue.empty():
            msg = self.queue.get()
            if msg[0] == "progress":
                current = msg[1]
                self.progress.configure(value=current)
                self.status_var.set(f"{current}/{self.total_to_convert}")
            elif msg[0] == "done":
                done = True
                errors = msg[1]

        if done:
            self._set_busy(False)
            if errors:
                preview = "\n".join(errors[:5])
                more = ""
                if len(errors) > 5:
                    more = f"\n... and {len(errors) - 5} more."
                messagebox.showwarning(
                    APP_TITLE, f"Finished with errors ({len(errors)}):\n{preview}{more}"
                )
            else:
                messagebox.showinfo(APP_TITLE, "Conversion completed.")
            return

        self.root.after(100, self._process_queue)

    def _set_busy(self, busy: bool) -> None:
        state = "disabled" if busy else "normal"
        for btn in (
            self.pick_source_btn,
            self.pick_dest_btn,
            self.convert_one_btn,
            self.convert_dir_btn,
            self.convert_all_btn,
        ):
            btn.configure(state=state)

    def _convert_file(self, src_path: str, dest_path: str) -> None:
        img = self._open_blp(src_path)
        img.save(dest_path, "PNG")

    def _open_blp(self, path: str) -> Image.Image:
        try:
            with Image.open(path) as img:
                img.load()
                return img.copy()
        except Exception:
            custom = self._try_decode_encoding3(path)
            if custom is not None:
                return custom
            raise

    def _try_decode_encoding3(self, path: str) -> Optional[Image.Image]:
        header = self._read_blp2_header(path)
        if not header or header["compression"] != 1 or header["encoding"] != 3:
            return None
        width = header["width"]
        height = header["height"]
        offset = header["offsets"][0]
        size = header["sizes"][0] or (width * height * 4)
        if offset <= 0 or width == 0 or height == 0:
            return None
        try:
            with open(path, "rb") as handle:
                handle.seek(offset)
                data = handle.read(size)
        except OSError:
            return None
        expected = width * height * 4
        if len(data) < expected:
            return None
        data = data[:expected]
        return Image.frombytes("RGBA", (width, height), data, "raw", "BGRA")

    def _read_blp2_header(self, path: str) -> Optional[dict]:
        try:
            with open(path, "rb") as handle:
                magic = handle.read(4)
                if magic != b"BLP2":
                    return None
                compression = struct.unpack("<I", handle.read(4))[0]
                encoding = struct.unpack("<B", handle.read(1))[0]
                alpha_depth = struct.unpack("<B", handle.read(1))[0]
                alpha_encoding = struct.unpack("<B", handle.read(1))[0]
                has_mips = struct.unpack("<B", handle.read(1))[0]
                width, height = struct.unpack("<II", handle.read(8))
                offsets = struct.unpack("<16I", handle.read(16 * 4))
                sizes = struct.unpack("<16I", handle.read(16 * 4))
        except (OSError, struct.error):
            return None
        return {
            "compression": compression,
            "encoding": encoding,
            "alpha_depth": alpha_depth,
            "alpha_encoding": alpha_encoding,
            "has_mips": has_mips,
            "width": width,
            "height": height,
            "offsets": offsets,
            "sizes": sizes,
        }

    def _format_error_message(self, path: str, exc: Exception) -> str:
        header = self._read_blp2_header(path)
        details = f"{exc}"
        if header:
            details += (
                f"\nBLP2 compression: {header['compression']} | encoding: {header['encoding']}"
                f" | {header['width']}x{header['height']}"
            )
        details += f"\nPillow: {PIL_VERSION}"
        details += "\nTip: Update Pillow: pip install -U Pillow"
        return details

    def _add_bg_button(self, parent: ttk.Frame, color: str) -> None:
        btn = tk.Button(
            parent,
            text=color,
            bg=color,
            fg=self._contrast_text(color),
            relief="raised",
            command=lambda c=color: self._set_preview_bg(c),
        )
        btn.pack(side="left", padx=(6, 0))

    def _apply_custom_bg(self) -> None:
        raw = self.custom_color_var.get().strip()
        normalized = self._normalize_hex(raw)
        if not normalized:
            messagebox.showinfo(APP_TITLE, "Please enter a valid hex color (e.g. #ff00ff).")
            return
        self.custom_color_var.set(normalized)
        self._set_preview_bg(normalized)

    def _set_preview_bg(self, color: str) -> None:
        self.preview_bg = color
        self.preview_canvas.configure(background=color)
        if self.current_pil is None:
            msg = self.placeholder_message or "No image loaded."
            self._clear_preview(msg)
        else:
            self.render_preview()

    def _contrast_text(self, color: str) -> str:
        try:
            color = color.lstrip("#")
            if len(color) == 3:
                color = "".join(c * 2 for c in color)
            r = int(color[0:2], 16)
            g = int(color[2:4], 16)
            b = int(color[4:6], 16)
        except (ValueError, IndexError):
            return "#ffffff"
        # YIQ contrast
        yiq = (r * 299 + g * 587 + b * 114) / 1000
        return "#000000" if yiq >= 128 else "#ffffff"

    def _normalize_hex(self, value: str) -> Optional[str]:
        if not value:
            return None
        v = value.lower()
        if not v.startswith("#"):
            v = "#" + v
        hex_part = v[1:]
        if len(hex_part) == 3 and all(c in "0123456789abcdef" for c in hex_part):
            return "#" + "".join(c * 2 for c in hex_part)
        if len(hex_part) == 6 and all(c in "0123456789abcdef" for c in hex_part):
            return "#" + hex_part
        return None


def main() -> None:
    root = tk.Tk()
    app = BlpApp(root)
    root.mainloop()


if __name__ == "__main__":
    main()
