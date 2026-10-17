---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Linebreak-fixer.md"
ref: HEAD
---

# Linebreak fixer

As a side effect of the parsing necessary for this project it also replaces line breaks inside Jass
string literals into properly escaped `\n` this also fixes an issue with PJass giving the wrong line
number if there are strings with linebreaks.