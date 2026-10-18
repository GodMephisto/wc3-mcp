---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/make_thumbnail.py"
ref: HEAD
---

"""Thumbnail generation — extract frame from gameplay + overlay text.

Output: session_dir/thumbnail.jpg (1280×720, YouTube spec)

Requires: pillow (already installed), ffmpeg in PATH
"""
from __future__ import annotations
import subprocess
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont  # type: ignore

from tools.shorts_pipeline.config import (
    THUMB_WIDTH, THUMB_HEIGHT,
    THUMB_FONT_SIZE, THUMB_COLOR_BG, THUMB_COLOR_TEXT, THUMB_COLOR_SUB,
    PROJECT_ROOT,
)

# Optional: place a custom font in tools/shorts_pipeline/assets/font.ttf
_FONT_PATH = PROJECT_ROOT / "tools" / "shorts_pipeline" / "assets" / "font.ttf"


def _extract_frame(video_path: Path, output_path: Path, timestamp: float = 30.0) -> Path:
    """Extract a single frame from video at timestamp seconds."""
    cmd = [
        "ffmpeg", "-y",
        "-ss", str(timestamp),
        "-i", str(video_path),
        "-frames:v", "1",
        "-q:v", "2",               # high quality JPEG
        str(output_path),
    ]
    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode != 0 or not output_path.exists():
        raise RuntimeError(f"Frame extraction failed: {result.stderr[-1000:]}")
    return output_path


def _load_font(size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    if _FONT_PATH.exists():
        return ImageFont.truetype(str(_FONT_PATH), size)
    # Fallback: try system fonts
    for name in ["/System/Library/Fonts/Helvetica.ttc",
                 "/System/Library/Fonts/Arial.ttf",
                 "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"]:
        if Path(name).exists():
            return ImageFont.truetype(name, size)
    return ImageFont.load_default()


def make_thumbnail(session_dir: Path, title: str, frame_ts: float = 30.0) -> Path:
    """
    Create thumbnail for the Short.

    Args:
        session_dir:  session folder
        title:        main text to overlay (keep ≤30 chars for readability)
        frame_ts:     seconds into gameplay to extract the frame

    Returns:
        path to thumbnail.jpg
    """
    thumb_path = session_dir / "thumbnail.jpg"

    # Find gameplay recording
    rec_ptr = session_dir / "recording_path.txt"
    if not rec_ptr.exists():
        raise FileNotFoundError("recording_path.txt missing")
    gameplay_path = Path(rec_ptr.read_text().strip())

    # Extract raw frame
    raw_frame_path = session_dir / "thumbnail_raw.jpg"
    _extract_frame(gameplay_path, raw_frame_path, timestamp=frame_ts)
    print(f"  [Thumb] frame extracted at t={frame_ts:.0f}s")

    # Open and resize to thumbnail dimensions
    img = Image.open(raw_frame_path).convert("RGB")
    img = img.resize((THUMB_WIDTH, THUMB_HEIGHT), Image.LANCZOS)

    # Dark overlay for text contrast
    overlay = Image.new("RGBA", img.size, (0, 0, 0, 0))
    draw_ov = ImageDraw.Draw(overlay)
    draw_ov.rectangle(
        [(0, THUMB_HEIGHT - 220), (THUMB_WIDTH, THUMB_HEIGHT)],
        fill=(26, 26, 26, 210),    # charcoal, 82% opacity
    )
    img = img.convert("RGBA")
    img = Image.alpha_composite(img, overlay)
    img = img.convert("RGB")

    # Draw title text
    draw = ImageDraw.Draw(img)
    font_main = _load_font(THUMB_FONT_SIZE)
    font_sub  = _load_font(36)

    # Main title — ember orange
    draw.text(
        (60, THUMB_HEIGHT - 180),
        title,
        font=font_main,
        fill=THUMB_COLOR_TEXT,
    )

    # Subtitle / channel tag — cold blue
    draw.text(
        (60, THUMB_HEIGHT - 70),
        "GrimForge",
        font=font_sub,
        fill=THUMB_COLOR_SUB,
    )

    img.save(str(thumb_path), "JPEG", quality=95)
    print(f"  [Thumb] saved → {thumb_path.name}")
    return thumb_path


if __name__ == "__main__":
    import sys
    if len(sys.argv) < 3:
        print("Usage: python make_thumbnail.py <session_dir> \"Thumbnail Title\" [frame_ts]")
        sys.exit(1)
    ts  = float(sys.argv[3]) if len(sys.argv) > 3 else 30.0
    out = make_thumbnail(Path(sys.argv[1]), sys.argv[2], frame_ts=ts)
    print(f"Thumbnail: {out}")
