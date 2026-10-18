---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/Type.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

public interface Type {
	
	String getName();

	boolean isTypeCompatible(Symbol symbol);

	boolean isUserType();
	
}
