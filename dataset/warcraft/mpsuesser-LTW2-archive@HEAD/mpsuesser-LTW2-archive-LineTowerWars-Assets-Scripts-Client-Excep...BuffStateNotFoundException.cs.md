---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Exceptions/BuffStateNotFoundException.cs"
ref: HEAD
---

﻿public class BuffStateNotFoundException : NotFoundException {
    public BuffStateNotFoundException(int buffID)
        : base($"A buff state for buff with ID {buffID} could not be found") {}
}