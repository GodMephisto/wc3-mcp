---
type: vendor-doc
source_repo: "https://github.com/mrsho240/war3map.bin-extracter"
source_path: "README.md"
ref: HEAD
---

# 🔓 Warcraft III Map Script Extractor

A small Python utility for extracting and decompressing **JASS script data** from an encoded or compressed `war3map.bin` file.

This project was created as an older **Reverse Engineering experiment** while investigating the internal structure of Warcraft III map files and how map scripts can be stored and compressed.

## 🛠️ Technologies

* **Python 3**
* `zlib`
* `re`

## 📌 Features

* Read hexadecimal data exported from `war3map.bin`
* Convert hexadecimal data back into binary
* Search for Zlib compressed blocks
* Support common Zlib headers:

  * `78 9C` — Default compression
  * `78 DA` — Best compression
* Decompress discovered Zlib streams
* Combine extracted data into a single JASS script
* Save the resulting script as a `.j` file

## ⚙️ How It Works

The tool follows a simple extraction pipeline:

```text
war3map.bin
     │
     ▼
Read hexadecimal data
     │
     ▼
Convert HEX → Binary
     │
     ▼
Search for Zlib blocks
     │
     ├── 78 9C
     └── 78 DA
     │
     ▼
Decompress Zlib streams
     │
     ▼
Combine decompressed blocks
     │
     ▼
war3map_unprotected.j
```

### 1. Read the Input

The script reads the hexadecimal representation of `war3map.bin` and removes offset information.

```python
parts = line.split('\t')

if len(parts) >= 2:
    hex_string = parts[1].replace(" ", "").strip()
```

The collected hexadecimal data is then converted back into raw binary:

```python
binary_data = bytes.fromhex(hex_data)
```

### 2. Find Zlib Streams

The program searches the binary data for common Zlib headers:

```python
binary_data.find(b'\x78\x9c')
binary_data.find(b'\x78\xda')
```

These offsets are used as possible starting points for compressed data.

### 3. Decompress the Data

Each discovered block is passed to Python's Zlib decompressor:

```python
decompressor = zlib.decompressobj()

uncompressed_data = decompressor.decompress(
    binary_data[start_idx:]
)
```

Successfully decompressed blocks are stored for later reconstruction.

### 4. Reconstruct the JASS Script

All extracted blocks are concatenated:

```python
final_plaintext = b"".join(plaintext_blocks)
```

The result is written to:

```text
war3map_unprotected.j
```

## 🚀 Usage

Place the input file in the project directory and run:

```bash
python extract.py
```

The script expects:

```text
war3map.bin
```

and produces:

```text
war3map_unprotected.j
```

The filenames can also be changed directly in the script:

```python
extract_map_script(
    "war3map.bin",
    "war3map_unprotected.j"
)
```

## 📂 Project Structure

```text
.
├── extract.py
├── war3map.bin
├── war3map_unprotected.j
└── README.md
```

## 🔬 Reverse Engineering Notes

This project was mainly created to explore how **Warcraft III map data** can be analyzed and reconstructed.

Rather than relying on a dedicated map editor, the script works directly with binary data and attempts to locate compressed streams based on their Zlib signatures.

This was an early experiment in understanding:

* Binary file formats
* Compression
* Hexadecimal data
* Memory/data offsets
* JASS scripts
* Reverse Engineering techniques

## ⚠️ Limitations

This is an experimental tool rather than a complete Warcraft III map parser.

It assumes that the input data contains recognizable Zlib streams using the searched headers. Maps using a different structure, compression method, encryption scheme, or data layout may require additional analysis.

The original code is preserved as an example of an earlier Reverse Engineering project.

## 📜 Disclaimer

This project is intended for **educational and research purposes**, particularly for learning about binary formats, compression, and Reverse Engineering.
