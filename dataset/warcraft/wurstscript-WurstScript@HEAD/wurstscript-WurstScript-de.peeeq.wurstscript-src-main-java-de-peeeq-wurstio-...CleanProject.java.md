---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/languageserver/requests/CleanProject.java"
ref: HEAD
---

package de.peeeq.wurstio.languageserver.requests;

import de.peeeq.wurstio.languageserver.ModelManager;

/**
 * Created by peter on 05.05.16.
 */
public class CleanProject extends UserRequest<String> {

	@Override
	public String execute(ModelManager modelManager) {
		modelManager.clean();
		modelManager.buildProject();
		return "done";
	}
}
