---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "units/test_units.py"
ref: HEAD
---

from shared import TestCase
from units import parse_script, serialize_as_blob


class Test(TestCase):
    def _test(self, test_data: str, expected_hex: str, is_lua=True):
        parsed = parse_script(test_data.split("\n"), is_lua=is_lua)
        w3_units_doo = serialize_as_blob(parsed)
        actual = w3_units_doo.data
        expected = self._from_hex(expected_hex)
        actual_hex = self._hex(actual)
        if not actual_hex == expected_hex:
            print("actual:\n" + actual_hex)
            print()
            print("expected:\n" + expected_hex)
        self.assertEqual(len(expected), len(actual), "data length")
        self.assertEqual(expected_hex, actual_hex)

    def test_parse(self):
        units = parse_script("local player p= Player(22)\n"
                             "    set u=BlzCreateUnitWithSkin(p, 'hpea', - 191.2, 75.9, 277.402, 'hpea')",
                             is_lua=False)
        self.assertTrue(units)
        self.assertEqual(units[0].type_id, 'hpea')
        self.assertEqual(units[0].owner, 22)

