---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-unit-sound.js"
ref: HEAD
---

import fs, { writeFileSync } from 'fs';

import csv from 'csv-parser';

function readSounds(path) {
    return new Promise((resolve) => {
        const sounds = [];
        fs.createReadStream(path)
            .pipe(csv())
            .on('data', (data) => {
                data["SoundName"] = data[Object.keys(data)[0]];
                delete data[Object.keys(data)[0]];
                data["Channel"] = parseInt(data["Channel"]);
                sounds.push(data);
            })
            .on('end', () => {
                resolve(sounds);
            });
    });
}

function readUnitUI() {
    return new Promise((resolve) => {
        const units = [];
        fs.createReadStream('manual-data/unitUI.csv')
            .pipe(csv())
            .on('data', (data) => {
                data["unitUIID"] = data[Object.keys(data)[0]];
                units.push(data);
            })
            .on('end', () => {
                resolve(units);
            });
    });
}

function cleanDuplicatedSounds(sounds) {
    // Remove duplicated rows with same SoundName, keep the one with highest version
    const soundMap = new Map();
    for (const sound of sounds) {
        const existing = soundMap.get(sound.SoundName);
        // If this is the first time encountering the SoundName or this version is higher, update the map
        if (!existing || sound.version > existing.version) {
            soundMap.set(sound.SoundName, sound);
        }
    }
    // Convert the map back to an array of the filtered sounds
    return Array.from(soundMap.values());
}

const allSounds = cleanDuplicatedSounds([
    ...await readSounds('manual-data/UnitAckSounds.csv'),
    ...await readSounds('manual-data/UnitAckSounds-2.csv'),
]);
console.log("Found " + allSounds.length + " sounds");

const units = await readUnitUI();

const suffices = [];

function match(unitSound, soundName) {
    if (unitSound === "owl" && soundName === "SnowOwlWhat") {
        let x;
    }

    if ([
        ["owl", "SnowOwlWhat"]
    ]
        .map(([s1, s2]) => s1.toLowerCase() + ":" + s2.toLowerCase())
        .includes(unitSound.toLowerCase() + ":" + soundName.toLowerCase())) return true;

    return [
        'What',
        'Pissed',
        'YesAttack',
        'Yes',
        'Ready',
        'Warcry',
    ].map(sf => (unitSound + sf).toLowerCase()).includes(soundName.toLowerCase());
}

const result = units.map(u => {
    if (u.unitUIID === "nowl") {
        let x;
    }
    const sounds = u.unitSound !== "" ? allSounds.filter(s => match(u.unitSound, s.SoundName)) : [];
    if (sounds.length === 0) {
        console.warn("Cannot find sound for", u.name, u.unitSound, u.file);
    }

    const data = {
        id: u.unitUIID,
        name: u.name,
        sounds: {
            What: sounds.filter(s => s.SoundName.endsWith("What")),
            Pissed: sounds.filter(s => s.SoundName.endsWith("Pissed")),
            YesAttack: sounds.filter(s => s.SoundName.endsWith("YesAttack")),
            Yes: sounds.filter(s => s.SoundName.endsWith("Yes")),
            Ready: sounds.filter(s => s.SoundName.endsWith("Ready")),
            Warcry: sounds.filter(s => s.SoundName.endsWith("Warcry")),
        }
    };
    if (Object.keys(data.sounds).flatMap(k => data.sounds[k]).length !== sounds.length) {
        console.warn("Split not equal", u.name);
    }

    sounds.forEach(s => {
        const suff = s.SoundName.slice(u.unitSound.length);
        // if (suff === "ArcherWhat") {
        //     console.log(u, s);
        // }
        suffices.push(suff);
    });

    return data;
});

console.log("suffices", new Set(suffices));
console.log("----------");
console.log("Volume", new Set(allSounds.map(s => s.Volume)));
console.log("Pitch", new Set(allSounds.map(s => s.Pitch)));
console.log("PitchVariance", new Set(allSounds.map(s => s.PitchVariance)));
console.log("Priority", new Set(allSounds.map(s => s.Priority)));
console.log("Channel", new Set(allSounds.map(s => s.Channel)));
console.log("Flags", new Set(allSounds.map(s => s.Flags)));
console.log("MinDistance", new Set(allSounds.map(s => s.MinDistance)));
console.log("MaxDistance", new Set(allSounds.map(s => s.MaxDistance)));
console.log("DistanceCutoff", new Set(allSounds.map(s => s.DistanceCutoff)));
console.log("EAXFlags", new Set(allSounds.map(s => s.EAXFlags)));

allSounds.forEach(data => {
    delete data.Volume;
    delete data.Pitch;
    delete data.PitchVariance;
    delete data.Priority;
    delete data.Flags;
    delete data.MinDistance;
    delete data.MaxDistance;
    delete data.DistanceCutoff;
    delete data.EAXFlags;
    delete data.InBeta;
    delete data.version;
});

fs.writeFileSync("object-data/unit-sounds.json", JSON.stringify(result, null, 2));