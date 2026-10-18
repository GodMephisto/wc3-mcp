---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/fdfparser/TestFDFParserBuilder.java"
ref: HEAD
---

package com.etheller.warsmash.fdfparser;

import java.io.IOException;

import org.antlr.v4.runtime.BaseErrorListener;
import org.antlr.v4.runtime.CharStreams;
import org.antlr.v4.runtime.CommonTokenStream;

public class TestFDFParserBuilder implements FDFParserBuilder {
	private final BaseErrorListener errorListener;

	public TestFDFParserBuilder(final BaseErrorListener errorListener) {
		this.errorListener = errorListener;
	}

	@Override
	public FDFParser build(final String path) {
		FDFLexer lexer;
		try {
			lexer = new FDFLexer(CharStreams.fromFileName(path));
		}
		catch (final IOException e) {
			throw new RuntimeException(e);
		}
		return new FDFParser(new CommonTokenStream(lexer));
	}
}
