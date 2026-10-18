---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelHelper/JassProjectOpenFileDialog.cs"
ref: HEAD
---

﻿using Microsoft.Win32;

namespace vJassMainJBlueprint.V1.ModelHelper
{
    internal class JassProjectOpenFileDialog
    {
        private static readonly OpenFileDialog openFileDialog = new()
        {
            Filter = "vJass Script files (*.j)|*.j|All files (*.*)|*.*", // 파일 필터 설정
            Title = "파일 열기" // 대화상자 제목 설정
        };

        /// <summary>
        /// Displays the open file dialog and returns the selected file path.
        /// </summary>
        /// <returns>The selected file path, or null if no file is selected.</returns>
        public static string? Show()
        {
            openFileDialog.FileName = ""; // 파일 이름 초기화
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
