---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/pipeline.py"
ref: HEAD
---

"""GrimForge Shorts Pipeline — main orchestrator.

Runs all steps for a completed bot session:
    1. Generate narration script (Claude API or template)
    2. Generate voiceover (edge-tts / ElevenLabs)
    3. Assemble video (FFmpeg)
    4. Generate thumbnail
    5. PAUSE for human review
    6. Upload to YouTube (optional)

Usage:
    # Full pipeline (script → voice → video → human review → upload)
    python tools/shorts_pipeline/pipeline.py <session_dir>

    # Skip to a specific step (if earlier steps already ran)
    python tools/shorts_pipeline/pipeline.py <session_dir> --from voice
    python tools/shorts_pipeline/pipeline.py <session_dir> --from assemble
    python tools/shorts_pipeline/pipeline.py <session_dir> --from upload

    # Latest session (auto-detect)
    python tools/shorts_pipeline/pipeline.py --latest

    # Skip upload (review only)
    python tools/shorts_pipeline/pipeline.py <session_dir> --no-upload
"""
from __future__ import annotations
import argparse
import datetime
import sys
from pathlib import Path

# Add project root to path so tools.shorts_pipeline imports work
_PROJECT_ROOT = Path(__file__).parent.parent.parent
if str(_PROJECT_ROOT) not in sys.path:
    sys.path.insert(0, str(_PROJECT_ROOT))

from tools.shorts_pipeline.config import RECORDINGS_DIR


def _latest_session() -> Path:
    sessions = sorted(RECORDINGS_DIR.iterdir(), reverse=True)
    if not sessions:
        raise RuntimeError(f"No sessions found in {RECORDINGS_DIR}")
    return sessions[0]


def _human_review(session_dir: Path) -> dict:
    """Interactive review step — returns user decisions."""
    short_path = session_dir / "short.mp4"
    script_path = session_dir / "narration_script.txt"

    print("\n" + "═" * 60)
    print("  HUMAN REVIEW")
    print("═" * 60)
    print(f"  Short:  {short_path}")
    print(f"  Script: {script_path}")
    if (session_dir / "thumbnail.jpg").exists():
        print(f"  Thumb:  {session_dir / 'thumbnail.jpg'}")
    print()

    # Show script
    if script_path.exists():
        print("  --- Narration script ---")
        for line in script_path.read_text().splitlines():
            print(f"  {line}")
        print()

    # Ask for title
    print("  Enter video title (≤70 chars, Enter to use default):")
    default_title = f"GrimForge — {datetime.datetime.now().strftime('%Y-%m-%d')}"
    title = input(f"  [{default_title}] > ").strip()
    if not title:
        title = default_title

    # Ask for clip start time
    print("  Clip start time in seconds (0 = beginning, Enter to keep):")
    clip_start_str = input("  [0] > ").strip()
    clip_start = float(clip_start_str) if clip_start_str else 0.0

    # Upload decision
    print("  Upload to YouTube? [y/N]")
    upload = input("  > ").strip().lower() == "y"

    # Schedule
    publish_at = None
    if upload:
        print("  Schedule publish time (UTC, e.g. '2026-05-31 12:00', Enter = stay private):")
        ts_str = input("  > ").strip()
        if ts_str:
            try:
                publish_at = datetime.datetime.fromisoformat(ts_str)
            except ValueError:
                print("  Invalid datetime format — will stay private.")

    print("═" * 60 + "\n")
    return {
        "title":      title,
        "clip_start": clip_start,
        "upload":     upload,
        "publish_at": publish_at,
    }


