---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/JassTextGeneratorExpr.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass;

public interface JassTextGeneratorExpr {
	default String generateJassEquivalent(final JassTextGenerator jassTextGenerator) {
		throw new RuntimeException();
	}
}
