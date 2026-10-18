---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/StringReference.cs"
ref: HEAD
---

namespace BananaMpq.Layer.Chunks
{
    public class StringReference
    {
        public string String { get; set; }
        public int Offset { get; set; }

        public override string ToString()
        {
            return string.Format("Offset: {0:X}, {1}", Offset, String);
        }
    }
}