---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/mach_test.py"
ref: HEAD
---

"""Quick test: can this Python process call task_for_pid on WC3?"""
import ctypes, ctypes.util, subprocess, sys

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]

result = subprocess.run(["pgrep", "-x", "Warcraft III"], capture_output=True, text=True)
pid = int(result.stdout.strip().split("\n")[0])
print(f"WC3 PID: {pid}")

task = ctypes.c_uint(0)
kern_return = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(task))
if kern_return == 0:
    print(f"task_for_pid OK — task port: {task.value}")
else:
    print(f"task_for_pid FAILED — kern_return: {kern_return} (5=KERN_FAILURE, 4=KERN_INVALID_TASK)")
