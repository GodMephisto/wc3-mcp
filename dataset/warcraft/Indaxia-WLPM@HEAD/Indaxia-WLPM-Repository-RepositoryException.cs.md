---
type: vendor-doc
source_repo: "https://github.com/Indaxia/WLPM"
source_path: "Repository/RepositoryException.cs"
ref: HEAD
---

using System;

namespace wlpm.Repository
{
    public class RepositoryException: Exception
    {
        public RepositoryException()
        {
        }

        public RepositoryException(string message): base(message)
        {
        }
    }
}