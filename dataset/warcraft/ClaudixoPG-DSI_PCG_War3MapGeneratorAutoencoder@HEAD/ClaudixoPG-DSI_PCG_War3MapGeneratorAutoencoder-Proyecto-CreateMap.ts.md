---
type: vendor-doc
source_repo: "https://github.com/ClaudixoPG/DSI_PCG_War3MapGeneratorAutoencoder"
source_path: "Proyecto/CreateMap.ts"
ref: HEAD
---


const fs = require('fs');

import {
    TerrainTranslator,
  } from 'wc3maptranslator';

  //make a for loop to iterate through all the files in the folder
    //for each folder, search for the .w3e file and convert it to json
    //then save the json file in the folder "ConvertedMaps" in a route like this: "ConvertedMaps\mapName.json"

    var mapsFolder = "D:\\DSI_PCG\\OutputMaps\\SeparatedMaps";
    //var mapsFolder = "C:\\Users\\claud\\Desktop\\asd";

    var maps = fs.readdirSync(mapsFolder);


    for (var i = 0; i < maps.length; i++) {
        var mapName = maps[i];

        console.log(`Converting map ${mapName} to json.`);

        //open json file and save it in a variable
        var mapPath = mapsFolder + "\\" + mapName;
        var mapBuffer = fs.readFileSync(mapPath);


        //check if the map buffer is empty
        if (mapBuffer.length == 0) {
            console.log(`Skipping map ${mapName} as it's empty.`);
            continue; // Jump to the next iteration of the loop
        }

        var jsonFile = JSON.parse(mapBuffer);

        //console.log(jsonFile.json.tileset);

        //var Terrain = TerrainTranslator.jsonToWar(jsonFile.json);

        try{
            var Terrain = TerrainTranslator.jsonToWar(jsonFile.json);
        } catch (error) {
            console.log(`Skipping map ${mapName} as it's corrupted.`);
            console.log(mapBuffer)
            continue; // Jump to the next iteration of the loop
        }

        //route to save the json file
        var mapJsonPath = "D:\\DSI_PCG\\OutputMaps\\FinalMaps" + "\\" + mapName + "\\" + 'war3map.w3e';

        fs.writeFileSync('war3map.w3e', Terrain.buffer);
        //var mapJsonPath = "D:\\DSI_PCG\\OutputMaps\\FinalMaps" + "\\" + mapName;
        //fs.writeFileSync(mapJsonPath, JSON.stringify(Terrain));

    }
