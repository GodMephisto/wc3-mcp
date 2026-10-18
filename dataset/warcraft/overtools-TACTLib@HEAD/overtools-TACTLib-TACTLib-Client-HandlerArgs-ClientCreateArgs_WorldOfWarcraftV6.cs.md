---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Client/HandlerArgs/ClientCreateArgs_WorldOfWarcraftV6.cs"
ref: HEAD
---

namespace TACTLib.Client.HandlerArgs {
    /// <inheritdoc />
    /// <summary>
    /// Args specific to WorldOfWarcraftV6
    /// </summary>
    public class ClientCreateArgs_WorldOfWarcraftV6 : IHandlerArgs {
        /// <summary>
        /// Lookup list file to load
        /// </summary>
        public string? ListFile { get; set; } = null;
    }
}
