---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "FourccToNumber/Program.cs"
ref: HEAD
---

﻿using JassToCsMain;
using System;
using System.Text.RegularExpressions;

namespace FourccToNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter: ");
                string line = Console.ReadLine();
                Console.WriteLine(GetResult(line));
            }
        }

        static string GetResult(string line)
        {
            int num;
            if (int.TryParse(line, out num))
            {
                return NumberConvertHelper.NumToFourcc(num);
            }
            else
            {
                line = Regex.Unescape(line);
                return NumberConvertHelper.FourccToNum(line).ToString();
            }
        }
    }
}
