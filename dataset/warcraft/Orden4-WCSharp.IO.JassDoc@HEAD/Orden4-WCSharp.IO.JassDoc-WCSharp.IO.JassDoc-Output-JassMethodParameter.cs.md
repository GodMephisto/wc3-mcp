---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Output/JassMethodParameter.cs"
ref: HEAD
---

﻿namespace WCSharp.IO.JassDoc.Output
{
	/// <summary>
	/// Any parameter used by methods or functions (e.g. <c>takes integer whichInt</c>).
	/// </summary>
	public class JassMethodParameter
	{
		/// <summary>
		/// The 0-based index of the parameter.
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string Name { get; set; } = null!;
		/// <summary>
		/// The type of the parameter.
		/// </summary>
		public string Type { get; set; } = null!;
		/// <summary>
		/// The description as provided by the <c>@param</c> annotation, if present.
		/// </summary>
		public string? Description { get; set; }
	}
}
