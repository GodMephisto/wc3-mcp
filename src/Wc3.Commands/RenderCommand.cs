// src/Wc3.Commands/RenderCommand.cs
namespace Wc3.Commands;

public static class RenderCommand
{
    /// <summary>Returns PNG bytes of a top-down terrain render; the CLI writes the file.</summary>
    public static byte[] Execute(Wc3.Model.MapDocument doc) =>
        Wc3.Render.TerrainRenderer.RenderTerrainPng(doc);
}
