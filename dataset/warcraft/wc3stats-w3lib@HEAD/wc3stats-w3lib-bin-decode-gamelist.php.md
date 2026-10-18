---
type: vendor-doc
source_repo: "https://github.com/wc3stats/w3lib"
source_path: "bin/decode-gamelist.php"
ref: HEAD
---

<?php

require 'base.php';

use w3lib\Library\Stream\Buffer;
use w3lib\Gamelist;

$record = base64_decode ($argv [1]);
$record = new Buffer ($record);
$record = Gamelist::unpack ($record);

var_dump ($record);
die ();

?>