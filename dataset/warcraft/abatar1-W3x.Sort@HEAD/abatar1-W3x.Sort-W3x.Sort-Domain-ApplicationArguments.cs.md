---
type: vendor-doc
source_repo: "https://github.com/abatar1/W3x.Sort"
source_path: "W3x.Sort/Domain/ApplicationArguments.cs"
ref: HEAD
---

﻿namespace W3x.Sort.Domain
{
    public class ApplicationArguments
    {
        public string Key { get; set; }
        public Mode Mode { get; set; }
        public string[] Files { get; set; }
    }

    public enum Mode
    {
        All,
        ByName,
        Ignore
    }
}
