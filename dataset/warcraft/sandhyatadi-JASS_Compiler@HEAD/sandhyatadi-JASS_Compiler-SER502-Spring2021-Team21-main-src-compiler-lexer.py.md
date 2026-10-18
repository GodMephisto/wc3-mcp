---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/JASS_Compiler"
source_path: "SER502-Spring2021-Team21-main/src/compiler/lexer.py"
ref: HEAD
---

# Author - Sonia Kalidindi, Jigisha Deven Gadhia
import sys
import os
import ply.lex as lex

#Dictionary of the reserved Tokens
reserved_keywords = {
    'true':'True',
    'false':'False',
    'print':'Print',
    'and':'And',
    'or':'Or',
    'not':'Not',
    'if':'If',
    'else':'Else',
    'elseif':'ElseIf',
    'for':'For',
    'while':'While',
    'in':'In',
    'range':'Range',
    'define':'Func',
    'strlen':'Func',
    'int':'Var',
    'char':'Var',
    'string':'Var',
    'float':'Var',
    'bool':'Var',
    'return':'Return'
   
}
tokens = ['Identifier','True','False','Var','Print','Number','String','If','ElseIf','Else',
    'While','For','In','Range','Grt','Les','Incr','Decr','BoolEqual','GrtEqual','LesEqual',
    'BoolNotEqual','Or','And','Not','Func','Return']
 
literals = ['+','-','*','/','(',')','{','}','=','!','%',';',',','#','?',':'] 

#Set of Regular Expressions
t_Incr = r'\+\+'
t_Decr = r'--'
t_BoolEqual = r'=='
t_BoolNotEqual = r'!='
t_Les = r'\>\>'
t_Grt = r'\<\<'
t_LesEqual = r'>='
t_GrtEqual = r'<='

#Checking for number, string
def t_Number(t):
    r'\d+'
    t.value = int(t.value)
    return t

def t_String(t):
    r'"(.*?)"'
    return t
#checking for alpha numerial value
def t_Identifier(t):
    r'[a-zA-Z_][a-zA-Z_0-9]*'
    t.type = reserved_keywords.get(t.value,'Identifier')   
    return t
#to keep track of the line number
def t_newline(t):
    r'\n+'
    t.lexer.lineno += len(t.value)
    
#ignoring the white spaces and tab spaces
t_ignore  = ' \t '

#error handling
def t_error(t):
    print("Character '%s' not recognized" % t.value[0])
    t.lexer.skip(1)

 
def test(d,lexer):
    tokens = []
    lexer.input(d)
    #'.' is added at the end of each token
    while True: 
        s_token = lexer.token()
        if not s_token:
          break
        if s_token.type=='Number':
            tokens.append(str(s_token.value)+".")
        elif s_token.type=='String':
            tokens.append(s_token.value+".")
        else:
            tokens.append("'"+str(s_token.value)+"'"+'.')
    return tokens
def main(argv):
    if argv[-4:] != "jass": #Checking for the right extension
        sys.exit("File with .jass extension is required") #exits with displaying error msg
    cd = os.curdir
    d = ""

    f_path = os.path.abspath(os.path.join(cd, '.')) + "\\" + argv
    with open(f_path, 'r') as file:
        d = file.read()
    Tokens = test(d,lex.lex())
    firstfile=open("file.txt","w") #to write the file.txt for each time the code is run
    for i in Tokens:
        firstfile.write('\n')
        firstfile.write(i)
    #Produces the following output when the tokens are sucessfully built.    
    print("Tokens are successfully generated")
    firstfile.close()
if __name__== "__main__":
    main(sys.argv[1])
