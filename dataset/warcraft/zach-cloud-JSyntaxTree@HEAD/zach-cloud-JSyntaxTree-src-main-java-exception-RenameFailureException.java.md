---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/exception/RenameFailureException.java"
ref: HEAD
---

package exception;

/**
 * Thrown is a variable/function rename crashes
 */
public final class RenameFailureException extends RuntimeException {

    public RenameFailureException(Exception e) {
        super(e);
    }

    public RenameFailureException(String message) {
        super(message);
    }

    public RenameFailureException() {
        super("");
    }
}
