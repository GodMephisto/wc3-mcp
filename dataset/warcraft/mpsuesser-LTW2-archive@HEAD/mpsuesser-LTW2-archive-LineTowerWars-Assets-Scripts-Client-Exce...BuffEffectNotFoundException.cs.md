---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Exceptions/BuffEffectNotFoundException.cs"
ref: HEAD
---

﻿public class BuffEffectNotFoundException : NotFoundException {
    public BuffEffectNotFoundException(BuffType buffType)
        : base($"A buff effect for buff of type {buffType} could not be found") {}
}