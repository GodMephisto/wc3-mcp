---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/util/TestHelper.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.util;

import com.ruke.vrjassc.vrjassc.exception.CompileException;
import org.junit.Rule;
import org.junit.rules.ExpectedException;

import java.io.IOException;

public abstract class TestHelper {

	@Rule
	public ExpectedException expectedEx = ExpectedException.none();

	protected Compile compile = new Compile();
	
	protected String runFromFile(String path) throws IOException {
		return this.compile.runFromFile(path);
	}
	
	protected String run(String code) throws CompileException {
		try {
			return this.compile.run(code);
		} catch (IOException e) {

		}
		return code;
	}
	
}
