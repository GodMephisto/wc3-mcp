---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/core/service/MdxAnimationScanner.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter.core.service;

import java.io.File;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;
import java.util.logging.Logger;

/**
 * Lightweight MDX binary scanner that detects alternate/upgrade animation sequences.
 *
 * <p>MDX files store animation sequences in a {@code SEQS} chunk. Each sequence record is
 * 132 bytes: an 80-byte null-terminated name followed by numeric fields. This scanner
 * finds the {@code SEQS} chunk and checks every sequence name for the keywords that
 * indicate extra unit definitions are needed:
 *
 * <ul>
 *   <li>{@code "Alternate"}      → {@code uani = "alternate"}</li>
 *   <li>{@code "Upgrade First"}  → {@code uani = "upgrade,first"}</li>
 *   <li>{@code "Upgrade Second"} → {@code uani = "upgrade,second"}</li>
 *   <li>{@code "Upgrade Third"}  → {@code uani = "upgrade,third"}</li>
 * </ul>
 *
 * <p>The scan is intentionally simple and fast: no full MDX parsing, just a chunk-header
 * search followed by fixed-stride name reads.
 */
public class MdxAnimationScanner {

    private static final Logger LOG = Logger.getLogger(MdxAnimationScanner.class.getName());

    /** Ordered list of keywords to detect. */
    public static final String[] ALTERNATE_KEYWORDS = {
            "Alternate", "Upgrade First", "Upgrade Second", "Upgrade Third"
    };

    /** Byte size of one sequence record inside the SEQS chunk. */
    private static final int SEQUENCE_RECORD_SIZE = 132;

    /** Byte size of the sequence name field (null-terminated ASCII). */
    private static final int SEQUENCE_NAME_SIZE = 80;

    private MdxAnimationScanner() {}

    /**
     * Reads {@code mdxFile} and returns the distinct alternate-animation keywords found
     * in its sequence names, in declaration order. Returns an empty list when the file
     * cannot be read, has no SEQS chunk, or has no matching sequences.
     */
    public static List<String> scan(File mdxFile) {
        try {
            byte[] bytes = java.nio.file.Files.readAllBytes(mdxFile.toPath());
            return scan(bytes);
        } catch (Exception e) {
            LOG.fine("Could not scan MDX for alternate animations: "
                    + mdxFile.getName() + " — " + e.getMessage());
            return Collections.emptyList();
        }
    }

    /**
     * Package-private overload used by unit tests (avoids disk I/O).
     */
    static List<String> scan(byte[] bytes) {
        if (bytes.length < 8) return Collections.emptyList();

        int seqsOffset = findChunk(bytes, "SEQS");
        if (seqsOffset < 0) return Collections.emptyList();

        // 4-byte tag + 4-byte little-endian size
        if (seqsOffset + 8 > bytes.length) return Collections.emptyList();
        int chunkSize = ByteBuffer.wrap(bytes, seqsOffset + 4, 4)
                .order(ByteOrder.LITTLE_ENDIAN).getInt();

        int dataStart = seqsOffset + 8;
        int dataEnd   = Math.min(dataStart + chunkSize, bytes.length);

        Set<String> found = new LinkedHashSet<>();
        for (int off = dataStart; off + SEQUENCE_RECORD_SIZE <= dataEnd; off += SEQUENCE_RECORD_SIZE) {
            String name = readCString(bytes, off, SEQUENCE_NAME_SIZE);
            for (String keyword : ALTERNATE_KEYWORDS) {
                if (name.contains(keyword)) {
                    found.add(keyword);
                }
            }
        }

        return new ArrayList<>(found);
    }

    /**
     * Searches {@code bytes} (starting after the 4-byte MDX magic) for the first
     * occurrence of the 4-character ASCII chunk tag.
     *
     * @return byte offset of the tag, or -1 when not found
     */
    private static int findChunk(byte[] bytes, String tag) {
        byte[] tagBytes = tag.getBytes(StandardCharsets.US_ASCII);
        // Skip the 4-byte "MDX\0" file magic
        for (int i = 4; i <= bytes.length - tagBytes.length - 4; i++) {
            if (bytes[i]     == tagBytes[0]
             && bytes[i + 1] == tagBytes[1]
             && bytes[i + 2] == tagBytes[2]
             && bytes[i + 3] == tagBytes[3]) {
                return i;
            }
        }
        return -1;
    }

    /**
     * Reads a null-terminated ASCII/UTF-8 string from {@code bytes} starting at
     * {@code offset}, reading at most {@code maxLen} bytes.
     */
    private static String readCString(byte[] bytes, int offset, int maxLen) {
        int len = 0;
        while (len < maxLen && offset + len < bytes.length && bytes[offset + len] != 0) {
            len++;
        }
        return new String(bytes, offset, len, StandardCharsets.UTF_8);
    }
}
