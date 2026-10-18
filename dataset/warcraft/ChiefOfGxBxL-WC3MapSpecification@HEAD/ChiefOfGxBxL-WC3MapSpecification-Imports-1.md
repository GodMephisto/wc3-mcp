---
type: vendor-doc
source_repo: "https://github.com/ChiefOfGxBxL/WC3MapSpecification"
source_path: "Imports/1.md"
ref: HEAD
---

File structure:
* `int` File version = 0x1 (1)
* `int` Number of imports
  * `byte` Type (8=standard, 13=custom)
  * `string` Path