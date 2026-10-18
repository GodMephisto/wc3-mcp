---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Config/PatchConfig.cs"
ref: HEAD
---

﻿using System.IO;

namespace TACTLib.Config {
    public class PatchConfig : Config {
        public PatchConfig(Stream? stream) : base(stream) {
            
        }
    }
}