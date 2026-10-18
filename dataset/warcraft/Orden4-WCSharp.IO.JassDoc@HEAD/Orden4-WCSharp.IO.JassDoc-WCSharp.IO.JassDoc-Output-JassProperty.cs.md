---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Output/JassProperty.cs"
ref: HEAD
---

﻿namespace WCSharp.IO.JassDoc.Output
{
	/// <summary>
	/// Any sort of property, value, constant or array decleration within a JassDoc file.
	/// </summary>
	public class JassProperty : JassEntity
	{
		/// <summary>
		/// The type of the property.
		/// </summary>
		public string Type { get; set; } = null!;
		/// <summary>
		/// The value of the property, if present.
		/// </summary>
		public string? Value { get; set; }
		/// <summary>
		/// Whether the property is an array.
		/// </summary>
		public bool IsArray { get; set; }
		/// <summary>
		/// If the property is a constant.
		/// </summary>
		public bool IsConstant { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return Name;
		}
	}
}
