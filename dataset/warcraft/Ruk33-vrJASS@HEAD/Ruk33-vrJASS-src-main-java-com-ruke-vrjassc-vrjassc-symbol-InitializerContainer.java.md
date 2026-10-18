---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/InitializerContainer.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import java.util.Collection;

public interface InitializerContainer {

	public void setInitializer(Symbol initializer);
	
	public Symbol getInitializer();
	
	public Collection<InitializerContainer> getInitializersToLoadFirst();
	
}
