---
type: vendor-doc
source_repo: "https://github.com/Selzier/CascViewUnity"
source_path: "CascViewUnity/Assets/Scripts/Tables/DirectoryData.cs"
ref: HEAD
---

using System;
using UnityEditor.IMGUI.Controls;
using SimpleSQL;

[Serializable]
public class DirectoryData
{
	[PrimaryKey]
	public int Directory { get; set; }	
	public int Directory_Parent { get; set; }
	public string Value { get; set; }
}