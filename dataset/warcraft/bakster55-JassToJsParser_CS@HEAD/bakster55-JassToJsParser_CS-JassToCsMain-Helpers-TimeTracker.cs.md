---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/TimeTracker.cs"
ref: HEAD
---

﻿using System;

namespace JassToCsMain
{
    public class TimeTracker : IDisposable
    {
        private DateTime dateTime;
        private string name;

        public TimeTracker(string name)
        {
            this.dateTime = DateTime.Now;
            this.name = name;
        }

        public void Dispose()
        {
            Console.WriteLine($"{name}: {DateTime.Now - dateTime}");
        }
    }
}
