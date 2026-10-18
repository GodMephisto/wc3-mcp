---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "war3map/blizzard-functions.js"
ref: HEAD
---

function ModuloInteger(dividend,divisor) {
    var modulus = dividend - R2I(dividend / divisor) * divisor
 
     
     
     
    if ((modulus < 0)) {
         modulus = modulus + divisor
    }
 
     return modulus
 }