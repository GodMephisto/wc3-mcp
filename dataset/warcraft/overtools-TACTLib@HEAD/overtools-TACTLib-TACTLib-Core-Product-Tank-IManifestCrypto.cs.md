---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/Tank/IManifestCrypto.cs"
ref: HEAD
---

namespace TACTLib.Core.Product.Tank {
    public interface IManifestCrypto<in T> {
        byte[] Key(T header, int length);
        byte[] IV(T header, byte[] digest, int length);
    }

    public interface ICMFEncryptionProc : IManifestCrypto<ContentManifestFile.CMFHeader> { }

    public interface ITRGEncryptionProc : IManifestCrypto<ResourceGraph.TRGHeader> { }
}