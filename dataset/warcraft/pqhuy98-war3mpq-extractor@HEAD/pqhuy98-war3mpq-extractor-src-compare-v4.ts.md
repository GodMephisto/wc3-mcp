---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "src/compare-v4.ts"
ref: HEAD
---

import { ElevenLabsClient } from "elevenlabs";
import "dotenv/config";
import fs from "fs";
import path from "path";
import { Voice } from "elevenlabs/api";
import assert from "assert";

const MODEL_ID = "eleven_v4";
const OLD_ROOT = "output/generated-voices";
const NEW_ROOT = "output/generated-voices-v4";
const VOICE_PACK_DIR = "output/voice-packs";

const VoiceDescriptions = {
  Footman: "Voice of a middle age American footman.",
  Knight: "Middle age warrior man. Strong and aggressive voice.",
  Rifleman: "Old dwarf rifleman.",
  Grunt: "Voice of a Orc warrior, highly aggressive.",
  MortarTeam: "Voice of an old dwarf man.",
  Peasant: "Voice of a peasant, weak mind and coward.",
  VillagerMan: "Voice of a male villager, weak mind and coward.",
  VillagerMan2: "Voice of a male villager, weak mind and coward.",
  VillagerWoman: "Voice of a male villager, soft and feminine.",
  VillagerKid: "Voice of 4 years old child.",
  Paladin: "Voice of a Paladin.",
  Archmage: "Voice of an old Archmage.",
  Bandit: "Voice of young male Bandit, rogue in nature.",
} as const;

type VoiceName = keyof typeof VoiceDescriptions;
const allVoiceNames = Object.keys(VoiceDescriptions) as VoiceName[];

const QUESTS: { name: string; maxLines?: number }[] = [
  { name: "starting" },
  { name: "test" },
  { name: "bandit-banish" },
  { name: "shadowfang-gate" },
  { name: "undead-invasion", maxLines: 20 },
];

type ParsedLine = {
  voiceName: VoiceName;
  speaker: string;
  prevText: string;
  text: string;
  oldFileName: string;
};

const client = new ElevenLabsClient({
  apiKey: process.env.ELEVENLABS_API_KEY,
});

const voiceMap = new Map<VoiceName, Voice>();
let voiceLimit = 0;

const { voiceLimit: limit } = await setup();
voiceLimit = limit;
console.log("Voice limit:", voiceLimit);
console.log("Existing cloned voices:", [...voiceMap.keys()]);

const compareRows: {
  quest: string;
  index: number;
  speaker: string;
  voiceName: string;
  oldText: string;
  v4Text: string;
  oldSrc: string;
  newSrc: string;
}[] = [];

for (const quest of QUESTS) {
  const lines = loadQuest(quest.name).slice(0, quest.maxLines);
  console.log(`\n=== ${quest.name}: ${lines.length} lines ===`);
  fs.mkdirSync(path.join(NEW_ROOT, quest.name), { recursive: true });

  let prevReqId: string | undefined;
  let i = 0;
  for (const line of lines) {
    const fileName = line.oldFileName;
    const outputPath = path.join(NEW_ROOT, quest.name, fileName);
    const v4Text = toV4SpokenText(line.prevText, line.text);
    console.log(`[${quest.name} ${i}] ${line.voiceName}: ${v4Text.slice(0, 80)}`);

    if (!fs.existsSync(outputPath)) {
      const reqId = await generate(line.voiceName, line.prevText, v4Text, outputPath, prevReqId);
      prevReqId = reqId;
    } else {
      console.log("  skip existing", outputPath);
    }

    compareRows.push({
      quest: quest.name,
      index: i,
      speaker: line.speaker,
      voiceName: line.voiceName,
      oldText: line.text,
      v4Text,
      oldSrc: `../generated-voices/${quest.name}/${fileName}`,
      newSrc: `./${quest.name}/${fileName}`,
    });
    i++;
  }
}

writeCompareHtml(compareRows);
console.log(`\nCompare page: ${NEW_ROOT}/compare.html`);
process.exit(0);

async function setup() {
  const voices = (await client.voices.getAll()).voices.filter((v) => v.category === "cloned");
  for (const name of allVoiceNames) {
    for (const voice of voices) {
      if (voice.name === name) {
        voiceMap.set(name, voice);
      }
    }
  }
  const user = await client.user.getSubscription();
  return { voiceLimit: user.voice_limit };
}

async function replaceVoice(name: VoiceName, description: string, filePath: string): Promise<Voice> {
  const voices = (await client.voices.getAll()).voices.filter((v) => v.category === "cloned");
  if (voices.length >= voiceLimit) {
    const evict = voices[0];
    console.log("Voice limit reached, deleting", evict.name);
    voiceMap.delete(evict.name as VoiceName);
    await client.voices.delete(evict.voice_id);
  }
  console.log("Creating voice", name, filePath);
  const { voice_id } = await client.voices.add({
    name,
    description,
    files: [fs.createReadStream(filePath)],
  });
  const voice = await client.voices.get(voice_id);
  voiceMap.set(name, voice);
  return voice;
}

