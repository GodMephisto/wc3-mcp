---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "README.md"
ref: HEAD
---

# War3Mdlx

A command line tool to convert warcraft 3 model file: *.mdl <-> *.mdx

## Usage

```bash
# convert mdl to mdx (omit output path)
$ war3mdlx input.mdl
# convert mdx to mdl
$ war3mdlx input.mdx output.mdl
# convert mdx to mdl (walk through directory hierarchy)
$ war3mdlx --mdx2l input/path output/path
# all available options
$ war3mdlx -h
Usage: war3mdlx [OPTIONS] <INPUT>

Options:
  -1, --mdl2x                     Convert *.mdl to *.mdx
  -2, --mdx2l                     Convert *.mdx to *.mdl
  -B, --mdl-rgb                   Make sure colors are in RGB order in mdl files
  -F, --flat                      Put output files in one directory and ignore hierarchy
  -f, --overwrite                 Overwrite existing output files [default: skip]
  -e, --stop-on-error             Stop walking the directory hierarchy when an error occurs
  -d, --max-depth <0..255>        Max depth of directory traversal [default: 255]
  -p, --precision <0..255>        Max precision of decimal numbers when converted to text [default: 4]
  -n, --line-ending <CR|LF|CRLF>  Used when writing text files [default: CRLF]
  -i, --indent <Ns|Nt>            Used when writing text files (e.g. 1t: one tab, 4s: four spaces) [default: 1t]
  -q, --quiet                     Do not print log messages
  -v, --verbose...                Print verbose log messages (-vv very verbose)
  -h, --help                      Print help
  -V, --version                   Print version
```

## Install

```bash
$ cargo install War3Mdlx
```

> You may need to install [rust and cargo](https://rust-lang.org/tools/install/).

## Build

When you are in the root directory of the project:
```bash
# build release
$ cargo build --release
# build for windows
$ cargo build --release --target x86_64-pc-windows-gnu
# build for linux
$ cargo build --release --target x86_64-unknown-linux-musl
```

> You may need to install [rust and cargo](https://rust-lang.org/tools/install/).
