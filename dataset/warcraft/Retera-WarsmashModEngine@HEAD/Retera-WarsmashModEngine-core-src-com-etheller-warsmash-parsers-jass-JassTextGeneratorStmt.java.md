---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/JassTextGeneratorStmt.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass;

public interface JassTextGeneratorStmt {
	default void generateJassEquivalent(final int indent, final JassTextGenerator jassTextGenerator) {
		throw new RuntimeException();
	}
}
