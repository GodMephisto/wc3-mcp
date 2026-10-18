---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_lua_reader2.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_lua_reader2.py — Test Option B hot read without liveness gate.

Run with WC3 + sidecar map running (game does NOT need to be live yet):
    python3 scripts/diag_lua_reader2.py

Prints:
- Bootstrap result
- Raw hot_poll output (even if T is not advancing)
- Whether the SIDECAR_RE matches
"""
import sys, time
sys.path.insert(0, "src")

from wc3bot.core.process import get_pid, get_task
from wc3bot.observe.lua_reader import LuaStateReader, _read_mem
from wc3bot.observe.state import parse_state, fmt_state, SIDECAR_RE

pid  = get_pid()
task = get_task(pid)
print(f"WC3 PID={pid}\n")

reader = LuaStateReader(task, pid=pid)
if reader.node_addr is None:
    print("[FAIL] bootstrap failed — is the sidecar map loaded?")
    sys.exit(1)

print(f"Node @ 0x{reader.node_addr:x}")
print("\n--- hot_poll test (first 10 reads, no liveness gate) ---")
for i in range(10):
    raw = reader.hot_poll()
    if raw is None:
        print(f"  [{i+1}] hot_poll → None")
    else:
        m = SIDECAR_RE.match(raw)
        if m:
            state = parse_state(raw)
            if state:
                print(f"  [{i+1}] MATCH: {fmt_state(state)}")
            else:
                print(f"  [{i+1}] MATCH but parse_state returned None")
        else:
            # Show what we got
            printable = raw[:120].replace(b'\x00', b'.').decode('latin1')
            print(f"  [{i+1}] raw ({len(raw)}B): {printable!r}")
    time.sleep(0.1)
