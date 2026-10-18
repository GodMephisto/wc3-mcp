---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/exception/MergeFailureException.java"
ref: HEAD
---

package exception;

/**
 * Thrown if a code merge crashes
 */
public final class MergeFailureException extends RuntimeException {

    public MergeFailureException(Exception e) {
        super(e);
    }

    public MergeFailureException(String message) {
        super(message);
    }

    public MergeFailureException() {
        super("");
    }
}
