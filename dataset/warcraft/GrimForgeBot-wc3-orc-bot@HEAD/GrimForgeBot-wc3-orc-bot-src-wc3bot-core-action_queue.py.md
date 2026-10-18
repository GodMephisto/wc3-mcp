---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/core/action_queue.py"
ref: HEAD
---

"""ActionQueue — priority-ordered command pipeline for the InputExecutor.

Design
──────
Decision threads (MacroLoop, MicroLoop, future agents) call queue.submit(action).
The InputExecutor is the *only* thread that ever calls WC3Input methods.
This eliminates all CGEvent thread-safety concerns and serialises input
exactly as WC3's event port expects.

Priority levels (lower = higher urgency)
  MICRO   0  hero abilities, unit reactions
  ARMY    2  train grunt, attack-move commands
  MACRO   5  build order, train peon, build burrow
  ECONOMY 8  lumber routing, utility moves

Within the same priority level, FIFO order is maintained via a monotonic
sequence counter so two MACRO actions enqueued at the same moment fire
in submission order.
"""
from __future__ import annotations
import queue
import threading
from dataclasses import dataclass, field
from typing import Callable


class Priority:
    MICRO   = 0
    ARMY    = 2
    MACRO   = 5
    ECONOMY = 8


@dataclass
class ActionStep:
    """One atomic input operation within an Action sequence.

    fn          — callable (e.g. inp.key, inp.right_click)
    args        — positional arguments
    kwargs      — keyword arguments
    delay_after — time.sleep() duration *after* this step completes.
                  Use for game-mechanic delays (menu open, camera snap, etc.)
                  NOT for input registration (the new inp defaults handle that).
    """
    fn:          Callable
    args:        tuple = ()
    kwargs:      dict  = field(default_factory=dict)
    delay_after: float = 0.0


@dataclass
class Action:
    """An ordered sequence of ActionSteps submitted to the InputExecutor.

    priority  — see Priority class; lower fires first
    label     — human-readable name for logs/debug
    steps     — list of ActionStep executed in order by the InputExecutor
    """
    priority: int
    label:    str
    steps:    list[ActionStep] = field(default_factory=list)


class ActionQueue:
    """Thread-safe priority queue consumed exclusively by InputExecutor.

    submit() is safe to call from any thread.
    get() / peek_priority() are called only from InputExecutor.
    """

    def __init__(self) -> None:
        self._q:       queue.PriorityQueue = queue.PriorityQueue()
        self._counter: int                 = 0
        self._lock:    threading.Lock      = threading.Lock()

    def submit(self, action: Action) -> None:
        """Enqueue an action. Thread-safe."""
        with self._lock:
            self._counter += 1
            seq = self._counter
        # Tuple ordering: (priority, seq, action) — seq breaks priority ties (FIFO)
        self._q.put((action.priority, seq, action))

    def get(self, timeout: float = 0.05) -> Action | None:
        """Block until an action is available, then return it. Returns None on timeout."""
        try:
            _, _, action = self._q.get(timeout=timeout)
            return action
        except queue.Empty:
            return None

    def peek_priority(self) -> int | None:
        """Return the priority of the next queued action without removing it.

        Used by InputExecutor for preemption decisions between steps.
        Safe to call from the executor thread only (single consumer).
        """
        try:
            return self._q.queue[0][0]
        except IndexError:
            return None

    def task_done(self) -> None:
        self._q.task_done()

    def qsize(self) -> int:
        return self._q.qsize()
