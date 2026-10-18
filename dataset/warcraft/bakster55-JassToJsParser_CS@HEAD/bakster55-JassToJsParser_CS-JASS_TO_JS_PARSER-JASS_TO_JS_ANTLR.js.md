---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JASS_TO_JS_PARSER/JASS_TO_JS_ANTLR.js"
ref: HEAD
---

var fs = require('fs');
var path = require('path');

var CommonParser = require('./common-parser.js');
var mapBasePath = "./war3map/HM_RPG_v7_08z_Eng_slk";

(function () {

    var jassFilePath = path.resolve(mapBasePath + '/war3map.j');
    var commonFilePath = path.resolve('./war3map/common.j');
    var blizzardFilePath = path.resolve('./war3map/blizzard.j');

    var tempCommonPath = path.resolve('./war3map/common.js');
    var tempBlizzardPath = path.resolve('./war3map/blizzard.js');
    var tempJassPath = path.resolve('./war3map/war3map.js');

    var commonContent = fs.readFileSync(commonFilePath, 'utf8');
    var blizzardContent = fs.readFileSync(blizzardFilePath, 'utf8');
    var jassContent = fs.readFileSync(jassFilePath, 'utf8');

    var jassContent = fs.readFileSync(jassFilePath, 'utf8');

    Parse(jassContent);

    // var objectsTypes = CommonParser.GetObjectTypes([commonContent, blizzardContent, jassContent].join("\n"));

    // // common.js
    // var commonParsedContent = CommonParser.Parse(commonContent, objectsTypes);
    // fs.writeFileSync(tempCommonPath, commonParsedContent);

    // // blizzard.js
    // var blizzardParsedContent = CommonParser.Parse(blizzardContent, objectsTypes);
    // fs.writeFileSync(tempBlizzardPath, blizzardParsedContent);

    // // war3map.js
    // var war3MapParsedContent = CommonParser.Parse(jassContent, objectsTypes);

    // war3MapParsedContent = war3MapParsedContent + "\nmain()";

    // fs.writeFileSync(tempJassPath, war3MapParsedContent);
})();

function Parse(jassContent) {
    var antlr4 = require('antlr4');
    var Parser = require('./antlr4JS/Jass/JassParser').JassParser;
    var Lexer = require('./antlr4JS/Jass/JassLexer').JassLexer;
    var Listener = require('./antlr4JS/Jass/JassListener').JassListener;
    var JassVisitor = require('./antlr4JS/Jass/JassVisitor').JassVisitor;

    var chars = new antlr4.InputStream(jassContent);
    var lexer = new Lexer(chars);
    var tokens = new antlr4.CommonTokenStream(lexer);
    var parser = new Parser(tokens);
    var listener = new Listener();
    parser.addParseListener(listener);
    parser.buildParseTrees = true;

    var tree = parser.file();

    var visitor = new JassVisitor();
    var result = visitor.visit(tree);

    //var b = flatten(a);

    //var treeString = tree.toStringTree(parser.ruleNames);

    const antlrTreePath = path.resolve('./antlrTree.js');
    fs.writeFileSync(antlrTreePath, result);
}