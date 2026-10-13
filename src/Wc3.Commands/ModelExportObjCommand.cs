// src/Wc3.Commands/ModelExportObjCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Exports a map-imported model to Wavefront OBJ. A thin named command over
/// <see cref="ConvertCommand.ExportModelToObj(MapDocument, string)"/>, which does the work of
/// parsing the model through the shared parser and converting every map-resolvable texture to
/// PNG. It exists as its own command so the CLI and MCP surfaces have a stable verb for the
/// operation rather than reaching into the general convert command.
/// </summary>
public static class ModelExportObjCommand
{
    /// <summary>Parses the model at an internal map path and returns its OBJ text, its MTL text
    /// and the PNG bytes of every texture the map can supply, keyed by the file name the MTL
    /// references. The .mdx/.mdl path swap is handled by the shared model finder.</summary>
    public static ModelExport Execute(MapDocument doc, string modelInternalPath, string? mtlName = null) =>
        ConvertCommand.ExportModelToObj(doc, modelInternalPath, mtlName);
}
