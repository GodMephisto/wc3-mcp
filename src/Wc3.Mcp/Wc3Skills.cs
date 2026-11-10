// src/Wc3.Mcp/Wc3Skills.cs
using System.Text;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// Three short skills served as resources, so a client reads the rule it needs in one read
/// instead of searching a surface of more than two hundred tools for it. Each one is a plain
/// markdown file copied beside the executable under <c>docs/skills</c>, with the same copy under
/// <c>context/skills</c> so the reference folder and the skills stay one corpus. The file is the
/// single source, and this class only names the URI and the sentence that helps a client pick.
///
/// A skill is guidance text. Serving one reads a file and changes nothing.
/// </summary>
public static class Wc3Skills
{
    /// <summary>Where the files are copied beside the executable, the primary location.</summary>
    internal const string PrimaryFolder = "docs/skills";

    /// <summary>The same three files inside the reference corpus, read when the primary is missing.</summary>
    internal const string FallbackFolder = "context/skills";

    /// <summary>One skill, its URI and the sentence a client reads before opening it.</summary>
    internal sealed record Skill(string Id, string Uri, string Name, string Description);

    /// <summary>Every skill this server serves, in the order a client should meet them.</summary>
    internal static readonly IReadOnlyList<Skill> All = new[]
    {
        new Skill(
            "map-safety",
            "wc3://skills/map-safety",
            "skills_map_safety",
            "The write rules for a map. Read the map first, never overwrite the input, and chain each "
                + "edit on the previous output. Read this before any tool that writes a map."),
        new Skill(
            "hero-package",
            "wc3://skills/hero-package",
            "skills_hero_package",
            "The files a hero package must carry, one per role, and the compiler gate every generated "
                + "model and script must pass before the package is installed."),
        new Skill(
            "publish",
            "wc3://skills/publish",
            "skills_publish",
            "Publishing is granted by the user in permissions.json and never by the agent. Read this "
                + "before any step that would publish, mirror or push."),
    };

    /// <summary>
    /// One read-only resource per skill, bound to the file behind it. Each carries the byte size and
    /// digest of the installed file, read once at start, so resources/list reports them and a
    /// client can see the skill came from disk rather than from text embedded in the assembly.
    /// </summary>
    public static IReadOnlyList<McpServerResource> CreateResources() =>
        All.Select(skill =>
        {
            byte[] bytes = File.ReadAllBytes(Locate(skill.Id));
            return (McpServerResource)new ServedTextResource(
                skill.Uri,
                skill.Name,
                null,
                skill.Description,
                "text/markdown",
                Encoding.UTF8.GetString(bytes),
                bytes);
        }).ToList();

    /// <summary>
    /// The installed path of a skill file, under <c>docs/skills</c> beside the executable or the
    /// <c>context/skills</c> copy when the first is not there. A missing file throws, so a client
    /// gets a refusal it can report rather than an empty skill that reads like a rule of nothing.
    /// </summary>
    internal static string Locate(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var looked = new List<string>();
        foreach (string folder in new[] { PrimaryFolder, FallbackFolder })
        {
            string path = Path.Combine(AppContext.BaseDirectory,
                folder.Replace('/', Path.DirectorySeparatorChar), id + ".md");
            looked.Add(path);
            if (File.Exists(path)) return path;
        }
        throw new FileNotFoundException(
            "the skill " + id + " is missing. Looked in " + string.Join(" and ", looked)
                + ". Reinstall the server so its skill files come with the executable.");
    }

    /// <summary>The skill's markdown, read whole from the installed folder.</summary>
    public static string Read(string id) => File.ReadAllText(Locate(id));
}
