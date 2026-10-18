---
type: vendor-doc
source_repo: "https://github.com/Indaxia/WLPM"
source_path: "ModuleException.cs"
ref: HEAD
---

using System;

namespace wlpm
{
    public class ModuleException: Exception
    {
        public ModuleException()
        {
        }

        public ModuleException(string message): base(message)
        {
        }
    }
}