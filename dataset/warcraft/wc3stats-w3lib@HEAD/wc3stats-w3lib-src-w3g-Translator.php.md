---
type: vendor-doc
source_repo: "https://github.com/wc3stats/w3lib"
source_path: "src/w3g/Translator.php"
ref: HEAD
---

<?php

namespace w3lib\w3g;

use w3lib\Library\Stream;

abstract class Translator
{
    public abstract function understands (Stream $stream);
    public abstract function translate (Stream $stream);
}

?>