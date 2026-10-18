---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InputDelegationSystem/IHandleKeyContinuousInput.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public interface IHandleKeyContinuousInput : IHandleInput {
    HashSet<KeyCode> KeyContinuousSubscriptions { get; }
    bool HandleInputKeyContinuous(KeyCode kc);
}