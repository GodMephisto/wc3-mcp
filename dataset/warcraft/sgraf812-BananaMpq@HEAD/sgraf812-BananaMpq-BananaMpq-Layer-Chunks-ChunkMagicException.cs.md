---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/ChunkMagicException.cs"
ref: HEAD
---

using System;
using System.Runtime.Serialization;

namespace BananaMpq.Layer.Chunks
{
    [Serializable]
    public class ChunkMagicException : ChunkValidationException<string>
    {
        public ChunkMagicException(string expected, string actual)
            : base(expected, actual)
        {
        }

        public ChunkMagicException(string message) : base(message)
        {
        }

        public ChunkMagicException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChunkMagicException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}