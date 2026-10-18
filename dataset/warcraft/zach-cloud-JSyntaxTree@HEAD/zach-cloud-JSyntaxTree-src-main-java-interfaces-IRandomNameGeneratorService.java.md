---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/IRandomNameGeneratorService.java"
ref: HEAD
---

package interfaces;

/**
 * Used to generate random and unique variable/function names
 */
public interface IRandomNameGeneratorService {

    /**
     * Produces the next unique random name
     *
     * @return  Unique random name
     */
    String next();
}
