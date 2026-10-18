---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToJsMain.UnitTest/Array/ArrayTest.cs"
ref: HEAD
---

﻿using NUnit.Framework;

namespace JassToJsMain.UnitTest.Array
{
    [TestFixture]
    public class ArrayTest : TestBase
    {
        protected override string FolderName => "Array";

        [Test]
        public void Test1()
        {
            base.Execute();
        }
    }
}
