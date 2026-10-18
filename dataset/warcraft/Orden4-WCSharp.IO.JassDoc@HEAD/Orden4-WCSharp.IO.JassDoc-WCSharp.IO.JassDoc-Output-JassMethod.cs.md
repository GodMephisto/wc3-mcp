---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/Output/JassMethod.cs"
ref: HEAD
---

﻿namespace WCSharp.IO.JassDoc.Output
{
	/// <summary>
	/// Describes a method or function definition within Jass.
	/// <para>Essentially, anything that is called with 0 or more parameters.</para>
	/// </summary>
	public class JassMethod : JassEntity
	{
		/// <summary>
		/// The return type.
		/// <para>Will be <c>nothing</c> if it doesn't return anything.</para>
		/// </summary>
		public string ReturnType { get; set; } = null!;
		/// <summary>
		/// All <c>@event</c> annotations.
		/// </summary>
		public List<string> Events { get; set; } = [];
		/// <summary>
		/// The parameters of the function (<c>takes integer whichInt</c>, etc.)
		/// </summary>
		public List<JassMethodParameter> Parameters { get; set; } = [];
		/// <summary>
		/// If the function is annotated with <c>@pure</c>.
		/// </summary>
		public bool IsPure { get; set; }
		/// <summary>
		/// If the function is annotated with <c>@async</c>.
		/// </summary>
		public bool IsAsync { get; set; }
		/// <summary>
		/// If the function is prefixed with <c>constant</c>.
		/// </summary>
		public bool IsConstant { get; set; }
		/// <summary>
		/// Whether the function has a body, i.e. is defined in code rather than a native call.
		/// </summary>
		public bool HasBody => SourceCode.EndsWith("endfunction");

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{Name}({string.Join(", ", Parameters.Select(x => x.Name))})";
		}
	}
}
