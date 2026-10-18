---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToJsMain.UnitTest/TestBase.cs"
ref: HEAD
---

﻿using JassToCsMain;
using Ninject;
using NUnit.Framework;
using System;
using System.IO;
using static JassParser;

namespace JassToJsMain.UnitTest
{
    public abstract class TestBase
    {
        protected abstract string FolderName { get; }

        private static bool isInited = false;

        public void Execute()
        {
            var projectBinDebugDirectoryInfo = Directory.GetParent(AppContext.BaseDirectory);
            var projectDirectoryInfo = projectBinDebugDirectoryInfo.Parent.Parent;

            var stackFrame = new System.Diagnostics.StackFrame(1, true);
            var currentMethodName = stackFrame.GetMethod().Name;
            var testDirectoryPath = Path.Combine(projectDirectoryInfo.FullName, FolderName, currentMethodName);

            var exampleFilePath = Path.Combine(testDirectoryPath, "Example.j");
            var expectedFilePath = Path.Combine(testDirectoryPath, "Expected.js");

            IKernel kernel = Program.GetKernel(new object());
            var parser = kernel.Get<Parser>();

            if (!isInited)
            {
                isInited = true;

                parser.ParseNativeFiles();
            }

            var actualFileContent = parser.ParseInternal(exampleFilePath, kernel.Get<JassVisitor>());
            var expectedFileContent = File.ReadAllText(expectedFilePath);

            Assert.AreEqual(expectedFileContent, actualFileContent);
        }
    }
}
