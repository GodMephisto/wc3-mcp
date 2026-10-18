---
type: vendor-doc
source_repo: "https://github.com/mrsho240/war3j2b"
source_path: "j2b-revert.py"
ref: HEAD
---

import zlib

def revert_to_bin(input_j, output_bin, compression_level=6):
    with open(input_j, "rb") as f:
        raw_data = f.read()

    compressed = zlib.compress(raw_data, compression_level)
    # level 6 = Default (78 9C), level 9 = Best (78 DA)

    with open(output_bin, "wb") as f:
        f.write(compressed)

    print(f"Done! {len(raw_data)} bytes → {len(compressed)} bytes compressed")
    print(f"Magic bytes: {compressed[:2].hex().upper()}")
import zlib, re, json

def extract_and_save_structure(input_bin, output_j, output_meta):
    with open(input_bin, "rb") as f:
        binary_data = f.read()

    blocks = []
    i = 0

    while i < len(binary_data):
        idx1 = binary_data.find(b'\x78\x9c', i)
        idx2 = binary_data.find(b'\x78\xda', i)
        possible = [x for x in (idx1, idx2) if x != -1]
        if not possible:
            break
        start = min(possible)

        try:
            dec = zlib.decompressobj()
            uncompressed = dec.decompress(binary_data[start:])
            consumed = len(binary_data[start:]) - len(dec.unused_data)

            blocks.append({
                "offset": start,             # ตำแหน่งใน bin เดิม
                "compressed_size": consumed,  # ขนาดหลัง compress
                "uncompressed_size": len(uncompressed),
                "magic": binary_data[start:start+2].hex()
            })

            i = start + consumed
        except zlib.error:
            i = start + 2

    # บันทึก metadata
    with open(output_meta, "w") as f:
        json.dump({
            "total_size": len(binary_data),
            "blocks": blocks
        }, f, indent=2)

    # บันทึก j file เหมือนเดิม
    all_data = b""
    for b in blocks:
        dec = zlib.decompressobj()
        all_data += dec.decompress(binary_data[b["offset"]:])
    
    with open(output_j, "wb") as f:
        f.write(all_data)

    print(f"Saved metadata to {output_meta}")
def revert_with_meta(original_bin, modified_j, meta_file, output_bin):
    with open(original_bin, "rb") as f:
        binary_data = bytearray(f.read())
    with open(modified_j, "rb") as f:
        j_data = f.read()
    with open(meta_file, "r") as f:
        meta = json.load(f)

    blocks = meta["blocks"]
    # ถ้า block เดียว ก็ compress ทั้งหมด
    # ถ้าหลาย block ต้อง split j_data ให้ตรงกับ uncompressed_size เดิม
    
    offset_in_j = 0
    for block in blocks:
        chunk = j_data[offset_in_j : offset_in_j + block["uncompressed_size"]]
        level = 9 if block["magic"] == "78da" else 6
        compressed = zlib.compress(chunk, level)

        # แทนที่ใน binary เดิม
        start = block["offset"]
        old_size = block["compressed_size"]
        binary_data[start:start+old_size] = compressed

        offset_in_j += block["uncompressed_size"]

    with open(output_bin, "wb") as f:
        f.write(binary_data)
    print("Reverted successfully!")
revert_to_bin("war3map.j", "war3map.bin")
