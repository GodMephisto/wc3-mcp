---
type: vendor-doc
source_repo: "https://github.com/erorus/casc"
source_path: "src/Erorus/CASC/BLTE/Chunk/Plain.php"
ref: HEAD
---

<?php

namespace Erorus\CASC\BLTE\Chunk;

use Erorus\CASC\BLTE\Chunk;

/**
 * A plaintext chunk, no encryption or compression.
 */
class Plain extends Chunk {
    /**
     * Receives encoded bytes, decodes them, and writes them to our $fileHandle.
     *
     * @param string $buffer Encoded bytes.
     * @return int How many encoded bytes were consumed.
     */
    public function Write(string $buffer): int {
        $written = fwrite($this->fileHandle, $buffer);
        $this->encodedBytesWritten += $written;

        return $written;
    }
}
