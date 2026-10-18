---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Exceptions/InvalidStateException.cs"
ref: HEAD
---

﻿using System;

public class InvalidStateException : Exception {
    public InvalidStateException(string message = "") : base(message) { }
}
