---
type: vendor-doc
source_repo: "https://github.com/jfhs/php-jass-parser"
source_path: "example.php"
ref: HEAD
---

<?php
//This example shows basic usage of JASS parser, by showing how you can get players start locations, using callback
include 'jass.php';

$file = $argv[1];
echo "Processing file $file\n";

$parser = new JASSParser(array('call_cb' => 'call_cb'/*, 'string_const_cb' => 'string_const_cb'*/));
$lexer = new JASSLexer(file_get_contents($file));

$start_locs = array();
$players_locs = array();

$parser->parse($lexer);
echo "Saving\n";

$parser->save('saved.j', array('indent' => false));

foreach($players_locs as $player=>$loc_id) {
	echo 'Player '.$player.' starts at '.$start_locs[$loc_id][0].':'.$start_locs[$loc_id][1]."\n";
}

function string_const_cb($str) {
    echo $str."\n";
}

function call_cb($function, $arguments) {
	$calculated_args = array();
	foreach($arguments as $k=>$arg) {
		$arg = JASSParser::recursive_calc_expr($arg);
		if (isset($arg['value'])) {
			$arg = $arg['value'];
		} else {
			$arg = null;
		}
		$calculated_args[$k] = $arg;
	}
	if ($function == 'DefineStartLocation') {
		list($id, $x, $y) = $calculated_args;
		if (($id === null) || ($x === null) || ($y === null)) {
			return;
		}
		global $start_locs;
		$start_locs[$id] = array($x, $y);
	} elseif (($function == 'SetPlayerStartLocation') || ($function == 'ForcePlayerStartLocation')) {
		list($id, $loc_id) = $calculated_args;
		if ($loc_id === null) {
			return;
		}
		if ($id == null) {
			$id = $arguments[0];
			if (($id['type'] == 'call') && ($id['id'] == 'Player')) {
				$pid = JASSParser::recursive_calc_expr($id['args'][0]);
				if (!isset($pid['value'])) {
					return;
				}
				$id = $pid['value'];
			} else {
				return;
			}
		}
		global $players_locs;
		$players_locs[$id] = $loc_id;
	}
}