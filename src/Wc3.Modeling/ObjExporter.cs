// src/Wc3.Modeling/ObjExporter.cs
using System.Globalization;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// Exports a parsed <see cref="Model3D"/> to Wavefront OBJ/MTL text.
/// Coordinates stay in WC3's Z-up convention — tell the importer "Z up"
/// (e.g. Blender's OBJ import axis setting) if the model lies on its side.
/// WC3 UVs have V running top-down, so V is flipped to OBJ's bottom-up.
/// </summary>
public static class ObjExporter
{
    public static string ToObj(Model3D model, string? mtlFileName)
    {
        ArgumentNullException.ThrowIfNull(model);
        var sb = new StringBuilder();
        sb.AppendLine("# exported by wc3ctl — WC3 coordinate system (Z-up, 1 unit ≈ 1/128 cell)");
        if (mtlFileName is not null)
            sb.Append("mtllib ").Append(mtlFileName).AppendLine();

        int vertexBase = 1; // OBJ indices are global across objects and 1-based
        for (int gi = 0; gi < model.Geosets.Count; gi++)
        {
            var g = model.Geosets[gi];
            int vertexCount = g.Vertices.Length / 3;
            sb.Append("o geoset").Append(gi).AppendLine();

            for (int v = 0; v < vertexCount; v++)
                sb.Append("v ").Append(F(g.Vertices[v * 3])).Append(' ')
                  .Append(F(g.Vertices[v * 3 + 1])).Append(' ')
                  .Append(F(g.Vertices[v * 3 + 2])).AppendLine();
            for (int v = 0; v < vertexCount; v++)
                sb.Append("vn ").Append(F(g.Normals[v * 3])).Append(' ')
                  .Append(F(g.Normals[v * 3 + 1])).Append(' ')
                  .Append(F(g.Normals[v * 3 + 2])).AppendLine();
            for (int v = 0; v < vertexCount; v++)
                sb.Append("vt ").Append(F(g.Uvs[v * 2])).Append(' ')
                  .Append(F(1f - g.Uvs[v * 2 + 1])).AppendLine(); // WC3 V is top-down

            if (mtlFileName is not null && g.TextureId >= 0)
                sb.Append("usemtl tex").Append(g.TextureId).AppendLine();

            for (int t = 0; t + 2 < g.Indices.Length; t += 3)
            {
                sb.Append("f ");
                for (int k = 0; k < 3; k++)
                {
                    int idx = vertexBase + g.Indices[t + k];
                    if (k > 0) sb.Append(' ');
                    sb.Append(idx).Append('/').Append(idx).Append('/').Append(idx);
                }
                sb.AppendLine();
            }

            vertexBase += vertexCount;
        }
        return sb.ToString();
    }

    /// <summary>
    /// One material per distinct geoset TextureId (unresolved -1 excluded), named
    /// tex&lt;id&gt; to match <see cref="ToObj"/>'s usemtl lines. When
    /// <paramref name="textureFileByTexId"/> maps an id to a file name, the
    /// material gets a map_Kd; otherwise it stays a plain grey diffuse.
    /// </summary>
    public static string ToMtl(Model3D model, IReadOnlyDictionary<int, string> textureFileByTexId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(textureFileByTexId);
        var sb = new StringBuilder();
        sb.AppendLine("# exported by wc3ctl");
        foreach (var id in model.Geosets.Select(g => g.TextureId)
                     .Where(id => id >= 0).Distinct().OrderBy(id => id))
        {
            sb.Append("newmtl tex").Append(id).AppendLine();
            sb.AppendLine("Kd 0.800 0.800 0.800");
            sb.AppendLine("d 1.000");
            sb.AppendLine("illum 1");
            if (textureFileByTexId.TryGetValue(id, out var file))
                sb.Append("map_Kd ").Append(file).AppendLine();
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string F(float value) =>
        value.ToString("0.######", CultureInfo.InvariantCulture);
}
