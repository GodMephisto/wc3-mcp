---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/War3Map/app.js"
ref: HEAD
---

window["war3map.w3u"] = require("./war3map/parsed/war3map.w3u.json");
window["War3mapMisc"] = require("./war3map/parsed/War3mapMisc.json");

require("script-loader!./common.js");
require("script-loader!./common-functions.js");
require("script-loader!./blizzard.js");
require("script-loader!./war3map/war3map.js");
