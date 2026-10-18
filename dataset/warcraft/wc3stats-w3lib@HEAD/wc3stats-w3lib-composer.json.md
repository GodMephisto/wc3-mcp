---
type: vendor-doc
source_repo: "https://github.com/wc3stats/w3lib"
source_path: "composer.json"
ref: HEAD
---

{
    "name": "anders/w3lib",
    "authors": [
        {
            "name": "Ethan Anders",
            "email": "ethananders97@gmail.com"
        }
    ],
    "require": {
        "monolog/monolog": "^3.0",
        "bramus/monolog-colored-line-formatter": "~3.0"
    },
    "autoload": {
        "psr-4" : {
            "w3lib\\Library\\": "lib/",
            "w3lib\\" : "src/"
        },
        "files" : [
	        "lib/binary.php",
            "lib/functions.php"
        ]
    }
}
