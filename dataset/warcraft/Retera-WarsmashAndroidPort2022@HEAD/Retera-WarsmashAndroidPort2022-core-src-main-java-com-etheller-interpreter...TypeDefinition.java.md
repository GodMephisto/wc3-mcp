---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/scope/TypeDefinition.java"
ref: HEAD
---

package com.etheller.interpreter.ast.scope;

public class TypeDefinition {
	private final String name;
	private final String supertype;

	public TypeDefinition(final String name, final String supertype) {
		this.name = name;
		this.supertype = supertype;
	}
}
