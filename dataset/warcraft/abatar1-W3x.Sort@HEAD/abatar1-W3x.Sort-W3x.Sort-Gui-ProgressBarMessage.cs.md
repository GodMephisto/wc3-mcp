---
type: vendor-doc
source_repo: "https://github.com/abatar1/W3x.Sort"
source_path: "W3x.Sort/Gui/ProgressBarMessage.cs"
ref: HEAD
---

﻿namespace W3x.Sort.Gui
{
    public class ProgressBarMessage
    {
        public ProgressBarMessage(double value, string text)
        {
            Value = value;
            Text = text;
        }

        public double Value { get; }
        public string Text { get; }
    }
}
