---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Exceptions/BuffDescriptorNotFoundException.cs"
ref: HEAD
---

﻿public class BuffDescriptorNotFoundException : NotFoundException {
    public BuffDescriptorNotFoundException(BuffType buffType)
        : base($"A buff descriptor for buff of type {buffType} could not be found") {}
}