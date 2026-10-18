---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Exceptions/EntityNotFoundException.cs"
ref: HEAD
---

﻿public class EntityNotFoundException : NotFoundException {
    public EntityNotFoundException(int entityID)
        : base($"An entity with ID {entityID} could not be found") {}
}