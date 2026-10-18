---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Widget/MapUnits.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class MapUnits
    {
        internal MapUnits(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            if (reader.ReadInt32() != FileFormatSignature)
            {
                throw new InvalidDataException($"Expected file header signature at the start of a .doo file.");
            }

            FormatVersion = reader.ReadInt32<MapWidgetsFormatVersion>();
            SubVersion = reader.ReadInt32<MapWidgetsSubVersion>();

            nint unitCount = reader.ReadInt32();
            if (unitCount > 0)
            {
                Units.Add(reader.ReadMapUnitData(FormatVersion, SubVersion, out var useNewFormat));
                UseNewFormat = useNewFormat;

                for (nint i = 1; i < unitCount; i++)
                {
                    Units.Add(reader.ReadMapUnitData(FormatVersion, SubVersion, out useNewFormat));
                    if (useNewFormat != UseNewFormat)
                    {
                        throw new InvalidDataException();
                    }
                }
            }
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(FileFormatSignature);
            writer.Write((int)FormatVersion);
            writer.Write((int)SubVersion);

            writer.Write(Units.Count);
            foreach (var unit in Units)
            {
                writer.Write(unit, FormatVersion, SubVersion, UseNewFormat);
            }
        }
    }
}