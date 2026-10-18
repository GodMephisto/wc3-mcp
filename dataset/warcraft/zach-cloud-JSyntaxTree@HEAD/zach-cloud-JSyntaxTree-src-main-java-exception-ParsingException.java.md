---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/exception/ParsingException.java"
ref: HEAD
---

package exception;

/**
 * Thrown if the AST Reading code fails to deserialize the JASS code
 */
public final class ParsingException extends RuntimeException {

    public ParsingException(Exception e) {
        super(e);
    }

    public ParsingException(String message) {
        super(message);
    }

    public ParsingException() {
        super("");
    }
}
