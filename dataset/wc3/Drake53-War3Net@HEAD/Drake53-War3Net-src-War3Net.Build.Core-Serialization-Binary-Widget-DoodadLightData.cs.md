---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Widget/DoodadLightData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class DoodadLightData
    {
        internal DoodadLightData(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            LightIndex = reader.ReadUInt32();
            IsShadowCasting = reader.ReadUInt32();
            Color = reader.ReadColorBgra();
            Intensity = reader.ReadSingle();
            ShadowCastingStart = reader.ReadSingle();
            ShadowCastingEnd = reader.ReadSingle();
            QuadraticFalloff = reader.ReadSingle();
            LinearFalloff = reader.ReadSingle();
            Damping = reader.ReadSingle();
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(LightIndex);
            writer.Write(IsShadowCasting);
            writer.Write(Color.ToBgra());
            writer.Write(Intensity);
            writer.Write(ShadowCastingStart);
            writer.Write(ShadowCastingEnd);
            writer.Write(QuadraticFalloff);
            writer.Write(LinearFalloff);
            writer.Write(Damping);
        }
    }
}