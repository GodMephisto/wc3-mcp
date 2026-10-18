---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/Base64Helper.cs"
ref: HEAD
---

﻿using System.Windows.Media.Imaging;

namespace vJassMainJBlueprint.Utils
{
    internal class Base64Helper
    {
        public static BitmapImage? Convert(string? base64String)
        {
            if (base64String == null)
            {
                return null;
            }
            byte[] bytes = System.Convert.FromBase64String(base64String);
            BitmapImage bitmapImage = new();
            using (var stream = new System.IO.MemoryStream(bytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static string? Convert(BitmapSource? bitmapSource)
        {
            if (bitmapSource == null)
            {
                return null;
            }
            string base64String;
            using (var stream = new System.IO.MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                base64String = System.Convert.ToBase64String(stream.ToArray());
            }
            return base64String;
        }
    }
}
