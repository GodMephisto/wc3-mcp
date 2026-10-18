---
type: vendor-doc
source_repo: "https://github.com/ChiefOfGxBxL/WC3MapSpecification"
source_path: "Regions/5.md"
ref: HEAD
---

File structure:
* `int` File version = 0x5 (5)
* `int` Number of regions
  * `float` Position left
  * `float` Position bottom
  * `float` Position right
  * `float` Position top
  * `string` Region name
  * `int` Region id
  * `char(4)` Weather effect
  * `string` Ambient sound
  * `byte` Color - blue
  * `byte` Color - green
  * `byte` Color - red
  * `unknown(1)` (always 255?)
