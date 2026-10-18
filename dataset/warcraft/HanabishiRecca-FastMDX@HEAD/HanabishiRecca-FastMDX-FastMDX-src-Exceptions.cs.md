---
type: vendor-doc
source_repo: "https://github.com/HanabishiRecca/FastMDX"
source_path: "FastMDX/src/Exceptions.cs"
ref: HEAD
---

﻿using System;

namespace FastMDX {
    class ParsingException : Exception {
        public override string Message => "Parsing error.";
    }
}
