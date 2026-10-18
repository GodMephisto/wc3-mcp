---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JASS_TO_JS_PARSER/ParseWar3MapResources.js"
ref: HEAD
---

var fs = require("fs");
var path = require("path");

var CommonParser = require("./common-parser.js");
var mapBasePath = "./war3map/LOM_RPG_2.38A_translated";
var recreatedAndDeoptimized = false;

(function () {
  //var jassFilePath = path.resolve(mapBasePath + '/war3map.j');
  //var commonFilePath = path.resolve('./war3map/common.j');
  //var blizzardFilePath = path.resolve('./war3map/blizzard.j');

  //var tempCommonPath = path.resolve('./war3map/common.js');
  //var tempBlizzardPath = path.resolve('./war3map/blizzard.js');
  //var tempJassPath = path.resolve('./war3map/war3map.js');

  //var commonContent = fs.readFileSync(commonFilePath, 'utf8');
  //var blizzardContent = fs.readFileSync(blizzardFilePath, 'utf8');
  //var jassContent = fs.readFileSync(jassFilePath, 'utf8');

  ParseMapResources();

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

function ParseMapResources() {
  var Translator = require("wc3maptranslator");

  if (!fs.existsSync(mapBasePath + "/parsed")) {
    fs.mkdirSync(mapBasePath + "/parsed");
  }

  if (!fs.existsSync(mapBasePath + "/recreated")) {
    fs.mkdirSync(mapBasePath + "/recreated");
  }

  if (!fs.existsSync(mapBasePath + "/deoptimized")) {
    fs.mkdirSync(mapBasePath + "/deoptimized");
  }

  // Items - Objects
  // war3map.w3t
  const w3tFilePath = path.resolve(mapBasePath + "/war3map.w3t");
  fs.readFile(w3tFilePath, function (err, data) {
    if (err) return;

    // Parsed
    var jsonResult = Translator.ObjectsTranslator.warToJson("items", data);
    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.w3t.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));

    if (recreatedAndDeoptimized) {
      // Recreated
      var warResult = new Translator.Objects.jsonToWar("items", jsonResult.json);
      const warPath = path.resolve(mapBasePath + "/recreated/war3map.w3t");
      fs.writeFileSync(warPath, warResult);

      // Deoptimized
      var deoptimizedJson = Deoptimize(jsonResult.json);
      const deoptimizedJsonPath = path.resolve(
        mapBasePath + "/deoptimized/war3map.w3t.json"
      );
      fs.writeFileSync(
        deoptimizedJsonPath,
        JSON.stringify(deoptimizedJson, null, 4)
      );
    }
  });

  // Abilities - Objects
  // war3map.w3a
  const w3aFilePath = path.resolve(mapBasePath + "/war3map.w3a");
  fs.readFile(w3aFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.ObjectsTranslator.warToJson("abilities", data);
    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.w3a.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));

    if (recreatedAndDeoptimized) {
      // Recreated
      var warResult = new Translator.Objects.jsonToWar(
        "abilities",
        jsonResult.json
      );
      const warPath = path.resolve(mapBasePath + "/recreated/war3map.w3a");
      fs.writeFileSync(warPath, warResult.buffer);

      // Deoptimized
      var deoptimizedJson = Deoptimize(jsonResult.json);
      const deoptimizedJsonPath = path.resolve(
        mapBasePath + "/deoptimized/war3map.w3a.json"
      );
      fs.writeFileSync(
        deoptimizedJsonPath,
        JSON.stringify(deoptimizedJson, null, 4)
      );
    }
  });

  // Units - Objects
  // war3map.w3u
  const w3uFilePath = path.resolve(mapBasePath + "/war3map.w3u");
  fs.readFile(w3uFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.ObjectsTranslator.warToJson("units", data);

    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.w3u.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));

    if (recreatedAndDeoptimized) {
      // Deoptimized
      var deoptimizedJson = Deoptimize(jsonResult.json);
      const deoptimizedJsonPath = path.resolve(
        mapBasePath + "/deoptimized/war3map.w3u.json"
      );
      fs.writeFileSync(
        deoptimizedJsonPath,
        JSON.stringify(deoptimizedJson, null, 4)
      );
    }
  });

  // Upgrades - Objects
  // war3map.w3q
  const w3qFilePath = path.resolve(mapBasePath + "/war3map.w3q");
  fs.readFile(w3qFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.ObjectsTranslator.warToJson("upgrades", data);

    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.w3q.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  });

  // Destructables - Objects
  // war3map.w3b
  const w3bFilePath = path.resolve(mapBasePath + "/war3map.w3b");
  fs.readFile(w3bFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.ObjectsTranslator.warToJson("destructables", data);

    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.w3b.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  });

  // Trigger files: Strings
  // war3map.wts
  const wtsFilePath = path.resolve(mapBasePath + "/war3map.wts");
  fs.readFile(wtsFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.StringsTranslator.warToJson(data);

    const jsonPath = path.resolve(mapBasePath + "/parsed/war3map.wts.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  });

  // Units
  // war3mapUnits.doo
  // const war3mapUnitsFilePath = path.resolve(mapBasePath + '/war3mapUnits.doo');
  // fs.readFile(war3mapUnitsFilePath, function (err, data) {
  //   if (err) return;

  //   var jsonResult = Translator.DoodadsTranslator.warToJson(data);

  //   const jsonPath = path.resolve(mapBasePath + '/parsed/war3mapUnits.doo.json');
  //   fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  // });

  //Doodads
  //war3map.doo
  // const war3mapDooFilePath = path.resolve(mapBasePath + '/war3map.doo');
  // fs.readFile(war3mapDooFilePath, function (err, data) {
  //   if (err) return;

  //   var jsonResult = Translator.DoodadsTranslator.warToJson(data);

  //   const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.doo.json');
  //   fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  // });

  //Doodads
  //war3map.w3d
  const w3dFilePath = path.resolve(mapBasePath + '/war3map.w3d');
  fs.readFile(w3dFilePath, function (err, data) {
    if (err) return;

    var jsonResult = Translator.ObjectsTranslator.warToJson("doodads", data);

    const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3d.json');
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  });

  // Info File
  // war3map.w3i
  // const w3iFilePath = path.resolve(mapBasePath + '/war3map.w3i');
  // fs.readFile(w3aFilePath, function (err, data) {
  //   if (err) return;

  //   var jsonResult = Translator.InfoTranslator.warToJson(data);
  //   const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3i.json');
  //   fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));

  //   if (recreatedAndDeoptimized) {
  //     // Recreated
  //     var warResult = new Translator.Info.jsonToWar(jsonResult.json);
  //     const warPath = path.resolve(mapBasePath + '/recreated/war3map.w3i');
  //     fs.writeFileSync(warPath, warResult.buffer);
  //   }
  // });

  // Regions
  // war3map.w3r
  // const w3rFilePath = path.resolve(mapBasePath + '/war3map.w3r');
  // fs.readFile(w3rFilePath, function(err, data) {
  //     if(err) return;

  //     var jsonResult = Translator.RegionsTranslator.warToJson(data);

  //     const jsonPath = path.resolve(mapBasePath + '/parsed/war3map.w3r.json');
  //     fs.writeFileSync(jsonPath, JSON.stringify(jsonResult.json, null, 4));
  // });

  // Misc files
  // War3mapMisc.wts
  const war3mapMisc = path.resolve(mapBasePath + "/War3mapMisc.txt");
  fs.readFile(war3mapMisc, { encoding: "utf8" }, function (err, data) {
    if (err) return;

    var jsonResult = {};
    var lines = data.split("\n");
    lines.forEach((line) => {
      var keyValue = line.split("=");
      if (keyValue.length == 2) {
        var key = keyValue[0];
        var value = keyValue[1];
        var parsedValue = Number(value);
        jsonResult[key] = !isNaN(parsedValue) ? parsedValue : value;
      }
    });

    const jsonPath = path.resolve(mapBasePath + "/parsed/War3mapMisc.json");
    fs.writeFileSync(jsonPath, JSON.stringify(jsonResult, null, 4));
  });
}

function Deoptimize(json) {
  var keys = Object.keys(json.custom);
  for (let index = 0; index < keys.length; index++) {
    const key = keys[index];
    var fourcc1 = key.split(":")[0];
    var fourcc2 = key.split(":")[1];

    var fourccNum =
      To8Bit(fourcc1.charCodeAt(0)) * 256 * 256 * 256 +
      To8Bit(fourcc1.charCodeAt(1)) * 256 * 256 +
      To8Bit(fourcc1.charCodeAt(2)) * 256 +
      To8Bit(fourcc1.charCodeAt(3));

    var newKey = fourccNum + ":" + fourcc2;
    json.custom[newKey] = json.custom[key];
    delete json.custom[key];
  }

  return json;
}

function To8Bit(num) {
  if (num < 127 && num > -128) {
    return num;
  }

  var sign = 1;
  if (num < 0) {
    sign = -1;
  }

  return sign * ~(((num * sign) & 127) ^ 127);
}
