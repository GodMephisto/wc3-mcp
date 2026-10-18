---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/ILconstNum.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;


public interface ILconstNum extends ILconstAddable {

    ILconstNum negate();

    ILconstNum sub(ILconstNum other);

    ILconstNum mul(ILconstNum other);

    ILconstNum div(ILconstNum other);

    ILconstBool less(ILconstNum other);

    ILconstBool lessEq(ILconstNum other);

    ILconstBool greater(ILconstNum other);

    ILconstBool greaterEq(ILconstNum other);
}