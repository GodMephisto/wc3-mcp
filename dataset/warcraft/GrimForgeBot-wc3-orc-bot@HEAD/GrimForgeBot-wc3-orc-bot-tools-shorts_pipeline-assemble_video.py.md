---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/assemble_video.py"
ref: HEAD
---

"""Video assembly — FFmpeg pipeline: gameplay + voiceover + captions → Short.

Input:
    session_dir/
        recording_path.txt   (path to OBS .mkv output)
        voiceover.mp3
        voiceover.srt        (optional; will use faster-whisper if missing)

Output:
    session_dir/short.mp4    (1080×1920, H264, AAC, ≤59s)

Reframe strategy:
    Source: 1920×1080 (16:9 OBS capture)
    Output: 1080×1920 (9:16 Shorts)
    Method: scale source to 1080 wide (preserving AR → 1080×607),
            pad to 1080×1920 with charcoal background,
            gameplay sits at top, black letterbox at bottom.

Requires: ffmpeg in PATH (brew install ffmpeg)
Optional: pip install faster-whisper   (for word-level captions when SRT missing)
"""
from __future__ import annotations
import subprocess
import shlex
from pathlib import Path

from tools.shorts_pipeline.config import (
    VIDEO_WIDTH, VIDEO_HEIGHT, VIDEO_FPS, MAX_SHORT_SECS,
    SOURCE_WIDTH, SOURCE_HEIGHT,
)

# Charcoal background: hex #1A1A1A
_BG_COLOR = "0x1A1A1A"


def _probe_duration(path: Path) -> float:
    result = subprocess.run(
        ["ffprobe", "-v", "quiet", "-show_entries", "format=duration",
         "-of", "default=noprint_wrappers=1:nokey=1", str(path)],
        capture_output=True, text=True, timeout=10,
    )
    try:
        return float(result.stdout.strip())
    except ValueError:
        return 0.0


def _generate_captions_whisper(mp3_path: Path, srt_path: Path) -> Path:
    """Generate word-level SRT from audio using faster-whisper."""
    try:
        from faster_whisper import WhisperModel  # type: ignore
    except ImportError:
        raise RuntimeError("pip install faster-whisper  (or provide voiceover.srt)")

    print("  [Captions] running faster-whisper (small model)...")
    model    = WhisperModel("small", device="cpu", compute_type="int8")
    segments, _ = model.transcribe(str(mp3_path), word_timestamps=True)

    lines = []
    idx   = 1
    for seg in segments:
        if seg.words:
            for word in seg.words:
                start = _fmt_srt_time(word.start)
                end   = _fmt_srt_time(word.end)
                lines.append(f"{idx}\n{start} --> {end}\n{word.word.strip()}\n")
                idx += 1
        else:
            start = _fmt_srt_time(seg.start)
            end   = _fmt_srt_time(seg.end)
            lines.append(f"{idx}\n{start} --> {end}\n{seg.text.strip()}\n")
            idx += 1

    srt_path.write_text("\n".join(lines), encoding="utf-8")
    print(f"  [Captions] SRT written → {srt_path.name}")
    return srt_path


