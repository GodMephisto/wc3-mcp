---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/generate_voice.py"
ref: HEAD
---

"""Voice generation — narration script → MP3 + SRT subtitles.

Primary:  edge-tts (free, no API key)   pip install edge-tts
Fallback: ElevenLabs (€5.50/month)     pip install elevenlabs

edge-tts generates the SRT alongside the audio automatically — used for captions.
"""
from __future__ import annotations
import asyncio
import subprocess
from pathlib import Path

from tools.shorts_pipeline.config import (
    EDGE_TTS_VOICE, VOICE_RATE,
    ELEVENLABS_API_KEY, ELEVENLABS_VOICE_ID,
)


def _generate_edge_tts(text: str, mp3_path: Path, srt_path: Path) -> None:
    """Generate voice with edge-tts (free). Writes mp3 + srt."""
    async def _run() -> None:
        import edge_tts  # type: ignore
        communicate = edge_tts.Communicate(text, EDGE_TTS_VOICE, rate=VOICE_RATE)
        sub_maker   = edge_tts.SubMaker()
        with open(mp3_path, "wb") as f:
            async for chunk in communicate.stream():
                if chunk["type"] == "audio":
                    f.write(chunk["data"])
                elif chunk["type"] == "WordBoundary":
                    sub_maker.feed(chunk)
        srt_path.write_text(sub_maker.get_srt(), encoding="utf-8")

    asyncio.run(_run())
    print(f"  [Voice] edge-tts → {mp3_path.name} + {srt_path.name}")


def _generate_elevenlabs(text: str, mp3_path: Path) -> None:
    """Generate voice with ElevenLabs API. No SRT — use faster-whisper for captions."""
    from elevenlabs.client import ElevenLabs  # type: ignore
    client = ElevenLabs(api_key=ELEVENLABS_API_KEY)
    audio  = client.text_to_speech.convert(
        voice_id=ELEVENLABS_VOICE_ID,
        text=text,
        model_id="eleven_turbo_v2_5",
    )
    with open(mp3_path, "wb") as f:
        for chunk in audio:
            f.write(chunk)
    print(f"  [Voice] ElevenLabs → {mp3_path.name}")


def _probe_audio_duration(mp3_path: Path) -> float:
    """Return audio duration in seconds via ffprobe."""
    try:
        result = subprocess.run(
            ["ffprobe", "-v", "quiet", "-show_entries", "format=duration",
             "-of", "default=noprint_wrappers=1:nokey=1", str(mp3_path)],
            capture_output=True, text=True, timeout=10,
        )
        return float(result.stdout.strip())
    except Exception:
        return 0.0


def generate_voice(session_dir: Path) -> tuple[Path, Path | None]:
    """
    Generate voice from narration_script.txt.
    Returns (mp3_path, srt_path). srt_path is None if not generated.
    """
    script_path = session_dir / "narration_script.txt"
    mp3_path    = session_dir / "voiceover.mp3"
    srt_path    = session_dir / "voiceover.srt"

    if not script_path.exists():
        raise FileNotFoundError(f"narration_script.txt missing in {session_dir}")

    # Strip comment lines (lines starting with #)
    lines = [l for l in script_path.read_text(encoding="utf-8").splitlines()
             if not l.startswith("#")]
    text = "\n".join(lines).strip()

    if not text:
        raise ValueError("narration_script.txt is empty or only has comments")

    # Try ElevenLabs first if configured
    if ELEVENLABS_API_KEY and ELEVENLABS_VOICE_ID:
        try:
            _generate_elevenlabs(text, mp3_path)
            duration = _probe_audio_duration(mp3_path)
            print(f"  [Voice] duration: {duration:.1f}s")
            return mp3_path, None   # no SRT from ElevenLabs; faster-whisper handles captions
        except Exception as e:
            print(f"  [Voice] ElevenLabs failed ({e}), falling back to edge-tts")

    # edge-tts
    try:
        _generate_edge_tts(text, mp3_path, srt_path)
        duration = _probe_audio_duration(mp3_path)
        print(f"  [Voice] duration: {duration:.1f}s")
        return mp3_path, srt_path
    except ImportError:
        raise RuntimeError(
            "edge-tts not installed. Run: pip install edge-tts\n"
            "Or set ELEVENLABS_API_KEY + ELEVENLABS_VOICE_ID in .env"
        )


if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("Usage: python generate_voice.py <session_dir>")
        sys.exit(1)
    mp3, srt = generate_voice(Path(sys.argv[1]))
    print(f"Voice: {mp3}")
    if srt:
        print(f"SRT:   {srt}")
