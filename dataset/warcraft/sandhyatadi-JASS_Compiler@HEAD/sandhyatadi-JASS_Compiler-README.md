---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/JASS_Compiler"
source_path: "README.md"
ref: HEAD
---

# JASS_Compiler
Compiler and Virtual Machine for a Programming Language: JASS 
Project: Compiler and Virtual Machine for a Programming Language

Name of the language created: JASS
Tools Used: SWI-Prolog , IDLE Shell 3.9.4 for building lexer in Python

*Directions/Instructions to install the tools for building this language: SWI-Prolog is downloaded from " https://www.swi-prolog.org/download/stable ". Python 3.9.4 is downloaded from " https://www.python.org/downloads/ ". 
This language requires the package PLY. This can be installed by entering the following commands:

python -m pip install --upgrade pip setuptools wheel pip install ply

*Directions to build and run the language: All the lexer.py, parser.pl, main.pl, script.bat and the test codes(.jass) files must be present in the same folder. First set the path of the folder in which the lexer, 
parser and the test codes are present in the command prompt. The Batch Script file has to be passed along with the Test code file name. Following is the command to pass: " script.bat testcodefile.jass ". 
The Batch Script contains the following commands: " python lexer.py %1 " " swipl -q -t main main.pl" main.pl contains the path of the parser. The output is displayed on the Terminal.
