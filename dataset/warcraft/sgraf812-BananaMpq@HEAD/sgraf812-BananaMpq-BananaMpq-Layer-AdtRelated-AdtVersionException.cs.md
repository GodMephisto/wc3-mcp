---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/AdtRelated/AdtVersionException.cs"
ref: HEAD
---

using System;
using System.Runtime.Serialization;
using BananaMpq.Layer.Chunks;

namespace BananaMpq.Layer.AdtRelated
{
    [Serializable]
    public class AdtVersionException : ChunkValidationException<int>
    {
        public AdtVersionException(int expected, int actual)
            : base(expected, actual)
        {
        }

        public AdtVersionException(string message) : base(message)
        {
        }

        public AdtVersionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AdtVersionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}