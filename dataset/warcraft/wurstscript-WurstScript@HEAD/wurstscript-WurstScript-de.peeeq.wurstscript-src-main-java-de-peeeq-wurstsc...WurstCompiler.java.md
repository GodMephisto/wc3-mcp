---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/WurstCompiler.java"
ref: HEAD
---

package de.peeeq.wurstscript;

import org.wurstscript.projectconfig.WurstProjectConfigData;
import de.peeeq.wurstscript.ast.WurstModel;
import org.eclipse.jdt.annotation.Nullable;

import java.io.File;

public interface WurstCompiler {


    void loadFiles(String... filenames);

    @Nullable WurstModel parseFiles();

    void loadFiles(File... files);

    void runCompiletime(WurstProjectConfigData projectConfigData, boolean isProd, boolean cache);
}
