---
type: vendor-doc
source_repo: "https://github.com/tdauth/wc3trans"
source_path: "src/main/java/org/tdauth/wc3trans/StringEntry.java"
ref: HEAD
---

package org.tdauth.wc3trans;

public class StringEntry {
    public final long id;
    public String comment = "";
    public String text = "";

    public StringEntry(long id) {
        this.id = id;
    }
}
