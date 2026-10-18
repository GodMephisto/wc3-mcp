---
type: vendor-doc
source_repo: "https://github.com/inwc3/JMPQ3"
source_path: "src/main/java/systems/crigges/jmpq3/compression/RecompressOptions.java"
ref: HEAD
---

package systems.crigges.jmpq3.compression;

public class RecompressOptions {
    public boolean recompress;
    public boolean useZopfli = false;
    /** Compress large deflate sector batches on the common fork/join pool; Zopfli stays serial. */
    public boolean parallelCompression = true;
    public int iterations = 16;
    public int newSectorSizeShift = 3;

    public RecompressOptions(boolean recompress) {
        this.recompress = recompress;
    }
}
