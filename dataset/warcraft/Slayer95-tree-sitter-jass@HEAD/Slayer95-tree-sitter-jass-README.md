---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "README.md"
ref: HEAD
---

# tree-sitter-jass

A Tree-sitter grammar for the JASS scripting language used by Warcraft III.

## Overview

This package provides a parser for JASS built on top of Tree-sitter.

The grammar aims to parse valid JASS source code into a concrete syntax tree suitable for:

* Syntax highlighting
* Static analysis
* Refactoring tools
* Language servers
* Source code indexing
* Custom tooling

## Scope

This package performs **syntactic parsing only**.

It does **not** perform semantic validation, including:

* Name resolution
* Type checking
* Function signature validation
* Global initialization rules
* Native declaration validation
* Warcraft III API validation
* Compile-time constant evaluation

As a result, some files may parse successfully while still failing compilation in Warcraft III.

## Source Encoding

JASS source files are traditionally ANSI-encoded (single-byte character encoding).

This grammar operates on text supplied by the host application and does not enforce a specific code page. However, for compatibility with Warcraft III source files, the following assumptions apply:

* ANSI-encoded source files are supported.
* A UTF-8 BOM (`EF BB BF`) at the beginning of the file is tolerated.
* The byte values `0x00` and `0xFF` are invalid anywhere in the source file, including inside comments and string literals.

Files containing `0x00` or `0xFF` are considered malformed and are not valid JASS source files.

## Semantic Validation

If you need semantic validation, symbol resolution, type checking, or linting, see Prism:

* https://github.com/Slayer95/prism

Prism consumes the syntax trees produced by this grammar and performs higher-level analysis.

## Installation

For Node.js, the ABI 14 branch is required.

```bash
npm install github:Slayer95/tree-sitter-jass#abi-14
```

## Usage

Source files should typically be loaded using the `latin1` encoding:

```javascript
const Parser = require('tree-sitter');
const Jass = require('tree-sitter-jass');

const parser = new Parser();
parser.setLanguage(Jass);

const sourceCode = fs.readFileSync('war3map.j', 'latin1');
const tree = parser.parse(sourceCode);
```

## Project Repository

The full project repository contains additional tooling and language bindings:

https://github.com/Slayer95/tree-sitter-jass

## License

MIT
