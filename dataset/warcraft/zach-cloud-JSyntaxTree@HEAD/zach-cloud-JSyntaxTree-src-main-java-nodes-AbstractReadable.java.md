---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/AbstractReadable.java"
ref: HEAD
---

package nodes;

public abstract class AbstractReadable {

    protected final String rename(String original, String oldName, String newName) {
        if(original.equals(oldName)) {
            return newName;
        } else if(original.equals("function " + oldName)) {
            return "function " + newName;
        } else {
            return original;
        }
    }

}
