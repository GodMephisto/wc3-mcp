---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "shared/testcase.py"
ref: HEAD
---

import unittest

class TestCase(unittest.TestCase):
    @staticmethod
    def _hex(data: bytes):
        block_size = 8
        separator = " "
        result = []
        block = []
        for c in data.hex(separator, 1).split(separator):
            block.append(c)
            if len(block) == block_size:
                result.append(separator.join(block))
                block = []
        if block:
            while len(block) != block_size:
                block.append("  ")
            result.append("'".join(block))
        return "\n".join(result)

    @staticmethod
    def _from_hex(hex_str: str):
        return bytes.fromhex(hex_str.replace(" ", "").replace("\n", ""))