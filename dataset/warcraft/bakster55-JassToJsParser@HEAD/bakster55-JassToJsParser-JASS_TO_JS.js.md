---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "JASS_TO_JS.js"
ref: HEAD
---

var fs = require('fs');
var path = require('path');

var CommonParser = require('./common-parser.js');
var mapBasePath = "./war3map/Twilight's Eve ORPG Final All Full";

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
    



    // var antlr4 = require('antlr4');
    // var Parser = require('./antlr4JS/Jass/JassParser').JassParser;
    // var Lexer = require('./antlr4JS/Jass/JassLexer').JassLexer;

    // var jsContent = fs.readFileSync(jassFilePath, 'utf8');

    // var chars = new antlr4.InputStream(jsContent);
    // var lexer = new Lexer(chars);
    // var tokens  = new antlr4.CommonTokenStream(lexer);
    // var parser = new Parser(tokens);
    // parser.buildParseTrees = true;
    // var tree = parser.file();

    // tree.children.forEach(children => {
    //     var a = 5;
    // });



    ParseMapResources();

    var objectsTypes = CommonParser.GetObjectTypes([commonContent, blizzardContent, jassContent].join("\n"));

    // common.js
    var commonParsedContent = CommonParser.Parse(commonContent, objectsTypes);
    fs.writeFileSync(tempCommonPath, commonParsedContent);

    // blizzard.js
    var blizzardParsedContent = CommonParser.Parse(blizzardContent, objectsTypes);
    fs.writeFileSync(tempBlizzardPath, blizzardParsedContent);

    // war3map.js
    var war3MapParsedContent = CommonParser.Parse(jassContent, objectsTypes);

    war3MapParsedContent = war3MapParsedContent + "\nmain()";

    fs.writeFileSync(tempJassPath, war3MapParsedContent);
})();

function ParseMapResources() {
    var Translator = require('wc3maptranslator');
    
    if (!fs.existsSync(mapBasePath + '/parsed')){
        fs.mkdirSync(mapBasePath + '/parsed');
    }

    // Items - Objects
    // war3map.w3t
    const w3tFilePath = path.resolve(mapBasePath + '/war3map.w3t');
    fs.readFile(w3tFilePath, function(err, data) {
        if(err) return;
    
        var result = new Translator.Objects.warToJson('items', data);

        const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3t.json');
        fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    });
    
    // Abilities - Objects
    // war3map.w3a
    const w3aFilePath = path.resolve(mapBasePath + '/war3map.w3a');
    fs.readFile(w3aFilePath, function(err, data) {
        if(err) return;
    
        var result = new Translator.Objects.warToJson('abilities', data);

        const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3a.json');
        fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    });

    // Units - Objects
    // war3map.w3u
    const w3uFilePath = path.resolve(mapBasePath + '/war3map.w3u');
    fs.readFile(w3uFilePath, function(err, data) {
        if(err) return;
    
        var result = new Translator.Objects.warToJson('units', data);

        const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3u.json');
        fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    });

    // Trigger files: Strings
    // war3map.wts
    const wtsFilePath = path.resolve(mapBasePath + '/war3map.wts');
    fs.readFile(wtsFilePath, function(err, data) {
        if(err) return;
    
        var result = new Translator.Strings.warToJson(data);

        const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.wts.json');
        fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    });

    // Regions
    // war3map.w3r
    const w3rFilePath = path.resolve(mapBasePath + '/war3map.w3r');
    fs.readFile(w3rFilePath, function(err, data) {
        if(err) return;
    
        var result = new Translator.Regions.warToJson(data);

        const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3r.json');
        fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    });

    // Units
    // war3mapUnits.doo
    // const war3mapUnitsFilePath = path.resolve(mapBasePath + '/war3mapUnits.doo');
    // fs.readFile(war3mapUnitsFilePath, function(err, data) {
    //     if(err) return;
    
    //     var result = new Translator.Units.warToJson(data);

    //     const jsonPath = path.resolve(mapBasePath + '/parsed/war3mapUnits.doo.json');
    //     fs.writeFileSync(jsonPath, JSON.stringify(result.json, null, 4));
    // });
}