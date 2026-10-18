---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Config/CDNConfig.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.IO;

namespace TACTLib.Config {
    public class CDNConfig : Config {
        public CDNConfig(Stream? stream) : base(stream) { }

        public List<string> Archives => Values["archives"];
    }
}