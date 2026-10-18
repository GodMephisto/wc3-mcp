---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/tree/TreeContext.java"
ref: HEAD
---

package tree;

/**
 * Context class to track the last line read
 * to give the user context of where it failed if crashed
 */
public final class TreeContext {

    private String lastLine;

    public String getLastLine() {
        return lastLine;
    }

    public void setLastLine(String lastLine) {
        this.lastLine = lastLine;
    }
}
