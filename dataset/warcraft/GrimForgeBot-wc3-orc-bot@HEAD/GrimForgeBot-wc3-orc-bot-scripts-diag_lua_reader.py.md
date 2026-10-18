---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_lua_reader.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_lua_reader.py — Quick smoke-test for LuaStateReader (Option B).

Run with WC3 + sidecar map already running:
    python3 scripts/diag_lua_reader.py

Should print:
  [LuaReader] bootstrap: ...
  [LuaReader] Node cached @ 0x<addr>  (<N.Ns)
  [LuaReader] live at T=<N>
  T=  3.5  G=  500  L=  ... units=5  bldgs=1  ...
  Polling 10 Hz — Ctrl+C to stop
  T=  3.6  ...
  ...
"""
import sys, time
sys.path.insert(0, "src")

from wc3bot.core.process import get_pid, get_task
from wc3bot.observe.lua_reader import LuaStateReader
from wc3bot.observe.state import fmt_state

pid  = get_pid()
task = get_task(pid)
print(f"WC3 PID={pid}")

reader = LuaStateReader(task, pid=pid)
if reader.node_addr is None:
    print("[FAIL] bootstrap failed — is the sidecar map loaded and game started?")
    sys.exit(1)

raw = reader.find_live_game(verbose=True)
if raw is None:
    print("[FAIL] no live game detected — start a game first")
    sys.exit(1)

from wc3bot.observe.state import parse_state
state = parse_state(raw)
if state:
    print(f"Initial: {fmt_state(state)}")

print("Polling 10 Hz — Ctrl+C to stop")
try:
    for state in reader.poll_loop(interval=0.1):
        print(fmt_state(state))
except KeyboardInterrupt:
    print("\nStopped.")
