---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/generate_script.py"
ref: HEAD
---

"""Script generation — git log + bot terminal output → narration script.

Requires:  pip install anthropic
Fallback:  if no API key, writes a template for manual editing.
"""
from __future__ import annotations
import subprocess
from pathlib import Path

from tools.shorts_pipeline.config import ANTHROPIC_API_KEY, PROJECT_ROOT

PROMPT_TEMPLATE = """\
You are writing a YouTube Shorts narration for GrimForge — a channel documenting
a Warcraft 3 AI bot being built in Python. The bot plays Orc race, focusing on
Blademaster hero micro and perfect macro execution.

Tone: cold, precise, declarative. No filler words. No "hey guys". Present tense.
Short sentences. Machine narrates facts.

--- GIT LOG (last 20 commits) ---
{git_log}

--- BOT RUN TERMINAL (last 100 lines) ---
{terminal_lines}

--- GAME RESULT ---
{game_result}

Write a 50-second narration script (130-150 words total) with this structure:
- HOOK (3s): one punchy declarative sentence — the most surprising outcome or fact
- WHAT CHANGED (20s): explain the technical progress in plain terms, no jargon dumps
- DEMO (20s): narrate what the viewer sees in the gameplay and terminal clip
- RESULT + CLIFFHANGER (7s): outcome and one sentence about what comes next

Output: narration text only. No section headers, no timestamps, no stage directions.
"""


def _get_git_log() -> str:
    try:
        result = subprocess.run(
            ["git", "log", "--oneline", "-20"],
            cwd=PROJECT_ROOT,
            capture_output=True, text=True, timeout=10
        )
        return result.stdout.strip() or "(no commits found)"
    except Exception as e:
        return f"(git log failed: {e})"


def _read_terminal_log(log_path: Path, lines: int = 100) -> str:
    try:
        text = log_path.read_text(encoding="utf-8")
        tail = "\n".join(text.splitlines()[-lines:])
        return tail or "(empty log)"
    except Exception as e:
        return f"(log read failed: {e})"


def generate_script(session_dir: Path) -> Path:
    """Generate narration script for the session. Returns path to script file."""
    script_path = session_dir / "narration_script.txt"

    # Read inputs
    log_path    = session_dir / "bot_run.log"
    result_path = session_dir / "game_result.txt"
    git_log     = _get_git_log()
    terminal    = _read_terminal_log(log_path)
    game_result = result_path.read_text().strip() if result_path.exists() else "unknown"

    if not ANTHROPIC_API_KEY:
        # Write a template for manual editing
        script_path.write_text(
            "# GrimForge narration script — fill in manually\n"
            "# (ANTHROPIC_API_KEY not set)\n\n"
            "The bot made progress this session. [DESCRIBE WHAT HAPPENED].\n"
            "The key change: [TECHNICAL DETAIL].\n"
            "You can see [DESCRIBE DEMO MOMENT].\n"
            "Result: [WIN/LOSS]. Next: [WHAT COMES NEXT].\n"
        )
        print(f"  [Script] no API key — template written to {script_path}")
        return script_path

    prompt = PROMPT_TEMPLATE.format(
        git_log=git_log,
        terminal_lines=terminal,
        game_result=game_result,
    )

    try:
        import anthropic
        client = anthropic.Anthropic(api_key=ANTHROPIC_API_KEY)
        message = client.messages.create(
            model="claude-haiku-4-5-20251001",   # fast + cheap for script gen
            max_tokens=512,
            messages=[{"role": "user", "content": prompt}],
        )
        script_text = message.content[0].text.strip()
        script_path.write_text(script_text, encoding="utf-8")
        word_count = len(script_text.split())
        print(f"  [Script] generated ({word_count} words) → {script_path}")
    except Exception as e:
        script_path.write_text(
            f"# Script generation failed: {e}\n"
            "# Edit manually before proceeding.\n"
        )
        print(f"  [Script] Claude API failed: {e}")

    return script_path


if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("Usage: python generate_script.py <session_dir>")
        sys.exit(1)
    out = generate_script(Path(sys.argv[1]))
    print(f"Script: {out}")
    print(out.read_text())
