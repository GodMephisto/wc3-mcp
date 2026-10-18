---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/ProcessHelper.cs"
ref: HEAD
---

﻿using System.Diagnostics;
using System.Windows;

namespace vJassMainJBlueprint.Utils
{
    public static class ProcessHelper
    {
        public static void Open(Window owner, string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, $"파일을 여는 도중 오류가 발생했습니다: {ex.Message}", "파일 열기");
            }
        }
    }
}
