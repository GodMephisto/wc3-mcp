---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "src/bundle-voice.js"
ref: HEAD
---

import ffmpeg from 'fluent-ffmpeg';
import fs from 'fs';
import path from 'path';

ffmpeg.setFfmpegPath('bin\\ffmpeg-7.0.2-essentials_build\\bin\\ffmpeg.exe');
ffmpeg.setFfprobePath('bin\\ffmpeg-7.0.2-essentials_build\\bin\\ffprobe.exe');

// const mode = "find-voice";
const mode = "bundle-voice";

// Directory where your input files are located
const speakerName = "LichKing";

const inputDir = 'manual-data\\voice-packs\\'+speakerName;
const outputFile = 'output\\voice-packs\\'+speakerName+".mp3"; // Change to desired output path and format

const silenceFilePath = 'manual-data\\voice-packs\\silence.wav'; // Path to the pre-generated silent audio file
const delaySeconds = 1; // Delay in seconds between each file

// Uncomment these 2 lines to copy matching files from extracted-mpq to inputDir
if (mode === "find-voice") {
    console.log("Finding voice files...");
    await copyMatchingFiles();
}
if (mode === "bundle-voice") {
    console.log("Bundling voice files...");
    generate();
}

// Async function to list all files with pattern *{speakerName}*.mp3 in folder extracted-mpq
// and copy them to inputDir
async function copyMatchingFiles() {
    const sourceDir = 'extracted-mpq';

    async function copyRecursively(dir) {
        try {
            const entries = await fs.promises.readdir(dir, { withFileTypes: true });
            for (const entry of entries) {
                const fullPath = path.join(dir, entry.name);
                if (entry.isDirectory()) {
                    await copyRecursively(fullPath);
                } else if (entry.isFile() && 
                            entry.name.toLowerCase().includes(speakerName.toLowerCase()) && 
                            [".mp3", ".wav"].includes(path.extname(entry.name).toLowerCase()) &&
                            !entry.name.toLowerCase().includes("death")
                        ) {
                    const relativePath = path.relative(sourceDir, fullPath);
                    const destPath = path.join(inputDir, entry.name);
                    await fs.promises.mkdir(path.dirname(destPath), { recursive: true });
                    await fs.promises.copyFile(fullPath, destPath);
                    console.log(`Copied ${relativePath} to ${destPath}`);
                }
            }
        } catch (error) {
            console.error(`Error processing directory ${dir}:`, error);
        }
    }

    try {
        await copyRecursively(sourceDir);
        console.log('File copying completed.');
    } catch (error) {
        console.error('Error copying files:', error);
    }
}

// Function to combine audio files
function combineAudioFiles(fileList, silenceFile, outputPath) {
    let command = ffmpeg();

    fileList.forEach((file, index) => {
        command = command.input(file);
        if (index < fileList.length - 1) {
            // command = command.input(silenceFile);
        }
    });

    command
        .on('end', () => {
            console.log('Files have been merged successfully');
        })
        .on('error', (err) => {
            console.log('Error merging files:', err.message);
        })
        .mergeToFile(outputPath);
};

// Get all mp3 and wav files in the directory
function getAudioFiles(dir) {
    return fs.readdirSync(dir)
        .filter(file => file.endsWith('.mp3') || file.endsWith('.wav') || file.endsWith('.ogg'))
        .map(file => path.join(dir, file));
};

// Main function
function generate() {
    const audioFiles = getAudioFiles(inputDir);
    if (audioFiles.length === 0) {
        console.log('No audio files found in the directory.');
        return;
    }

    combineAudioFiles(audioFiles, silenceFilePath, outputFile);
};
