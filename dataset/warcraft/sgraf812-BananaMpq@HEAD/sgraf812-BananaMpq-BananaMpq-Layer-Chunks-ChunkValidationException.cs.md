---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/ChunkValidationException.cs"
ref: HEAD
---

using System;
using System.Runtime.Serialization;

namespace BananaMpq.Layer.Chunks
{
    [Serializable]
    public abstract class ChunkValidationException<T> : Exception
    {
        public ChunkValidationException(T expected, T actual)
            : this(string.Format("Expected: {0}, actual: {1}", expected, actual))
        {
        }

        public ChunkValidationException(string message) : base(message)
        {
        }

        public ChunkValidationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChunkValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}