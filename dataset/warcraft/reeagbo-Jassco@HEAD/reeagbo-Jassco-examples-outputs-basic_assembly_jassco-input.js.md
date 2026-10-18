---
type: vendor-doc
source_repo: "https://github.com/reeagbo/Jassco"
source_path: "examples/outputs/basic_assembly_jassco/input.js"
ref: HEAD
---

﻿// jassco: include("io.asc")
var result = 0
result = 2 + 3
console.log("Basic JavaScript result (5): ", result)

assembly {
    call basic_marker
    jp basic_continue

basic_marker:
    ld a, "!"
    rst 16
    ret

basic_continue:
}

console.log("")
console.log("Back in JavaScript")

