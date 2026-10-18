---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/ColorHelper.cs"
ref: HEAD
---

﻿using System.Globalization;
using System.Windows.Media;

namespace vJassMainJBlueprint.Utils
{
    public static class ColorHelper
    {
        // 1. 문자열을 Color로 변환 (string => Color)
        public static Color Parse(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex) || hex[0] != '#' || hex.Length != 9)
            {
                throw new ArgumentException("올바른 #AARRGGBB 형식의 색상 문자열을 제공해야 합니다.");
            }

            // 16진수로 변환
            byte a = byte.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(7, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        // 문자열을 Color로 변환 시도 (string => Color)
        public static bool TryParse(string value, out Color color)
        {
            color = default; // 변환 실패 시 기본값으로 초기화

            // #AARRGGBB 형식 확인
            if (string.IsNullOrWhiteSpace(value) || value[0] != '#' || value.Length != 9)
            {
                return false;
            }

            try
            {
                // 16진수로 변환
                byte a = byte.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
                byte r = byte.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(value.Substring(7, 2), NumberStyles.HexNumber);

                color = Color.FromArgb(a, r, g, b);
                return true;
            }
            catch
            {
                return false; // 변환 중 오류 발생 시 false 반환
            }
        }

        // 2. Color를 문자열로 변환 (Color => string)
        public static string ToHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
