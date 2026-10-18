---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Infrastructure/DelegateUtil.cs"
ref: HEAD
---

﻿using System;

namespace BananaMpq.View.Infrastructure
{
    public static class DelegateUtil
    {
        public static T ConvertTo<T>(this Delegate d) where T: class
        {
            return (T)(object)Delegate.CreateDelegate(typeof(T), d.Target, d.Method);
        }
    }
}