// src/Wc3.Commands/Rag/Neo4jStore.cs
using Neo4j.Driver;

namespace Wc3.Commands.Rag;

/// <summary>
/// The Neo4j store for the shared corpus graph, behind `rag install --neo4j`. It is fed the same
/// nodes and edges the in-process graph already built, so choosing this store does not re-extract
/// anything twice. The load merges by id and reads its own counts back, so the install can fail
/// when the store and the graph disagree rather than claiming success.
/// </summary>
public sealed class Neo4jStore : IDisposable, IAsyncDisposable
{
    public const string DefaultUri = "bolt://localhost:7687";
    public const string DefaultUser = "neo4j";
    public const string DefaultDatabase = "neo4j";

    private readonly IDriver _driver;
    private readonly string _database;

    public Neo4jStore(string uri, string user, string password, string database = DefaultDatabase)
    {
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        _database = database;
    }

    /// <summary>From the environment, NEO4J_URI, NEO4J_USER, NEO4J_PASSWORD, NEO4J_DATABASE.</summary>
    public static Neo4jStore FromEnvironment()
    {
        string uri = Environment.GetEnvironmentVariable("NEO4J_URI") is { Length: > 0 } u ? u : DefaultUri;
        string user = Environment.GetEnvironmentVariable("NEO4J_USER") is { Length: > 0 } n ? n : DefaultUser;
        string pass = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "";
        string db = Environment.GetEnvironmentVariable("NEO4J_DATABASE") is { Length: > 0 } d ? d : DefaultDatabase;
        return new Neo4jStore(uri, user, pass, db);
    }

    public async Task VerifyConnectivityAsync(CancellationToken ct = default) =>
        await _driver.VerifyConnectivityAsync().ConfigureAwait(false);

    public async Task EnsureSchemaAsync(CancellationToken ct = default)
    {
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
        await session.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync("CREATE CONSTRAINT doc_id IF NOT EXISTS FOR (d:Doc) REQUIRE d.id IS UNIQUE");
            await tx.RunAsync("CREATE CONSTRAINT sec_id IF NOT EXISTS FOR (s:Section) REQUIRE s.id IS UNIQUE");
            return 0;
        });
    }

    /// <summary>Merge the graph into the store. Node labels from the node type, edges by type.</summary>
    public async Task LoadAsync(RagGraph graph, Action<int, int>? progress = null, CancellationToken ct = default)
    {
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
        foreach (var group in graph.Nodes.GroupBy(n => n.Type))
        {
            string label = group.Key;
            var rows = group.Select(n => new Dictionary<string, object> { ["id"] = n.Id, ["name"] = n.Name }).ToArray();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(
                    $"UNWIND $rows AS r MERGE (n:{label} {{id: r.id}}) SET n.name = r.name",
                    new { rows });
                return 0;
            });
            progress?.Invoke(rows.Length, 0);
        }
        foreach (var group in graph.Edges.GroupBy(e => e.Type))
        {
            string type = group.Key;
            var rows = group.Select(e => new Dictionary<string, object> { ["from"] = e.From, ["to"] = e.To, ["w"] = e.Weight }).ToArray();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(
                    $"UNWIND $rows AS r MATCH (a {{id: r.from}}) MATCH (b {{id: r.to}}) MERGE (a)-[e:{type}]->(b) SET e.w = r.w",
                    new { rows });
                return 0;
            });
            progress?.Invoke(0, rows.Length);
        }
    }

    /// <summary>Node and edge counts back from the store, so a load can be compared to the graph.</summary>
    public async Task<(long Nodes, long Edges)> CountsAsync(CancellationToken ct = default)
    {
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
        return await session.ExecuteReadAsync(async tx =>
        {
            var nr = await (await tx.RunAsync("MATCH (n) RETURN count(n) AS c")).SingleAsync();
            var er = await (await tx.RunAsync("MATCH ()-[e]->() RETURN count(e) AS c")).SingleAsync();
            return (nr["c"].As<long>(), er["c"].As<long>());
        });
    }

    public async ValueTask DisposeAsync() => await _driver.DisposeAsync().ConfigureAwait(false);

    public void Dispose() => _driver.Dispose();
}
