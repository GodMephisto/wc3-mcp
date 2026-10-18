---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "src/generate-dialogues.ts"
ref: HEAD
---

import { ElevenLabsClient } from "elevenlabs";
import 'dotenv/config';
import fs from "fs"
import { Voice } from "elevenlabs/api";
import assert from "assert"

const questName = "test"

const client = new ElevenLabsClient({
  apiKey: process.env.ELEVENLABS_API_KEY
});


const VoiceDescriptions = {
  "Footman": "Voice of a middle age American footman.",
  "Knight": "Middle age warrior man. Strong and aggressive voice.",
  "Rifleman": "Old dwarf rifleman.",
  "Grunt": "Voice of a Orc warrior, highly aggressive.",
  "MortarTeam": "Voice of an old dwarf man.",
  "Peasant": "Voice of a peasant, weak mind and coward.",
  "VillagerMan": "Voice of a male villager, weak mind and coward.",
  "VillagerMan2": "Voice of a male villager, weak mind and coward.",
  "VillagerWoman": "Voice of a male villager, soft and feminine.",
  "VillagerKid": "Voice of 4 years old child.",
  "Paladin": "Voice of a Paladin.",
  "Archmage": "Voice of an old Archmage.",
  "Bandit": "Voice of young male Bandit, rogue in nature.",
}
type VoiceName = (keyof typeof VoiceDescriptions);
const allVoiceNames = Object.keys(VoiceDescriptions) as VoiceName[];

const actualRun = true;
// const actualRun = false;

/**
 * Code
 */
const workingDir = `output/generated-voices/${questName}`
const inputPath = workingDir + "/input.txt"

// backup input
const backupPath = workingDir + "/input.bak.txt"
fs.writeFileSync(backupPath, fs.readFileSync(inputPath).toString())

// validate data
const dialogues = fs.readFileSync(inputPath).toString()
  // .replace(/\r/g, "")
  .split("\n\n")
  .filter(l => l.trim().length > 0)
  .map(l => l.split("\n").filter(l2 => l2.length > 0).join("\n"));

dialogues.forEach(raw => {
  const lines = raw.split("\n")
  assert.ok(lines.length === 4, "Must have 4 lines, but found " + lines.length + ". Raw: " + raw)
  const {reqId, voiceName, speaker, prevText, text} = parse(raw)
  assert.ok(allVoiceNames.includes(voiceName), "unexpect voice " + voiceName)
  assert.ok(speaker.length > 0)
  assert.ok(prevText.length > 0)
  assert.ok(text.length > 0)
  assert.ok(reqId === undefined || reqId.length > 5)
})


// Setup client
const { voiceMap, voiceLimit } = await setup();
console.log("Voice limit:", voiceLimit);
const voiceKeys = [...voiceMap.keys()]
console.log(`Existing voices (${voiceKeys.length}): `, voiceKeys);

// Process data
let prevReqId: string | undefined
const data: {speaker: string, text: string, fileName: string }[] = []
const newContent: string[] = []
let i = 0

for(const dialogue of dialogues) {
  const parsed = parse(dialogue)
  const {voiceName, speaker, prevText, text} = parsed;
  let reqId = parsed.reqId

  const fileName = `${questName}-${i++}-${voiceName.toLowerCase()}.mp3`;
  const outputFilePath = `output/generated-voices/${questName}/${fileName}`;

  console.log(voiceName, fileName)
  if (!reqId) {
    if (actualRun) {
      reqId = await generate(voiceName, prevText, `"${text}"`, outputFilePath, prevReqId);
      console.log({reqId, prevReqId})
    } else {
      console.log("mock generate", {voiceName, prevReqId, text})
      reqId = ""
    }
  }
  prevReqId = reqId
  data.push({
    speaker, text: text, fileName
  })
  newContent.push(serialize({reqId, voiceName, speaker, prevText, text}))
}

