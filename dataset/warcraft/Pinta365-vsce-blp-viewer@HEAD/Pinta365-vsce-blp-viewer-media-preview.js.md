---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "media/preview.js"
ref: HEAD
---

const vscode = acquireVsCodeApi();

const canvas = document.getElementById("canvas");
const ctx = canvas.getContext("2d");
const metaEl = document.getElementById("meta");
const errorEl = document.getElementById("error");

function formatLabel(version, content, compression, alphaBits, preferredFormat) {
    if (version === "BLP1") {
        if (content === 0) return "BLP1 JPEG";
        if (content === 1) return "BLP1 RAW1 (Palettized)";
        return "BLP1 (Unknown Content)";
    }

    // compression: 1=PALETTE, 2=DXT, 3=ARGB8888
    // preferredFormat: DXT1=0, DXT3=1, DXT5=7, ARGB8888=4, ARGB1555=5, ARGB4444=6
    if (compression === 1) return "RAW1 (Palettized)";
    if (compression === 3) {
        if (preferredFormat === 5) return "ARGB1555";
        if (preferredFormat === 6) return "ARGB4444";
        return "RAW3 (ARGB8888)";
    }
    if (compression === 2) {
        if (alphaBits <= 1) return "DXT1";
        if (preferredFormat === 1) return "DXT3";
        return "DXT5";
    }
    return "Unknown";
}

function alphaLabel(alphaBits) {
    if (alphaBits === 0) return "None";
    if (alphaBits === 1) return "1-bit";
    if (alphaBits === 4) return "4-bit";
    if (alphaBits === 8) return "8-bit";
    return `${alphaBits}-bit`;
}

function fileSizeLabel(bytes) {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
}

function renderMeta(width, height, meta) {
    const { version, content, compression, alphaBits, preferredFormat, mipCount, fileSize } = meta;
    const rows = [
        ["Dimensions", `${width} × ${height}`],
        ["Version", version],
        ["Format", formatLabel(version, content, compression, alphaBits, preferredFormat)],
        ["Alpha", alphaLabel(alphaBits)],
        ["Mipmaps", mipCount > 1 ? `${mipCount} levels` : "None"],
        ["File size", fileSizeLabel(fileSize)],
    ];
    metaEl.innerHTML = rows.map(([k, v]) => `<dt>${k}</dt><dd>${v}</dd>`).join("");
    metaEl.style.display = "grid";
}

window.addEventListener("message", (event) => {
    const msg = event.data;

    if (msg.type === "image") {
        const { width, height, pixels, meta } = msg;

        canvas.width = width;
        canvas.height = height;

        const imageData = ctx.createImageData(width, height);
        imageData.data.set(pixels);
        ctx.putImageData(imageData, 0, 0);

        canvas.style.display = "block";
        renderMeta(width, height, meta);

    } else if (msg.type === "error") {
        errorEl.textContent = `Failed to decode BLP: ${msg.message}`;
        errorEl.style.display = "block";
    }
});
