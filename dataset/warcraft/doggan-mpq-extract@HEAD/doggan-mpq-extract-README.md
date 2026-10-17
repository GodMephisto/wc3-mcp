---
type: vendor-doc
source_repo: "https://github.com/doggan/mpq-extract"
source_path: "README.md"
ref: HEAD
---

# mpq-extract
A tool for extracting files from MPQ archives.

## Installation
`npm install mpq-extract -g`

## Usage
```
  Usage: mpq-extract [options] <mpq> <listfile>

  Options:

    -h, --help         output usage information
    -V, --version      output the version number
    -o, --dest <path>  the folder to extract files to (default: current directory)
    -f, --fullpath     preserve the full path hierarchy of the extracted files.
    -c, --lowercase    convert extracted file paths to lowercase.
    -v, --verbose      verbose output
```