fs.writeFileSync(inputPath, newContent.join("\n"))
fs.writeFileSync(`${workingDir}/code.ts`, `
import { getDialogues } from 'lib/quests/dialogue_sound';

const dialogues = getDialogues(
${JSON.stringify({
  questName,
  dialogues: data
}, null, 2)});`)

async function setup() {
  const voiceMap = new Map<VoiceName, Voice>();
  const voices = (await client.voices.getAll()).voices.filter(v => v.category === "cloned")
  Object.keys(VoiceDescriptions).forEach(name => {
    voices.forEach(voice => {
      if (voice.name === name) {
        voiceMap.set(name as VoiceName, voice)
      }
    });
  })

  const user = await client.user.getSubscription()
  return {
    voiceMap,
    voiceLimit: user.voice_limit
  }
}

async function replaceVoice(name: VoiceName, description: string, filePath: string): Promise<Voice> {
  const voices = (await client.voices.getAll()).voices.filter(v => v.category === "cloned");
  if (voices.length >= voiceLimit) {
    console.log("Found existing temporary API voice", voices[0].name, " - deleting it to make space.")
    voiceMap.delete(voices[0].name as VoiceName);
    await client.voices.delete(voices[0].voice_id)
  }
  console.log("Creating voice", {name, description, filePath});
  const { voice_id } = await client.voices.add({
    name,
    description,
    files: [fs.createReadStream(filePath)],
  })
  const voice = await client.voices.get(voice_id)
  voiceMap.set(name, voice);
  return voice;
}

async function generateDialogue(voice: Voice, previousText: string, text: string, outputPath: string, previousReqId?: string) {
    const resp = await client.generate({
    voice: voice.name,
    text,
    previous_text: previousText.length > 0 ? previousText : undefined,
    previous_request_ids: previousReqId ? [previousReqId] : undefined,
    voice_settings: {
      stability: 0.1,
      similarity_boost: 1,
      use_speaker_boost: true,
    },
    enable_logging: true,
    output_format: "mp3_44100_128",
    model_id: "eleven_multilingual_v2",
  })

  const writeStream = fs.createWriteStream(outputPath);
  resp.pipe(writeStream);
  await new Promise((resolve, reject) => {
      writeStream.on('finish', resolve);
      writeStream.on('error', reject);
  });
  return (await client.history.getAll({ voice_id: voice.voice_id })).history[0].request_id
}

async function generate(voiceName: VoiceName, previousText: string, text: string, outputFileName: string, previousReqId?: string) {
  try {
    const samplePath = "./output/voice-packs/"+voiceName+".mp3"
    const voice = voiceMap.get(voiceName) ?? await replaceVoice(voiceName, VoiceDescriptions[voiceName], samplePath)

    if (!voice) {
      throw new Error("Cannot reuse existing voice nor create new voice!");
    }

    const requestId = await generateDialogue(voice, previousText, text, outputFileName, previousReqId)
    return requestId
  } catch (e: any) {
    console.log(e)
    console.log(e.statusCode)
    console.log(e.body.readableStream as ReadableStream);
    console.log(streamToString(e.body.readableStream as ReadableStream));
  }
}

async function streamToString(stream: ReadableStream): Promise<string> {
  return new Response(stream.pipeThrough(new TextDecoderStream())).text();
}

function parse(rawStr: string) {
  let s0: string, s1: string, s2: string, s3: string
  [s0, s1, s2, s3] = rawStr.trim().split("\n")
    .map(l => l.trim())
    .filter(l => l.length > 0)

  const reqId = s0.split(" ")[1];
  const voiceName = s1.split(": ")[0];
  const speaker = s1.split(": ")[1] ?? voiceName;
  const prevText = s2
  const text = s3

  return {
    reqId: reqId?.length > 0 ? reqId : undefined,
    voiceName: voiceName as VoiceName,
    speaker,
    prevText,
    text,
  }
}

function serialize({
  reqId,
  voiceName,
  speaker,
  prevText,
  text,
}) {
  return (
`// ${reqId}
${voiceName}${voiceName !== speaker ? `: ${speaker}` : ""}
${prevText}
${text}
`)
}