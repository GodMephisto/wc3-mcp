---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Parsing/JassDocTarget.cs"
ref: HEAD
---

﻿namespace WCSharp.IO.JassDoc.Parsing
{
	/// <summary>
	/// The target JassDoc to parse.
	/// </summary>
	/// <param name="Uri">The path or URL towards the JassDoc file.</param>
	/// <param name="FileName">The filename (e.g. common.j). Will use the last item of <see cref="Uri.Segments"/> if null.</param>
	public record JassDocTarget(Uri Uri, string? FileName = null);
}
