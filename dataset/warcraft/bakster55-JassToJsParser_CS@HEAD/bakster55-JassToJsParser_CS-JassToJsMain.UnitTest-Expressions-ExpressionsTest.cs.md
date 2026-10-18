---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToJsMain.UnitTest/Expressions/ExpressionsTest.cs"
ref: HEAD
---

﻿using NUnit.Framework;

namespace JassToJsMain.UnitTest.Expressions
{
    [TestFixture]
    public class ExpressionsTest : TestBase
    {
        protected override string FolderName => "Expressions";

        [Test]
        public void Test1()
        {
            base.Execute();
        }

        [Test]
        public void Test2()
        {
            base.Execute();
        }
    }
}
