---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Environment/PreviewIcon.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class PreviewIcon
    {
        internal PreviewIcon(BinaryReader reader, MapPreviewIconsFormatVersion formatVersion)
        {
            ReadFrom(reader, formatVersion);
        }

        internal void ReadFrom(BinaryReader reader, MapPreviewIconsFormatVersion formatVersion)
        {
            IconType = reader.ReadInt32<PreviewIconType>();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Color = reader.ReadColorBgra();
        }

        internal void WriteTo(BinaryWriter writer, MapPreviewIconsFormatVersion formatVersion)
        {
            writer.Write((int)IconType);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Color.ToBgra());
        }
    }
}