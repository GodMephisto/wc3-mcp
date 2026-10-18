---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/SendMenu/SendMenuTavern.cs"
ref: HEAD
---

﻿using UnityEngine;

public class SendMenuTavern : MonoBehaviour {
    public SendUnitOption[] CreepSendOptions => sendOptions;
    
    [SerializeField] private SendUnitOption[] sendOptions;
}