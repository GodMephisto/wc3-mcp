---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/ModuloExpression.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class ModuloExpression extends Expression {

    Expression a;
    Expression b;

    public ModuloExpression(Expression a, Expression b) {
        this.a = a;
        this.b = b;

        this.a.setParent(this);
        this.b.setParent(this);
    }

    @Override
    public String translate() {
        return "ModuloReal(" + this.a.translate() + "," + this.b.translate() + ")";
    }
}
