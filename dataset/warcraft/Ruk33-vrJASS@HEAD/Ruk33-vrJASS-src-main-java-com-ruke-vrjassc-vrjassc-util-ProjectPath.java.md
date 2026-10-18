---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/util/ProjectPath.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.util;

public class ProjectPath {

	public static String getRoot() {
		/*return ProjectPath.class.getProtectionDomain().getCodeSource()
				.getLocation().getPath()
				+ "..";*/
		return ".";
	}

	public static String getSrc() {
		return ProjectPath.getRoot() + "/src";
	}

	public static String getTest() {
		return ProjectPath.getSrc() + "/test/java/com/ruke/vrjassc/vrjassc";
	}

}
