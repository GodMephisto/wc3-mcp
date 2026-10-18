---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "regions/test_regions.py"
ref: HEAD
---


from regions import parse_script, serialize_as_blob
from shared import Color, TestCase


class Test(TestCase):
    def _test(self, test_data: str, expected_hex: str, is_lua=True):
        parsed = parse_script(test_data.split("\n"), is_lua=is_lua, static_color=Color(128, 128, 255))
        w3r = serialize_as_blob(parsed)
        actual = w3r.data
        expected = self._from_hex(expected_hex)
        actual_hex = self._hex(actual)
        if not actual_hex == expected_hex:
            print("actual:\n" + actual_hex)
            print()
            print("expected:\n" + expected_hex)
        self.assertEqual(len(expected), len(actual), "data length")
        self.assertEqual(expected_hex, actual_hex)

    def test_parse(self):
        regions = parse_script("gg_rct_Region_000 = Rect(-64.0, -224.0, 192.0, 128.0)", is_lua=True)
        self.assertTrue(regions)
        self.assertEqual(regions[0].left, -64.0)

    def test_region1(self):
        expected = '05 00 00 00 01 00 00 00\n'\
                   '00 00 80 c2 00 00 60 c3\n'\
                   '00 00 40 43 00 00 00 43\n'\
                   '52 65 67 69 6f 6e 20 30\n'\
                   '30 30 00 00 00 00 00 00\n'\
                   '00 00 00 00 ff 80 80 ff'
        test_data = "gg_rct_Region_000 = Rect(-64.0, -224.0, 192.0, 128.0)"
        self._test(test_data, expected)
