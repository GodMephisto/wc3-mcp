---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Output/JassEntity.cs"
ref: HEAD
---

﻿namespace WCSharp.IO.JassDoc.Output
{
	/// <summary>
	/// Any sort of entity within a JassDoc specification, be it method, type or property.
	/// </summary>
	public class JassEntity
	{
		/// <summary>
		/// The file this entity was retrieved from (e.g. <c>common.j</c>, <c>Blizzard.j</c>, etc.)
		/// </summary>
		public string SourceFile { get; set; } = null!;
		/// <summary>
		/// The source code of the entity.
		/// </summary>
		public string SourceCode { get; set; } = null!;
		/// <summary>
		/// The name of this entity.
		/// </summary>
		public string Name { get; set; } = null!;
		/// <summary>
		/// The description as provided via JassDoc, i.e. any text prior to an annotation.
		/// <para>If the only text was whitespace, this will be null.</para>
		/// </summary>
		public string? Description { get; set; }
		/// <summary>
		/// The <c>@patch</c> annotation.
		/// <para>Assumed to always be present.</para>
		/// </summary>
		public string Patch { get; set; } = null!;
		/// <summary>
		/// The content of all <c>@bug</c> annotations.
		/// </summary>
		public List<string> Bugs { get; set; } = [];
		/// <summary>
		/// The content of all <c>@note</c> annotations.
		/// </summary>
		public List<string> Notes { get; set; } = [];
	}
}
