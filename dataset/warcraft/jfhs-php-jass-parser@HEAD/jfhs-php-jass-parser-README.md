---
type: vendor-doc
source_repo: "https://github.com/jfhs/php-jass-parser"
source_path: "README.md"
ref: HEAD
---

# PHP JASS parser
JASS parser for PHP, which parse JASS into arrays of defined types, global vars, global constants, natives and functions with theirs local variables and list of statements.
## Usage
Usage is simple as:

	$parameters = array();
	$parser = new JASSParser($parameters);
	$lexer = new JASSLexer(file_get_contents('example.j'));
	$parser->parse($lexer);
	//now go throw $parser->globals, $parser->functions, etc.

## Callbacks
If you don't want to go throw parsed syntax tree of functions and want, i.e. only catch certain function calls, you can use a callback.
For now, only one callback is defined:
call_cb($function, $arguments)

You should pass callbacks in parameters array to JASSParser, when you create it, i.e:

	$parameters = array('call_cb' => 'call_cb');
	$parser = new JASSParser($parameters);
	//...
	function call_cb($function, $arguments) {
		echo $function." is called\n";
	}

## TODO
1. Lexer seems to be OK, but would be good if it will know position where he is so error messages can be more useful on big files.
2. Make folding function to use global constants and constant functions, allow user to define native functions, if he wants to, i.e. Player() or ConvertPlayerColor()
