---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "tools/normalize_jass.py"
ref: HEAD
---

#!/usr/bin/env python3
import sys
from pathlib import Path


def normalize(text: str) -> str:
    return "\n".join(line.rstrip() for line in text.replace("\r\n", "\n").replace("\r", "\n").split("\n")).strip() + "\n"


def main() -> int:
    if len(sys.argv) != 2:
        print("usage: normalize_jass.py <file>", file=sys.stderr)
        return 2
    sys.stdout.write(normalize(Path(sys.argv[1]).read_text(encoding="utf-8", errors="surrogateescape")))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
