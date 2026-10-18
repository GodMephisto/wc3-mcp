---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/JassTextGeneratorCallStmt.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass;

public interface JassTextGeneratorCallStmt extends JassTextGeneratorStmt {
	String generateJassEquivalent(JassTextGenerator jassTextGenerator);

	@Override
	default void generateJassEquivalent(final int indent, final JassTextGenerator jassTextGenerator) {
		final StringBuilder sb = new StringBuilder();
		JassTextGenerator.Util.indent(indent, sb);
		sb.append("call ");
		sb.append(generateJassEquivalent(jassTextGenerator));
		jassTextGenerator.println(sb.toString());
	}
}
