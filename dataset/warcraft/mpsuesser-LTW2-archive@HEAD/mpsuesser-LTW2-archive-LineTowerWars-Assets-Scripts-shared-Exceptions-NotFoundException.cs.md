---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Exceptions/NotFoundException.cs"
ref: HEAD
---

using System;

public class NotFoundException : Exception
{
    public NotFoundException(string message = "") : base(message) { }
}

public class ResourceNotFoundException : NotFoundException
{
    public ResourceNotFoundException(string message = "") : base(message) { }
}
