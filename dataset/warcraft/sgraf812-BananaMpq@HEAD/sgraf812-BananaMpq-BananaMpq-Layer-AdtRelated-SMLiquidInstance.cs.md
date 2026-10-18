---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/AdtRelated/SMLiquidInstance.cs"
ref: HEAD
---

namespace BananaMpq.Layer.AdtRelated
{
#pragma warning disable 169, 649
    struct SMLiquidInstance
    {
        public short liquidType;
        public short liquidObjectId;
        public float minWaterHeight;
        public float maxWaterHeight;
        public byte xOffset;
        public byte yOffset;
        public byte width;
        public byte height;
        public int existsTable;
        public int data;
    }
#pragma warning restore 169, 649
}