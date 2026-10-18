---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/NumberConvertHelper.cs"
ref: HEAD
---

using System;
using System.Linq;
using System.Text;

namespace JassToCsMain
{
    public static class NumberConvertHelper
    {
        public static int HexToDecimal(string jassHex)
        {
            string hex = jassHex;
            if (jassHex[0] == '$')
            {
                hex = jassHex.Replace("$", "0x");
            }
            ;
            return Convert.ToInt32(hex, 16);
        }

        public static string GetDecimalOrFourcc(string number)
        {
            return GetDecimalOrFourcc(long.Parse(number));
        }

        public static string GetDecimalOrFourcc(long number)
        {
            if (number >= 256 * 256 * 256)
            {
                string fourcc = NumToFourcc(number);
                if (fourcc.All(b => char.IsLetterOrDigit(b)))
                {
                    return $"\"{fourcc}\"";
                }
            }

            return number.ToString();
        }

        public static int FourccToNum(string fourcc)
        {
            int num = (sbyte)fourcc[0] * 256 * 256 * 256 + (sbyte)fourcc[1] * 256 * 256 + (sbyte)fourcc[2] * 256 + (sbyte)fourcc[3];

            return num;
        }

        public static string NumToFourcc(long num)
        {
            // 945821957-256^3*56-256^2*96-256*25-5
            long difference = num;
            byte x = (byte)(difference / (256 * 256 * 256));
            difference -= (long)256 * 256 * 256 * x;
            byte y = (byte)(difference / (256 * 256));
            difference -= 256 * 256 * y;
            byte z = (byte)(difference / 256);
            difference -= 256 * z;
            byte d = (byte)(difference / 1);
            difference -= d;

            if (difference != 0)
            {
                throw new Exception();
            }

            byte[] bytes = new byte[4] { x, y, z, d };
            string fourcc = Encoding.Default.GetString(bytes);

            return fourcc;
        }
    }
}
