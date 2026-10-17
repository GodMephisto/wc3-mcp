---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "README.md"
ref: HEAD
---

[![](https://dcbadge.limes.pink/api/server/https://discord.gg/CNeQmXAgVq)](https://discord.gg/CNeQmXAgVq)

[Русская версия документации](README.ru.md)

# blp-lib

Rust library for working with BLP files (Blizzard texture format) with C/C++ interface for use in other projects.

## Description

This library provides a C-compatible API for working with BLP files, using the Rust crate [blp](https://crates.io/crates/blp). Supports loading BLP files from data buffer or filesystem and converting them to RGBA format.

## Compilation

```bash
cargo build --release
```

After compilation, local artifacts are placed in `target/release` (crate name based):
- `libblp_lib.a` - static library
- `libblp_lib.dylib` (macOS) / `libblp_lib.so` (Linux) / `blp_lib.dll` (Windows) - dynamic library

For cross-platform, ready-to-ship artifacts with simplified names, use the distribution builder below.

## Cross-Platform Distribution

Use `./build-only.sh` to create distribution packages for all platforms:

### macOS (Universal Binary)
- `libblp-macos.a` - static library (arm64 + x86_64)
- `libblp-macos.dylib` - dynamic library (arm64 + x86_64)

### Windows
- `libblp-windows.a` - static library
- `blp-windows.dll` - dynamic library

### Linux
- `libblp-linux.a` - static library (musl)

### Header File
- `blp.h` - C/C++ header file (cross-platform)

## Usage

### C/C++

Header files:
- Local developer build: include `include/blp_lib.h`
- Distribution build: include `dist/blp.h`

Example of loading a BLP file:
```c
#include <stdio.h>
#include <stdlib.h>
#include "blp.h"

int main() {
    BlpImage image;
    BlpResult result = blp_load_from_file("texture.blp", &image);
    
    if (result == BLP_SUCCESS) {
        printf("Loaded image %dx%d, data size: %d bytes\n", 
               image.width, image.height, image.data_len);
        
        // Use image.data (RGBA format)
        
        // Don't forget to free memory
        blp_free_image(&image);
    } else {
        printf("Error loading BLP file: %d\n", result);
    }
    
    return 0;
}
```

### Compiling C Project

#### Using Local Build
```bash
# Static linking (recommended for simple tests)
gcc -I./include -L./target/release your_program.c -o your_program -lblp_lib
```

#### Using Distribution Build

**macOS:**
```bash
gcc -I./dist -L./dist your_program.c -o your_program -lblp-macos -ldl
```

**Linux:**
```bash
gcc -I./dist -L./dist your_program.c -o your_program -lblp-linux -ldl -lpthread
```

**Windows:**
```bash
# Using MinGW (static)
gcc -I./dist -L./dist your_program.c -o your_program.exe -lblp-windows
```

## API

Note on mipmaps:
- By default, decoding loads only the base (top) mip for performance.
- Encoding APIs let you specify how many mip levels to generate or pass explicit flags.

### Structures

#### `BlpImage`
Structure for storing BLP image data:
- `uint32_t width` - image width
- `uint32_t height` - image height  
- `uint8_t* data` - pointer to data in RGBA format
- `uint32_t data_len` - data size in bytes

#### `BlpResult`
Operation result codes:
- `BLP_SUCCESS = 0` - operation completed successfully
- `BLP_INVALID_INPUT = -1` - invalid input parameters
- `BLP_PARSE_ERROR = -2` - BLP file parsing error
- `BLP_MEMORY_ERROR = -3` - memory allocation error
- `BLP_UNKNOWN_ERROR = -99` - unknown error

### Functions

#### `blp_load_from_buffer(data, data_len, out_image)`
Loads BLP file from data buffer into memory.

**Parameters:**
- `const uint8_t* data` - pointer to BLP file data
- `uint32_t data_len` - data length in bytes
- `BlpImage* out_image` - pointer to structure for storing result

**Returns:** `BlpResult`

#### `blp_load_from_file(filename, out_image)`
Loads BLP file from filesystem.

**Parameters:**
- `const char* filename` - path to BLP file
- `BlpImage* out_image` - pointer to structure for storing result

**Returns:** `BlpResult`

#### `blp_free_image(image)`
Frees memory allocated for `BlpImage`.

**Parameters:**
- `BlpImage* image` - pointer to structure to free

#### `blp_get_version()`
Returns library version string.

**Returns:** `const char*` - pointer to version string

#### `blp_is_valid(data, data_len)`
Checks if data buffer is a valid BLP file.

**Parameters:**
- `const uint8_t* data` - pointer to data to check
- `uint32_t data_len` - data length in bytes

**Returns:** `int` - 1 if valid BLP, 0 otherwise

### Encoding functions

Generate BLP from a source image (PNG/JPEG/etc.):

- `blp_encode_file_to_blp(input_path, output_path, quality, mip_count)`
    - Create a BLP with the first `mip_count` levels (min 1). `quality` is 0..100.

- `blp_encode_file_to_blp_with_flags(input_path, output_path, quality, mip_flags, mip_flags_len)`
    - `mip_flags` is an array of 0/1 values, each enabling a mip level by index.

- `blp_encode_bytes_to_blp(image_bytes, image_len, output_path, quality, mip_count)`
    - Encode image content already in memory.

- `blp_encode_bytes_to_blp_with_flags(image_bytes, image_len, output_path, quality, mip_flags, mip_flags_len)`
    - Same as above, but with explicit mip visibility flags.

### Decode / Extract helpers

- `blp_decode_mip_to_png_from_file(blp_path, mip_index, output_png_path)`
    - Decode selected mip into a PNG file. Loads only the requested mip.

- `blp_decode_mip_to_png_from_buffer(blp_bytes, blp_len, mip_index, output_png_path)`
    - Same as above, but takes the .blp data from memory.

- `blp_extract_mip_to_jpg_from_file(blp_path, mip_index, output_jpg_path)`
    - For JPEG-BLP: extract raw JPEG stream of the chosen mip without decoding.

- `blp_extract_mip_to_jpg_from_buffer(blp_bytes, blp_len, mip_index, output_jpg_path)`
    - Same as above for in-memory .blp data.

## Testing

```bash
cargo test
```

<p align="center">
  <img src="https://raw.githubusercontent.com/WarRaft/blp-lib/refs/heads/main/preview/logo.png" alt="BLP"/>
</p>