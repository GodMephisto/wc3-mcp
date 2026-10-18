---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Output/JassApi.cs"
ref: HEAD
---

﻿using System.Text.Json.Serialization;

namespace WCSharp.IO.JassDoc.Output
{
	/// <summary>
	/// The complete API as specified within one or more JassDoc files.
	/// </summary>
	public class JassApi
	{
		/// <summary>
		/// All entities defined within this Jass API.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<JassEntity> Entities => Methods.AsEnumerable<JassEntity>().Concat(Properties).Concat(Types);

		/// <summary>
		/// All type specifications.
		/// </summary>
		public List<JassType> Types { get; set; } = [];
		/// <summary>
		/// <para>All function specifications.</para>
		/// <para>Includes both native declerations and function declerations:</para>
		/// <b>Examples:</b>
		/// <code>
		/// native GetUnitX takes unit whichUnit returns real
		/// function IsPointBlightedBJ takes location where returns boolean (...) endfunction
		/// </code>
		/// </summary>
		public List<JassMethod> Methods { get; set; } = [];
		/// <summary>
		/// <para>All property/field/value/array specifications.</para>
		/// <para>Does not include locals created within function blocks.</para>
		/// <b>Examples:</b>
		/// <code>
		/// constant subanimtype SUBANIM_TYPE_READY = ConvertSubAnimType(19)
		/// constant integer bj_ITEM_STATUS_HIDDEN = 0
		/// player ai_player
		/// force array bj_FORCE_PLAYER
		/// </code>
		/// </summary>
		public List<JassProperty> Properties { get; set; } = [];

		/// <summary>
		/// Adds all entities within <paramref name="other"/> to this instance.
		/// </summary>
		public void Merge(JassApi other)
		{
			Types.AddRange(other.Types);
			Methods.AddRange(other.Methods);
			Properties.AddRange(other.Properties);
		}
	}
}
