---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/Overrideable.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import java.util.Collection;

public interface Overrideable {
	
	/**
	 * 
	 * @return
	 */
	public Collection<Symbol> getImplementations();

}
