---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Data/EventData/EventData.cs"
ref: HEAD
---

﻿using RiptideNetworking;

public abstract class EventData<T> {
    public abstract void WriteDataToMessage(ref Message message);
    // Read data should be `= new T(ref message)`
}