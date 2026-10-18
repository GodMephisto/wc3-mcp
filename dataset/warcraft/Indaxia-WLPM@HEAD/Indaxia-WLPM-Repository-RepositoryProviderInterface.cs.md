---
type: vendor-doc
source_repo: "https://github.com/Indaxia/WLPM"
source_path: "Repository/RepositoryProviderInterface.cs"
ref: HEAD
---

using System.Threading.Tasks;

namespace wlpm.Repository
{
    public interface RepositoryProviderInterface
    {
        bool IdentifyURL(string repositoryUrl);
        string GetZipFileUrl(string repositoryUrl, string version);
    }
}