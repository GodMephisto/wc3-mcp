---
type: vendor-doc
source_repo: "https://github.com/mrsho240/war3j2b"
source_path: "README.md"
ref: HEAD
---

# War3MapScriptRepacker

A small Python utility for **extracting, modifying, and repacking JASS scripts** from Warcraft III map binary data.

This project was created as an older **Reverse Engineering experiment** for studying how compressed `war3map.j` script data can be extracted from and written back into `war3map.bin`.

> **Note:** This is an experimental project and is mainly intended for learning and research.

## Features

* Compress a JASS script into a binary file using Zlib
* Detect Zlib-compressed blocks inside `war3map.bin`
* Extract the original JASS script
* Record the structure of compressed blocks
* Save block information as JSON metadata
* Modify the extracted JASS script
* Recompress modified blocks
* Replace compressed data inside the original binary
* Preserve unrelated data in the original binary

## Workflow

The general workflow is:

```text
             ┌──────────────────────┐
             │     war3map.bin      │
             └──────────┬───────────┘
                        │
                        ▼
              Find Zlib compressed
                    blocks
                        │
                        ▼
             ┌──────────────────────┐
             │      war3map.j       │
             └──────────┬───────────┘
                        │
                    Modify JASS
                        │
                        ▼
             ┌──────────────────────┐
             │  Recompress blocks   │
             └──────────┬───────────┘
                        │
                        ▼
             ┌──────────────────────┐
             │  Rebuilt .bin file   │
             └──────────────────────┘
```

## 🛠️ Requirements

* Python 3
* Python standard library

No external packages are required.

The project uses:

```python
import zlib
import json
```

## 📦 Basic Compression

The `revert_to_bin()` function can compress a JASS script into a binary file.

```python
revert_to_bin(
    "war3map.j",
    "war3map.bin"
)
```

The default compression level is `6`.

```python
zlib.compress(raw_data, compression_level)
```

The script also prints the resulting Zlib magic bytes.

Common headers used by the script are:

| Compression Level | Zlib Header |
| ----------------: | ----------- |
|               `6` | `78 9C`     |
|               `9` | `78 DA`     |

## 🔍 Extracting the Script

The `extract_and_save_structure()` function searches the binary for Zlib streams.

```python
extract_and_save_structure(
    "war3map.bin",
    "war3map.j",
    "metadata.json"
)
```

For every detected block, the tool records:

```json
{
  "offset": 1234,
  "compressed_size": 500,
  "uncompressed_size": 1200,
  "magic": "789c"
}
```

The metadata contains:

* Original binary size
* Block offset
* Compressed size
* Uncompressed size
* Zlib magic bytes

This information is later used when rebuilding the binary.

## 🧩 Repacking Modified JASS

After modifying the extracted `war3map.j`, the `revert_with_meta()` function can recompress the modified data and replace the corresponding blocks in the original binary.

```python
revert_with_meta(
    "war3map.bin",
    "modified.j",
    "metadata.json",
    "modified.bin"
)
```

The tool uses the original block's compression type:

```python
level = 9 if block["magic"] == "78da" else 6
```

The modified data is then written back to the original block location.



## 🔬 Reverse Engineering

This project was created while experimenting with the internal structure of Warcraft III map data.

A Warcraft III map can contain a `war3map.j` file, which is the map's main JASS script.

The purpose of this project was to understand how compressed script data could be located, decompressed, modified, and reconstructed without implementing a complete Warcraft III map parser.

The project focuses on:

* Binary analysis
* Hexadecimal data
* Zlib compression
* Compressed data blocks
* File offsets
* Binary patching
* JASS scripts
* Reverse Engineering

## ⚠️ Limitations

This is an experimental implementation and **does not implement the complete Warcraft III map format**.

In particular:

* It searches for known Zlib headers rather than fully parsing the map container.
* It assumes the relevant script data can be identified through these compressed streams.
* Repacked data may have different compressed sizes from the original.
* The tool does not validate the complete map structure after modification.
* Different Warcraft III versions or protected maps may use different layouts.

Therefore, always keep a backup of the original file before attempting to rebuild it.

## 📚 Background

In Warcraft III maps, `war3map.j` contains the main JASS script used by the map. Depending on the map and protection method, the script may be stored or relocated in ways that make direct extraction more difficult.

This project was an early experiment in understanding that process at the binary level.



## 📜 Disclaimer

This project is provided for **educational and research purposes**.

Use it only with files and maps that you have permission to analyze or modify.
