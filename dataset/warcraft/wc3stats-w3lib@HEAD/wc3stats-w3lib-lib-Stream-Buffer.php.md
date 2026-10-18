---
type: vendor-doc
source_repo: "https://github.com/wc3stats/w3lib"
source_path: "lib/Stream/Buffer.php"
ref: HEAD
---

<?php

namespace w3lib\Library\Stream;

use w3lib\Library\Stream;

class Buffer extends Stream
{
    public function __construct (string $s = '')
    {
        $stream = fopen ('php://memory', 'r+');
    
        fwrite ($stream, $s);
        rewind ($stream);

        parent::__construct ($stream);
    }
}

?>