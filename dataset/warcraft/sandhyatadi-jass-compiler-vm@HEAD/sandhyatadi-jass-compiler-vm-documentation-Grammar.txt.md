---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "documentation/Grammar.txt"
ref: HEAD
---

Program ::= { Block } 
Block ::= Declaration ; Command

Declaration ::= Declaration ; Declaration | Initialize | int Identifier | string Identifier | bool Identifier
Command ::= Command Command | Identifier = Expression ; | Identifier = String ; | If | While |
            For | Print ;|UnaryOp ; | StringFunction |  

Initialize ::= int Identifier = Integer | char Identifier = Character | string Identifier = String |bool Identifier = Boolean | Declaration | Null
NewIdentifier ::= Identifier | NewIdentifier , IdentifierStringFunction ::= StringLength
StringLength ::= strlen ( Identifier ) | strlen ( String )
Ternary ::= ( Boolean ) ? { Expression } : { Expression }
If ::= if ( Boolean ) { Block } else { Block } | if ( Boolean ) { Block } Elseif
Elseif ::= elseif ( Boolean ) { Block } else { Block } | elseif ( Boolean ) { Block } Elseif |
EMPTY
For ::=for ( Initialize ; Boolean ; UnaryOp ) { Block } | for Identifier in range ( Integer , Integer ){ Block }
While ::=while ( Boolean ) { Block }
UnaryOp ::= Identifier++ | Identifier--

Expression ::= Expression + Expression | Expression - Expression | Expression * Expression |
Expression / Expression | Expression % Expression | ( Expression ) | Integer | String | Ternary | Identifier | Identifier = Expression
Boolean ::= true | false | Expression == Expression | Expression >> Expression | Expression << Expression |
Expression <= Expression | Expression >= Expression | not Boolean | Expression
              and Expression | Expression or Expression | Expression | Boolean and Boolean | Boolean or Boolean
Identifier ::= Alphabets | Identifier Identifier

Print ::= print ( Statement)
Statement ::= Statement Statement | Identifier | “ String ” | EMPTY

String ::= Alphabets | Alphabets String | EMPTY
Alphabets ::= a | b | c | d | e | f | g | h | i | j | k | l | m | n | o | p | q | r | s | t | u | v | x | y | z | A | B | C |
D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | X | Y | Z

Integer ::= Digits
Digits ::= Digit | Digit Digits
Digit ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
