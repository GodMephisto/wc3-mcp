---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "shared/blob.py"
ref: HEAD
---

import struct


class Blob:
    def __init__(self):
        self._data = bytes()

    def append_int(self, value: int):
        self._data += struct.pack('<i', value)

    def append_float(self, value: float):
        self._data += struct.pack('<f', value)

    def append_byte(self, value):
        self._data += struct.pack('<B', value)

    def append_plain_str(self, value):
        self._data += value.encode('utf-8')
        self._data += b"\x00"

    def append_color(self, r, g, b):
        self.append_byte(b)
        self.append_byte(g)
        self.append_byte(r)

    def append_char4(self, value: str):
        value = value.encode('utf-8')
        assert len(value) == 4
        for b in value:
            self.append_byte(b)

    def __repr__(self):
        return repr(self._data)

    # def _fill(self, alignment=4):
    #     alignment = 4
    #     l = len(self._data)
    #     if l % alignment != 0:
    #         filler_byte_count = (8 - l % alignment)
    #         self._data += filler_byte_count * b'\x00'

    @property
    def data(self):
        # data = bytes(self._data)
        # if len(data) % 8 != 0:
        #     filler_byte_count = (8 - len(data) % 8)
        #     data += filler_byte_count * b'\x00'
        # return data
        return self._data

    def persist(self, file_path):
        with open(file_path, "wb") as f:
            f.write(self.data)
        print("wrote: " + file_path)
