---
type: vendor-doc
source_repo: "https://github.com/Blimba/PyWC3"
source_path: "pysrc/std/handle.py"
ref: HEAD
---

"""

    Handle

    A wrapper meant to collect the python object belonging to a blizzard 'handle'.

"""
class Handle:
    handles = {}
    def __init__(self, handle):
        # constructorfunc can be a function that returns a handle, or a handle directly
        self._handle = handle
        if Handle.get(handle) != None:
            print("Warning: secondary object created for handle ", handle, "object", self)
        Handle.handles[self._handle] = self

    @staticmethod
    def get(handle):
        if handle in Handle.handles:
            return Handle.handles[handle]
        return None

    def destroy(self):
        del Handle.handles[self._handle]
        self._handle = None

    @property
    def handle(self):
        return self._handle
