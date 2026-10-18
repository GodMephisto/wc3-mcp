---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Client/HandlerArgs/ClientCreateArgs_Fenris.cs"
ref: HEAD
---

namespace TACTLib.Client.HandlerArgs;

public class ClientCreateArgs_Fenris : IHandlerArgs {
    /// <summary>
    /// Language to use for base language overrides.
    /// </summary>
    public string? BaseLanguage { get; set; }
}
