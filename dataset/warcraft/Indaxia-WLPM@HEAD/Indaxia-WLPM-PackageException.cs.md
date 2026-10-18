---
type: vendor-doc
source_repo: "https://github.com/Indaxia/WLPM"
source_path: "PackageException.cs"
ref: HEAD
---

using System;

namespace wlpm
{
    public class PackageException: Exception
    {
        public PackageException()
        {
        }

        public PackageException(string message): base(message)
        {
        }
    }
}