def run_pipeline(
    session_dir: Path,
    from_step: str = "script",
    no_upload: bool = False,
    thumbnail_ts: float = 30.0,
) -> None:
    steps = ["script", "voice", "assemble", "thumbnail", "review", "upload"]
    start_idx = steps.index(from_step) if from_step in steps else 0

    print(f"\n{'═'*60}")
    print(f"  GrimForge Shorts Pipeline")
    print(f"  Session: {session_dir.name}")
    print(f"{'═'*60}\n")

    # ── Step 1: Script ─────────────────────────────────────────────────────────
    if start_idx <= 0:
        print("[1/5] Generating narration script...")
        from tools.shorts_pipeline.generate_script import generate_script
        generate_script(session_dir)

    # ── Step 2: Voice ──────────────────────────────────────────────────────────
    if start_idx <= 1:
        print("[2/5] Generating voiceover...")
        from tools.shorts_pipeline.generate_voice import generate_voice
        generate_voice(session_dir)

    # ── Step 3: Assemble ───────────────────────────────────────────────────────
    if start_idx <= 2:
        print("[3/5] Assembling video...")
        from tools.shorts_pipeline.assemble_video import assemble_video
        assemble_video(session_dir, clip_start=0.0)
        # Note: clip_start may be adjusted in human review → re-assemble if needed

    # ── Step 4: Thumbnail ──────────────────────────────────────────────────────
    if start_idx <= 3:
        print("[4/5] Generating thumbnail...")
        from tools.shorts_pipeline.make_thumbnail import make_thumbnail
        default_title = f"GrimForge — {session_dir.name}"
        make_thumbnail(session_dir, title=default_title, frame_ts=thumbnail_ts)

    # ── Step 5: Human review ───────────────────────────────────────────────────
    decisions = _human_review(session_dir)

    # Re-assemble if clip_start changed
    if decisions["clip_start"] > 0:
        print(f"[3/5] Re-assembling with clip_start={decisions['clip_start']:.1f}s...")
        from tools.shorts_pipeline.assemble_video import assemble_video
        assemble_video(session_dir, clip_start=decisions["clip_start"])

    # Re-generate thumbnail with user title
    print("[4/5] Re-generating thumbnail with final title...")
    from tools.shorts_pipeline.make_thumbnail import make_thumbnail
    make_thumbnail(
        session_dir,
        title=decisions["title"],
        frame_ts=decisions["clip_start"] + thumbnail_ts,
    )

    # ── Step 6: Upload ─────────────────────────────────────────────────────────
    if decisions["upload"] and not no_upload:
        print("[5/5] Uploading to YouTube...")
        from tools.shorts_pipeline.upload_youtube import upload_short
        description = (
            f"{decisions['title']}\n\n"
            f"GrimForge — Building a Warcraft 3 AI bot from scratch.\n\n"
            f"Support the project:\n"
            f"Ko-fi: https://ko-fi.com/grimforge\n"
            f"GitHub: https://github.com/sebastianhuber/wc3-orc-bot\n\n"
            f"#WC3 #Warcraft3 #AIBot #GameDev #GrimForge"
        )
        url = upload_short(
            session_dir,
            title=decisions["title"],
            description=description,
            publish_at=decisions["publish_at"],
        )
        print(f"\n  Published: {url}\n")
    else:
        print("[5/5] Upload skipped — short.mp4 ready for manual upload.")
        print(f"  File: {session_dir / 'short.mp4'}")

    print(f"\n{'═'*60}")
    print("  Pipeline complete.")
    print(f"{'═'*60}\n")


def main() -> None:
    parser = argparse.ArgumentParser(description="GrimForge Shorts Pipeline")
    parser.add_argument("session_dir", nargs="?", help="Path to session directory")
    parser.add_argument("--latest", action="store_true", help="Use latest session")
    parser.add_argument(
        "--from", dest="from_step", default="script",
        choices=["script", "voice", "assemble", "thumbnail", "review", "upload"],
        help="Start pipeline from this step (default: script)",
    )
    parser.add_argument("--no-upload", action="store_true", help="Skip YouTube upload")
    parser.add_argument("--thumb-ts", type=float, default=30.0,
                        help="Timestamp (seconds) for thumbnail frame extraction")
    args = parser.parse_args()

    if args.latest:
        session_dir = _latest_session()
        print(f"Using latest session: {session_dir}")
    elif args.session_dir:
        session_dir = Path(args.session_dir)
    else:
        parser.print_help()
        sys.exit(1)

    if not session_dir.exists():
        print(f"Session directory not found: {session_dir}")
        sys.exit(1)

    run_pipeline(
        session_dir=session_dir,
        from_step=args.from_step,
        no_upload=args.no_upload,
        thumbnail_ts=args.thumb_ts,
    )


if __name__ == "__main__":
    main()
