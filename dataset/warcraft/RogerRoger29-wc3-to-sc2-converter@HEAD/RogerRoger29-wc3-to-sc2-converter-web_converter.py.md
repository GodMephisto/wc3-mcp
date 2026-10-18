---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "web_converter.py"
ref: HEAD
---

"""Web converter prototype — Flask-based web UI for zero-install conversions.

Upload .mdx + .blp files → convert server-side → download .m3 + .dds.
Requires Flask: pip install flask

Usage:
    python web_converter.py
    → Open http://localhost:5000
"""
from __future__ import annotations
import os, sys, uuid, shutil, subprocess, tempfile, zipfile, io

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)

try:
    from flask import Flask, request, render_template_string, send_file, jsonify
except ImportError:
    print("Flask not installed. Run: pip install flask")
    sys.exit(1)

app = Flask(__name__)
app.config["MAX_CONTENT_LENGTH"] = 100 * 1024 * 1024  # 100MB max upload

HTML = """<!DOCTYPE html>
<html><head><meta charset="utf-8"><title>WC3→SC2 Web Converter</title>
<style>
body{font-family:Segoe UI,Arial;background:#1e1e2e;color:#cdd6f4;max-width:700px;margin:40px auto;padding:20px}
h1{color:#89b4fa} .box{background:#313244;border-radius:8px;padding:20px;margin:15px 0}
input,button{padding:10px 16px;border-radius:6px;border:1px solid #45475a;background:#1e1e2e;color:#cdd6f4;cursor:pointer}
button{background:#a6e3a1;color:#1e1e2e;font-weight:bold} button:hover{background:#94e2d5}
.result{padding:10px;border-radius:6px;margin:8px 0} .ok{background:#1e3a1e} .err{background:#3a1e1e}
</style></head><body>
<h1>WC3 → SC2 Model Converter</h1>
<p>Upload a .mdx model and its .blp textures, get back a .m3 + .dds ready for SC2.</p>
<div class="box">
<form action="/convert" method="post" enctype="multipart/form-data">
<p><b>Model file (.mdx):</b><br><input type="file" name="mdx" accept=".mdx" required></p>
<p><b>Texture files (.blp/.png/.tga):</b><br><input type="file" name="textures" accept=".blp,.png,.tga" multiple></p>
<p><b>Scale (optional):</b> <input type="number" name="scale" value="0.05" step="0.01" style="width:80px"></p>
<button type="submit">🔄 Convert</button>
</form></div>
<p style="color:#6c7086;font-size:11px">Powered by wc3toSC2 v3.0 | <a href="https://github.com/RogerRoger29/wc3-to-sc2-converter" style="color:#89b4fa">GitHub</a></p>
</body></html>"""

RESULT_HTML = """<!DOCTYPE html>
<html><head><meta charset="utf-8"><title>Conversion Result</title>
<style>body{font-family:Segoe UI;background:#1e1e2e;color:#cdd6f4;max-width:700px;margin:40px auto;padding:20px}
h1{color:#89b4fa} .ok{background:#1e3a1e;padding:15px;border-radius:8px} .err{background:#3a1e1e;padding:15px;border-radius:8px}
a{color:#a6e3a1}</style></head><body>
<h1>{{title}}</h1>
<p>{{message}}</p>
{% if download_url %}<p><a href="{{download_url}}">📥 Download {{download_name}}</a></p>{% endif %}
{% if report %}<pre style="background:#313244;padding:10px;border-radius:6px;font-size:11px">{{report}}</pre>{% endif %}
<p><a href="/">← Convert another</a></p>
</body></html>"""


@app.route("/")
def index():
    return HTML


@app.route("/convert", methods=["POST"])
def convert():
    mdx_file = request.files.get("mdx")
    if not mdx_file or not mdx_file.filename.lower().endswith(".mdx"):
        return render_template_string(RESULT_HTML, title="Error", message="Please upload a .mdx file.")

    # Create temp workspace
    work_dir = tempfile.mkdtemp(prefix="wc3web_")
    mdx_path = os.path.join(work_dir, "model.mdx")
    mdx_file.save(mdx_path)

    # Save textures
    textures = request.files.getlist("textures")
    tex_dir = os.path.join(work_dir, "Textures")
    os.makedirs(tex_dir, exist_ok=True)
    for t in textures:
        if t.filename:
            t.save(os.path.join(tex_dir, os.path.basename(t.filename)))

    scale = request.form.get("scale", "0.05")
    out_dir = os.path.join(work_dir, "out")

    try:
        import subprocess
        result = subprocess.run(
            [sys.executable, os.path.join(HERE, "convert.py"),
             mdx_path, out_dir,
             "--quiet"],
            capture_output=True, text=True, timeout=120, cwd=HERE)

        m3_path = os.path.join(out_dir, "model.m3")
        if os.path.exists(m3_path):
            # Create ZIP with all outputs
            zip_buf = io.BytesIO()
            with zipfile.ZipFile(zip_buf, "w", zipfile.ZIP_DEFLATED) as zf:
                for root, _, files in os.walk(out_dir):
                    for f in files:
                        zf.write(os.path.join(root, f), f)
            zip_buf.seek(0)

            zip_path = os.path.join(work_dir, "converted.zip")
            with open(zip_path, "wb") as f:
                f.write(zip_buf.read())

            return render_template_string(
                RESULT_HTML, title="✅ Conversion Complete",
                message="Your model has been converted. Download the ZIP containing the .m3 and .dds files.",
                download_url=f"/download/{os.path.basename(work_dir)}",
                download_name="converted.zip",
                report=result.stdout[-2000:])
        else:
            return render_template_string(
                RESULT_HTML, title="❌ Conversion Failed",
                message="The converter encountered an error.",
                report=result.stderr[-2000:])
    except Exception as e:
        return render_template_string(
            RESULT_HTML, title="❌ Error", message=str(e))
    finally:
        # Cleanup handled by OS temp cleanup
        pass


@app.route("/download/<work_id>")
def download(work_id):
    work_dir = os.path.join(tempfile.gettempdir(), f"wc3web_{work_id}")
    zip_path = os.path.join(work_dir, "converted.zip")
    if os.path.exists(zip_path):
        return send_file(zip_path, as_attachment=True, download_name="converted.zip")
    return "File not found (expired)", 404


if __name__ == "__main__":
    print("WC3 → SC2 Web Converter")
    print("Open http://localhost:5000 in your browser")
    app.run(host="0.0.0.0", port=5000, debug=False)
