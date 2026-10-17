---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "README.md"
ref: HEAD
---

# stormlib-js

Pure TypeScript implementation of [StormLib](https://github.com/ladislav-zezula/StormLib) for reading MPQ (Mo'PaQ) archives. No native bindings, no WASM — just TypeScript.

MPQ is the archive format used by Blizzard Entertainment games including Diablo, StarCraft, Warcraft III, and World of Warcraft.

## Features

- **Read-only** MPQ archive support
- **MPQ V1 through V4** format support
- **Pure TypeScript** — no native addons or WASM compilation required
- **Synchronous API** — simple, straightforward file extraction
- **Single runtime dependency** — only [pako](https://github.com/nickolaskgas/pako) for zlib
- **Dual module output** — ships both CommonJS and ESM builds with full type declarations

### Compression algorithms

| Algorithm | Status |
|---|---|
| zlib (Deflate) | Supported |
| PKWARE DCL (Implode) | Supported |
| Blizzard Huffman | Supported |
| ADPCM Mono / Stereo | Supported |
| Sparse (RLE) | Supported |
| BZIP2 | Not supported |
| LZMA | Not supported |

> BZIP2 and LZMA are rarely used in practice (only some SC2/HotS archives). If you need them, contributions are welcome.

### Table formats

| Table | Status |
|---|---|
| Classic Hash Table | Supported |
| Classic Block Table | Supported |
| Hi-Block Table (V2+) | Supported |
| HET Table (V3+) | Supported |
| BET Table (V3+) | Supported |

## Installation

```bash
npm install stormlib-js
```

## Quick start

```typescript
import { MpqArchive } from 'stormlib-js';

// Open an archive
const archive = MpqArchive.open('game.mpq');

// List known files (from internal listfile)
const files = archive.getFileList();
console.log(files);

// Extract a file
const data = archive.extractFile('war3map.j');
console.log(data.toString('utf-8'));

// Search with wildcards
const results = archive.findFiles('*.blp');
for (const entry of results) {
  console.log(entry.fileName, entry.fileSize);
}

// Clean up
archive.close();
```

Browser or in-memory usage:

```typescript
import { MpqArchive } from 'stormlib-js/browser';

const file = input.files?.[0];
if (!file) throw new Error('No file selected');

const arrayBuffer = await file.arrayBuffer();
const archive = MpqArchive.openFromBuffer(arrayBuffer);
const war3mapJ = archive.extractFile('war3map.j');
archive.close();
```

`stormlib-js/browser` is browser-safe and does not include Node `fs` loading code.

## API

### `MpqArchive`

The main class for working with MPQ archives.

#### `MpqArchive.open(path, options?)`

Opens an MPQ archive from a file path.

> In `stormlib-js/browser`, `MpqArchive.open()` throws by design. Use `openFromBuffer()`.

```typescript
const archive = MpqArchive.open('archive.mpq');

// With options
const archive = MpqArchive.open('archive.mpq', {
  noListfile: false,    // Skip loading internal (listfile)
  noAttributes: false,  // Skip loading internal (attributes)
  noHeaderSearch: false, // Don't search for MPQ header (assume offset 0)
  forceMpqV1: false,    // Force reading as MPQ V1
});
```

#### `MpqArchive.openFromBuffer(data, options?)`

Opens an MPQ archive from in-memory binary data (`ArrayBuffer`, `Uint8Array`, or `Buffer`).

```typescript
import { MpqArchive } from 'stormlib-js/browser';

const arrayBuffer = await file.arrayBuffer();
const archive = MpqArchive.openFromBuffer(arrayBuffer);
```

#### `archive.hasFile(name)`

Returns `true` if the file exists in the archive.

```typescript
if (archive.hasFile('war3map.j')) {
  // ...
}
```

#### `archive.extractFile(name)`

Extracts a file and returns its contents as a `Buffer`.

```typescript
const buf = archive.extractFile('war3map.w3i');
```

#### `archive.openFile(name)`

Opens a file handle for more control over reading.

```typescript
const file = archive.openFile('units.doo');
console.log(file.size, file.compressedSize, file.flags);
const data = file.read();
file.close();
```

#### `archive.getFileList()`

Returns an array of known file names (populated from the internal listfile).

```typescript
const names = archive.getFileList();
// ['war3map.j', 'war3map.w3e', ...]
```

#### `archive.findFiles(mask?)`

Searches for files matching a wildcard pattern. Supports `*` and `?`.

```typescript
const textures = archive.findFiles('*.blp');
for (const f of textures) {
  console.log(f.fileName, f.fileSize, f.compSize);
}
```

#### `archive.enumerateFiles()`

Lists all file entries including unnamed ones. Unnamed entries are given synthetic names like `File00000001.xxx`.

```typescript
const all = archive.enumerateFiles();
console.log(`Total: ${all.length} files`);
```

#### `archive.addListfile(names)`

Applies an external list of file names to resolve unnamed entries. Useful for archives without an internal `(listfile)`.

```typescript
const resolved = archive.addListfile([
  'war3map.j', 'war3map.w3e', 'war3map.w3i',
]);
console.log(`${resolved} new names resolved`);
```

#### `archive.getHeader()`

Returns the parsed MPQ header.

```typescript
const header = archive.getHeader();
console.log(`Format: V${header.wFormatVersion + 1}`);
console.log(`Sector size: ${512 << header.wSectorSize}`);
```

#### `archive.close()`

Closes the archive and releases the underlying file handle.

### Error classes

All errors extend `MpqError`:

| Class | Description |
|---|---|
| `MpqNotFoundError` | File not found in the archive |
| `MpqCorruptError` | Archive data is corrupt |
| `MpqUnsupportedError` | Unsupported feature (e.g. BZIP2) |
| `MpqEncryptionError` | Decryption failure |
| `MpqCompressionError` | Decompression failure |

```typescript
import { MpqNotFoundError } from 'stormlib-js';

try {
  archive.extractFile('nonexistent.txt');
} catch (err) {
  if (err instanceof MpqNotFoundError) {
    console.log('File does not exist');
  }
}
```

## Supported games

This library can read MPQ archives from:

- **Diablo** / **Diablo II** (V1, Huffman/PKWARE compression)
- **StarCraft** / **Brood War** (V1, Huffman/PKWARE compression)
- **Warcraft III** / **The Frozen Throne** (V1, zlib compression)
- **World of Warcraft** (V2, 64-bit offsets)
- **StarCraft II** (V3/V4, HET/BET tables)

## Advanced usage

Low-level crypto and hashing functions are exported for specialized use cases:

```typescript
import {
  hashString, jenkinsHash,
  decryptBlock, encryptBlock,
  getStormBuffer,
  MPQ_HASH_TABLE_INDEX,
  MPQ_HASH_NAME_A,
} from 'stormlib-js';

// Compute MPQ hash
const idx = hashString('(listfile)', MPQ_HASH_TABLE_INDEX);

// Jenkins hash for HET tables
const hash = jenkinsHash('war3map.j');
```

## Development

```bash
# Install dependencies
npm install

# Run tests
npm test

# Type check
npm run typecheck

# Build
npm run build
```

## Acknowledgments

- [Ladislav Zezula](https://github.com/ladislav-zezula) for the original [StormLib](https://github.com/ladislav-zezula/StormLib) C/C++ library and the comprehensive MPQ format documentation
- [The StormLib documentation](http://www.zezula.net/en/mpq/stormlib.html) for format specifications

## License

MIT
