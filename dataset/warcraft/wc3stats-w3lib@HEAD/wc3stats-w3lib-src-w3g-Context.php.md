---
type: vendor-doc
source_repo: "https://github.com/wc3stats/w3lib"
source_path: "src/w3g/Context.php"
ref: HEAD
---

<?php

namespace w3lib\w3g;

class Context
{
    public static $settings;
    public static $replay;
    public static $time;
    public static $leavers;

    public static function majorVersion ()
    {
        return self::$replay->header->majorVersion;
    }

    public static function getTime ()
    {
        return floor (self::$time);
    }

    public static function isReforged ()
    {
        return self::majorVersion () >= Parser::WC3_VERSION_32;
    }
}

?>