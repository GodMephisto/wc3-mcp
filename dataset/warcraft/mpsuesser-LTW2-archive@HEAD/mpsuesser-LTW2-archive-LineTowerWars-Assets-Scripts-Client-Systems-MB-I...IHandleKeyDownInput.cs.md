---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InputDelegationSystem/IHandleKeyDownInput.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;

public interface IHandleKeyDownInput : IHandleInput {
    HashSet<KeyCode> KeyDownSubscriptions { get; }
    bool HandleInputKeyDown(KeyCode kc);
}
