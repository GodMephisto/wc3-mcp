---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "common-parser.js"
ref: HEAD
---

module.exports = {
    Parse: Parse,
    GetObjectTypes: GetObjectTypes,
};

function Parse(contents, objectsTypes) {
    contents = handleDivisionOperators(contents, objectsTypes);

    contents = replaceComments(contents);

    contents = replaceNative(contents);

    contents = replaceGlobals(contents);

    contents = replaceFunctions(contents);

    contents = replaceConstants(contents);

    contents = replaceTypes(contents);

    contents = replaceOther(contents);

    return contents;
}

function GetObjectTypes(contents) {

}

function handleDivisionOperators(contents, objectsTypes) {
    return contents;
}

function replaceComments(contents) {
    contents = contents.replace(/(")([^"\\]|\\.)*(")/gm, function(match, g1, g2, g3) {
        var result = match;
        if (match.indexOf("\n") !== -1) {
            result = "`" + g2 + "`";
        }

        // Handles comments pattern inside string literal.
        result = result.replace(/\/\//gm, "");

        return result;
    });

    contents = contents.replace(/\/\/.*$/gm, "");

    return contents;
}

function replaceNative(contents) {
    var functionRegEx = /(constant[\s]+)?native[\s]+([\w]+)[\s]+takes[\s]+([\w ,]+)[\s]+returns[\s]+\w+/gm;

    contents = contents.replace(functionRegEx, function (match, g1, functionName, jassFunctionArguments) {
        var jsFunctionArguments = jassFunctionArguments.split(",").map(arg => arg.trim().split(/[\s]+/)[1]);
        var jsFunctionArgumentsString = jsFunctionArguments.join(",");

        return "function " + functionName + "(" + jsFunctionArgumentsString + ") {}";
    });

    return contents;
}

function replaceGlobals(contents) {
    contents = contents.replace(/(globals(?=[\s]))|(endglobals(?=[\s]))/gm, "");

    return contents;
}

function replaceFunctions(contents) {
    var functionRegEx = /(constant[\s]+)?(function [\w]+)( takes )([\w ,]+)( returns \w+)/gm;

    contents = contents.replace(functionRegEx, function (match, g, g1, g2, jassFunctionArguments, g4) {
        var jsFunctionArguments = jassFunctionArguments.split(",").map(arg => arg.trim().split(" ")[1]);
        var jsFunctionArgumentsString = jsFunctionArguments.join(",");

        return g1 + "(" + jsFunctionArgumentsString + ") {";
    });

    contents = contents.replace(/endfunction/gm, "}");

    return contents;
}

function replaceConstants(contents) {
    var jassVariableTypes = [
        "integer",
        "string",
        "real",
        "boolean",
        "group",
        "itemtype",
        "item",
        "unit",
        "location",
        "effect",
        "timer",
        "trigger",
        "triggeraction",
        "player",
        "weathereffect",
        "version",
        "destructable",
        "gamecache",
        "timerdialog",
        "quest",
        "rect",
        "texttag",
        "force",
        "multiboard",
        "sound",
        "soundtype",
        "effecttype",
        "fogstate",
        "texmapflags",
        "raritycontrol",
        "blendmode",
        "camerafield",
        "unittype",
        "unitevent",
        "playercolor",
        "race",
        "playergameresult",
        "alliancetype",
        "attacktype",
        "damagetype",
        "playerevent",
        "gameevent",
        "playerscore",
        "playerstate",
        "weapontype",
        "pathingtype",
        "racepreference",
        "mapcontrol",
        "dialogevent",
        "widgetevent",
        "unitstate",
        "fgamestate",
        "igamestate",
        "playerslotstate",
        "gamespeed",
        "gametype",
        "mapflag",
        "placement",
        "startlocprio",
        "mapdensity",
        "gamedifficulty",
        "aidifficulty",
        "limitop",
        "volumegroup",
        "playerunitevent",
        "force",
        "boolexpr",
        "widget",
        "fogmodifier",
        "terraindeformation",
        "questitem",
        "defeatcondition",
        "leaderboard",
        "hashtable",
        "button",
        "lightning",
        "image",
        "ubersplat",
        "camerasetup",
        "region",
        "dialog",
        "multiboarditem",
        "eventid",
        "gamestate",
        "conditionfunc",
        "triggercondition",
    ];

    // replace arrays
    var jassVariableTypesRegex = new RegExp("([\\s])(constant[\\s]+)?(local[\\s]+)?(" + jassVariableTypes.join("|") + ")[\\s]+array[\\s]+([\\w_]+)", "gm");

    contents = contents.replace(jassVariableTypesRegex, "$1var $5 = []");

    // replace variables
    var jassVariableTypesRegex = new RegExp("([\\s])(constant[\\s]+)?(local[\\s]+)?(" + jassVariableTypes.join("|") + ")[\\s]+([\\w_]+)([\\s]*=)?", "gm");

    contents = contents.replace(jassVariableTypesRegex, "$1var $5$6");

    return contents;
}

function replaceTypes(contents) {
    contents = contents.replace(/type[\w\t ]+extends[\w\t\n ]+/gm, "");

    return contents;
}

function replaceOther(contents) {
    contents = contents.replace(/([^\w])(do|in|this)(?=[^\w])/gm, "$1$2$2");

    contents = contents.replace(/(set )|(call )/gm, "");

    contents = contents.replace(/([\s])(else)?if[\s]*([^\n]+?)[\s]*then/gm, "$1$2if ($3) {");
    contents = contents.replace(/([\s])endif(?=[\s])/gm, "$1}");
    contents = contents.replace(/([\s])else([\s])/gm, "$1} else {$2");
    contents = contents.replace(/([\s])elseif[\s]/gm, "$1} else if ");

    contents = contents.replace(/([\s])loop[\s]/gm, "$1do {");
    contents = contents.replace(/([\s])endloop([\s])/gm, "$1} while (true)$2");
    contents = contents.replace(/exitwhen([ ]?.*$)/gm, "if ($1) { break; }");

    //contents = contents.replace(/([^\W]+?)( ?= ?)([^\W]+)/gm, "$1 = $2;");

    // Hexadecimal
    contents = contents.replace(/\$([0-9A-F]+)/gm, function (value, group1) {
        return parseInt(group1, 16);
    })

    // Real with dot
    contents = contents.replace(/(\d+)\.([^\d])/gm, "$1.0$2");

    contents = contents.replace(/([")\s\d\]\W\d])or(["(\s\d\W\[])/gm, "$1||$2");
    contents = contents.replace(/([")\s\d\]])and(["(\s\d\W\[])/gm, "$1&&$2");


    // Syntax errors

    contents = contents.replace(/function (\w+\))/gm, "$1");

    contents = contents.replace(/([^\w])not([^\w])/gm, "$1!$2");

    contents = contents.replace(/length\(\)/gm, "length");

    contents = contents.replace(/function (\w+) \)/gm, "$1)");

    return contents;
}