def _fmt_srt_time(seconds: float) -> str:
    h = int(seconds // 3600)
    m = int((seconds % 3600) // 60)
    s = int(seconds % 60)
    ms = int((seconds % 1) * 1000)
    return f"{h:02d}:{m:02d}:{s:02d},{ms:03d}"


def assemble_video(session_dir: Path, clip_start: float = 0.0) -> Path:
    """
    Assemble Short from session directory.

    Args:
        session_dir:  path to session folder
        clip_start:   seconds into the gameplay recording to start the clip (default 0)

    Returns:
        path to assembled short.mp4
    """
    # ── Locate inputs ──────────────────────────────────────────────────────────

    rec_ptr     = session_dir / "recording_path.txt"
    mp3_path    = session_dir / "voiceover.mp3"
    srt_path    = session_dir / "voiceover.srt"
    output_path = session_dir / "short.mp4"

    if not rec_ptr.exists():
        raise FileNotFoundError(
            f"recording_path.txt missing in {session_dir}\n"
            "Did OBS save a recording? Check OBS output settings."
        )
    gameplay_path = Path(rec_ptr.read_text().strip())
    if not gameplay_path.exists():
        raise FileNotFoundError(f"Gameplay file not found: {gameplay_path}")

    if not mp3_path.exists():
        raise FileNotFoundError(f"voiceover.mp3 missing in {session_dir}")

    # ── Generate captions if SRT missing ──────────────────────────────────────

    if not srt_path.exists():
        print("  [Captions] voiceover.srt missing — generating with faster-whisper")
        srt_path = _generate_captions_whisper(mp3_path, srt_path)

    # ── Determine clip duration ────────────────────────────────────────────────

    voice_duration = _probe_duration(mp3_path)
    if voice_duration <= 0:
        raise ValueError(f"Could not determine voiceover duration: {mp3_path}")

    clip_duration = min(voice_duration + 1.0, MAX_SHORT_SECS)
    print(f"  [Assemble] voice={voice_duration:.1f}s  clip={clip_duration:.1f}s  start={clip_start:.1f}s")

    # ── Compute video geometry ─────────────────────────────────────────────────
    # Scale 1920×1080 → 1080×607, pad to 1080×1920 (charcoal bg, gameplay at top)

    scaled_h = int(VIDEO_WIDTH * SOURCE_HEIGHT / SOURCE_WIDTH)   # = 607
    pad_y    = (VIDEO_HEIGHT - scaled_h) // 2                    # center vertically
    # For Shorts: put gameplay at top (y=0) so action is immediately visible
    # and text/stats area is the bottom letterbox

    # ── Build FFmpeg filter graph ──────────────────────────────────────────────

    srt_escaped = str(srt_path).replace(":", "\\:")
    # Note: on macOS the SRT path must be escaped for the subtitles filter

    # Build SRT path for FFmpeg subtitles filter (must handle colons on macOS)
    srt_str = str(srt_path)

    filter_complex = (
        # Scale gameplay to 1080 wide, maintain AR
        f"[0:v]scale={VIDEO_WIDTH}:{scaled_h},"
        # Pad to full 9:16 frame, gameplay at top (y=0)
        f"pad={VIDEO_WIDTH}:{VIDEO_HEIGHT}:0:0:{_BG_COLOR},"
        # Burn subtitles
        f"subtitles='{srt_str}':force_style='Fontsize=18,PrimaryColour=&H00FFFFFF,"
        f"OutlineColour=&H00000000,Outline=2,Bold=1,Alignment=2,MarginV=60'[v];"
        # Mix voiceover + gameplay audio (gameplay at low volume for ambience)
        f"[0:a]volume=0.08[bg];"
        f"[1:a][bg]amix=inputs=2:duration=first:dropout_transition=2[a]"
    )

    cmd = [
        "ffmpeg", "-y",
        "-ss", str(clip_start),              # seek in gameplay
        "-t",  str(clip_duration),           # clip duration
        "-i",  str(gameplay_path),           # input 0: gameplay
        "-i",  str(mp3_path),                # input 1: voiceover
        "-filter_complex", filter_complex,
        "-map", "[v]",
        "-map", "[a]",
        "-c:v", "libx264",
        "-crf", "18",
        "-preset", "fast",
        "-c:a", "aac",
        "-b:a", "192k",
        "-movflags", "+faststart",           # web-optimized MP4
        str(output_path),
    ]

    print(f"  [Assemble] running FFmpeg...")
    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode != 0:
        print(result.stderr[-3000:])   # show last 3k chars of FFmpeg error
        raise RuntimeError(f"FFmpeg failed (exit {result.returncode})")

    size_mb = output_path.stat().st_size / 1_000_000
    print(f"  [Assemble] done → {output_path.name} ({size_mb:.1f} MB)")
    return output_path


if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("Usage: python assemble_video.py <session_dir> [clip_start_seconds]")
        sys.exit(1)
    start = float(sys.argv[2]) if len(sys.argv) > 2 else 0.0
    out   = assemble_video(Path(sys.argv[1]), clip_start=start)
    print(f"Short: {out}")