async function generate(
  voiceName: VoiceName,
  previousText: string,
  text: string,
  outputPath: string,
  previousReqId?: string,
): Promise<string | undefined> {
  const samplePath = path.join(VOICE_PACK_DIR, `${voiceName}.mp3`);
  const voice = voiceMap.get(voiceName) ?? await replaceVoice(voiceName, VoiceDescriptions[voiceName], samplePath);
  const resp = await client.generate({
    voice: voice.name,
    text,
    previous_text: previousText.length > 0 ? previousText : undefined,
    previous_request_ids: previousReqId ? [previousReqId] : undefined,
    voice_settings: {
      stability: 0.1,
      similarity_boost: 1,
    },
    enable_logging: true,
    output_format: "mp3_44100_128",
    model_id: MODEL_ID,
  });

  const writeStream = fs.createWriteStream(outputPath);
  resp.pipe(writeStream);
  await new Promise<void>((resolve, reject) => {
    writeStream.on("finish", () => resolve());
    writeStream.on("error", reject);
  });

  const history = await client.history.getAll({ voice_id: voice.voice_id });
  return history.history[0]?.request_id;
}

function loadQuest(questName: string): ParsedLine[] {
  const inputPath = path.join(OLD_ROOT, questName, "input.txt");
  const raw = fs.readFileSync(inputPath).toString();
  const dialogues = raw
    .split("\n\n")
    .filter((l) => l.trim().length > 0)
    .map((l) => l.split("\n").filter((l2) => l2.length > 0).join("\n"));

  return dialogues.map((dialogue, i) => {
    const parsed = parse(dialogue);
    assert.ok(allVoiceNames.includes(parsed.voiceName), `unexpected voice ${parsed.voiceName}`);
    const oldFileName = `${questName}-${i}-${parsed.voiceName.toLowerCase()}.mp3`;
    const oldPath = path.join(OLD_ROOT, questName, oldFileName);
    assert.ok(fs.existsSync(oldPath), `missing old mp3 ${oldPath}`);
    return { ...parsed, oldFileName };
  });
}

function parse(rawStr: string): Omit<ParsedLine, "oldFileName"> {
  const lines = rawStr.trim().split("\n").map((l) => l.trim()).filter((l) => l.length > 0);
  assert.ok(lines.length === 4, `Must have 4 lines, found ${lines.length}. Raw: ${rawStr}`);
  const [, s1, s2, s3] = lines;
  const voiceName = s1.split(": ")[0] as VoiceName;
  const speaker = s1.split(": ")[1] ?? voiceName;
  return {
    voiceName,
    speaker,
    prevText: s2,
    text: s3,
  };
}

function toV4SpokenText(prevText: string, text: string): string {
  let spoken = text.replace(/<([^>\n]+)>/g, "[$1]").trim();
  if (spoken.startsWith("[")) return spoken;
  const star = prevText.match(/\*([^*]+)\*/);
  if (star) {
    const dir = star[1].replace(/^:\s*/, "").trim();
    if (dir.length > 0 && dir.length < 90) spoken = `[${dir}] ${spoken}`;
  }
  return spoken;
}

function writeCompareHtml(rows: typeof compareRows) {
  fs.mkdirSync(NEW_ROOT, { recursive: true });
  const groups = new Map<string, typeof rows>();
  for (const row of rows) {
    const list = groups.get(row.quest) ?? [];
    list.push(row);
    groups.set(row.quest, list);
  }

  const sections = [...groups.entries()].map(([quest, questRows]) => {
    const cards = questRows.map((row) => `
      <article class="card">
        <header>
          <strong>${escapeHtml(row.speaker)}</strong>
          <span>${escapeHtml(row.voiceName)} #${row.index}</span>
        </header>
        <p class="line">${escapeHtml(row.oldText)}</p>
        ${row.v4Text !== row.oldText ? `<p class="v4">v4 prompt: ${escapeHtml(row.v4Text)}</p>` : ""}
        <div class="pair">
          <label>multilingual v2 (2024)<audio controls src="${row.oldSrc}"></audio></label>
          <label>eleven v4<audio controls src="${row.newSrc}"></audio></label>
        </div>
      </article>`).join("\n");
    return `<section><h2>${escapeHtml(quest)}</h2>${cards}</section>`;
  }).join("\n");

  fs.writeFileSync(path.join(NEW_ROOT, "compare.html"), `<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>ElevenLabs v2 vs v4</title>
  <style>
    body { font-family: sans-serif; margin: 24px; background: #111; color: #eee; }
    h1 { font-size: 20px; }
    h2 { margin-top: 32px; border-bottom: 1px solid #444; padding-bottom: 8px; }
    .card { background: #1c1c1c; border: 1px solid #333; border-radius: 8px; padding: 12px 16px; margin: 12px 0; }
    header { display: flex; justify-content: space-between; color: #bbb; margin-bottom: 8px; }
    .line, .v4 { white-space: pre-wrap; }
    .v4 { color: #9cf; font-size: 13px; }
    .pair { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
    label { display: flex; flex-direction: column; gap: 6px; font-size: 12px; color: #aaa; }
    audio { width: 100%; }
  </style>
</head>
<body>
  <h1>ElevenLabs multilingual v2 vs eleven v4</h1>
  <p>Old files untouched in output/generated-voices. New files in output/generated-voices-v4.</p>
  ${sections}
</body>
</html>
`);
}

function escapeHtml(s: string): string {
  return s
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");
}
