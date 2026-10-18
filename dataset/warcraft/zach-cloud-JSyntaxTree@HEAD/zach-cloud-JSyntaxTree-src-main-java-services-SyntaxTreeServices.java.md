---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/services/SyntaxTreeServices.java"
ref: HEAD
---

package services;

import interfaces.IPreprocessFileService;
import interfaces.IRandomNameGeneratorService;

public final class SyntaxTreeServices {

    public static IPreprocessFileService defaultPreprocessor() {
        return new PreprocessFileService();
    }

    public static IRandomNameGeneratorService defaultRandomNameGenerator() {
        return new RandomNameGeneratorService();
    }

    private SyntaxTreeServices() {}
}
