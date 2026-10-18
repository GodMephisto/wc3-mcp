---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Main/Program.cs"
ref: HEAD
---

﻿using Ninject;
using System.IO;

namespace JassToCsMain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IKernel kernel = GetKernel(new object());

            string basePath = @"E:\JassToCsMain\JassToCsMain\War3Map";
            //File.WriteAllText($@"{basePath}\common.js", kernel.Get<Parser>().Parse($@"{basePath}\common.j"));
            File.WriteAllText($@"{basePath}\blizzard.js", kernel.Get<Parser>().Parse($@"{basePath}\blizzard.j"));

            //string baseJassPath = @"E:\JassToCsMain\JASS_TO_JS_PARSER\war3map\LOM_RPG_2.38A_translated";
            //string parsed = kernel.Get<Parser>().Parse($@"{baseJassPath}\war3map.j");
            //File.WriteAllText($@"{baseJassPath}\war3map.js", parsed);

            //Test();
        }

        public static IKernel GetKernel(object scope)
        {
            var kernel = new StandardKernel();
            kernel.Bind<Parser>().ToSelf().InScope((c) => scope);
            kernel.Bind<JassVisitor>().ToSelf().InScope((c) => scope);
            kernel.Bind<Helper>().ToSelf().InScope((c) => scope);
            kernel.Bind<FuncHelper>().ToSelf().InScope((c) => scope);

            return kernel;
        }

        public static void Test()
        {
            string baseWar3mapPath = @"F:\JassToCsMain\JASS_TO_JS_PARSER\war3map";
            string baseTestPath = @"F:\JassToCsMain\JASS_TO_JS_PARSER\test";
            string[] subdirectoryPathList = Directory.GetDirectories(baseWar3mapPath);

            foreach (string subdirectoryPath in subdirectoryPathList)
            {
                string subdirectoryName = subdirectoryPath.Remove(0, baseWar3mapPath.Length);
                string fileName = subdirectoryPath + "\\war3map.j";
                string content = new Parser().Parse(fileName);

                string baseTestFilePath = baseTestPath + subdirectoryName + ".js";
                File.WriteAllText(baseTestFilePath, content);
            }
        }
    }
}
