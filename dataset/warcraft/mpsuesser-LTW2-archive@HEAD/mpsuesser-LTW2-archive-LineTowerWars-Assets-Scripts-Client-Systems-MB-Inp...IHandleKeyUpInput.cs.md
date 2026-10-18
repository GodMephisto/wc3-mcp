---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InputDelegationSystem/IHandleKeyUpInput.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public interface IHandleKeyUpInput : IHandleInput {
    HashSet<KeyCode> KeyUpSubscriptions { get; }
    bool HandleInputKeyUp(KeyCode kc);
}