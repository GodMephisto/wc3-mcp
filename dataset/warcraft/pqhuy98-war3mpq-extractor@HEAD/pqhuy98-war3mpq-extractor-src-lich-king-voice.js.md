---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "src/lich-king-voice.js"
ref: HEAD
---

import ffmpeg from 'fluent-ffmpeg';
import fs from 'fs';
import path from 'path';

// Ensure ffmpeg binaries are found (Windows paths in this repo)
ffmpeg.setFfmpegPath('bin\\ffmpeg-7.0.2-essentials_build\\bin\\ffmpeg.exe');
ffmpeg.setFfprobePath('bin\\ffmpeg-7.0.2-essentials_build\\bin\\ffprobe.exe');

// Parse CLI args for iterative tweaking and sweeps
function parseArgs(argv) {
	const out = { _: [] };
	for (let i = 2; i < argv.length; i++) {
		const a = argv[i];
		if (a.startsWith('--')) {
			const [keyRaw, valRaw] = a.replace(/^--/, '').split('=');
			const key = keyRaw.trim();
			if (valRaw !== undefined) {
				out[key] = valRaw;
			} else {
				// next token as value if not another flag, else boolean true
				const next = argv[i + 1];
				if (!next || next.startsWith('--')) {
					out[key] = 'true';
				} else {
					out[key] = next;
					i++;
				}
			}
		} else {
			out._.push(a);
		}
	}
	return out;
}

const args = parseArgs(process.argv);

// Input can be provided as --in <path> or first non-flag arg; defaults to provided test file
const inputPath = args.in ?? args._[0] ?? 'manual-data\\test-lk.mp3';

// Build output path next to input with a suffix
const parsed = path.parse(inputPath);
const outputPath = path.join(parsed.dir, `${parsed.name}_lich_king.mp3`);

function ensureParentDir(filePath) {
	const dir = path.dirname(filePath);
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir, { recursive: true });
	}
}

function fileExists(filePath) {
	try {
		fs.accessSync(filePath, fs.constants.R_OK);
		return true;
	} catch {
		return false;
	}
}

if (!fileExists(inputPath)) {
	console.error(`Input file not found: ${inputPath}`);
	process.exit(1);
}

ensureParentDir(outputPath);

// Complex filtergraph approximating the YouTube/Audacity chain in FFmpeg.
// Steps:
// 1) VO chain: noise reduction, EQ (bass/treble boost), gentle compression
// 2) Split into 3 layers:
//    - main: subtle echo, center
//    - left: slight pitch down + pre-delay + multi-tap echo (reverb-ish), panned left
//    - right: reverse -> echo chain (pre-verb) -> reverse back, panned right
// 3) Mix and limit to -3 dB peak
function num(value, fallback) {
	const n = Number(value);
	return Number.isFinite(n) ? n : fallback;
}

// Build a filtergraph from param set
function buildGraph(p) {
	const eqBassG = num(p.eqBassGain, 6);
	const eqTrebleG = num(p.eqTrebleGain, 3);
	const compThr = p.compThreshold ?? '-14dB';
	const compRatio = num(p.compRatio, 2);
	const compAttack = num(p.compAttack, 200);
	const compRelease = num(p.compRelease, 1000);
	const compMakeup = num(p.compMakeup, 2);
	const limiterAmp = num(p.limiterAmp, 0.707);

	const globalPitch = p.globalPitchFactor ? Number(p.globalPitchFactor) : null; // e.g., 0.85
	const globalPitchChain = globalPitch && globalPitch > 0 && globalPitch !== 1
		? `asetrate=44100*${globalPitch},aresample=44100,atempo=${(1/globalPitch).toFixed(3)},`
		: '';

	const mainEchoDelay = num(p.mainEchoDelay, 400);
	const mainEchoDecay = num(p.mainEchoDecay, 0.05);
	const mainGain = num(p.mainGain, 0.7);

	const delays = p.multiEchoDelays ?? '60|120|200';
	const decays = p.multiEchoDecays ?? '0.5|0.35|0.25';
	const preDelay = num(p.preDelay, 100);

	const leftPitch = num(p.leftPitchFactor, 0.90);
	const leftTempo = Number((1 / leftPitch).toFixed(3));
	const leftVol = num(p.leftVol, 0.5);
	const leftPanL = num(p.leftPanL, 0.95);
	const leftPanR = num(p.leftPanR, 0.35);

	const rightVol = num(p.rightVol, 0.5);
	const rightPanL = num(p.rightPanL, 0.35);
	const rightPanR = num(p.rightPanR, 0.95);

	return [
		// Base VO chain (all in one chain, then split)
		`[0:a]afftdn=nr=18,${globalPitchChain}` +
		`equalizer=f=110:t=q:w=1.0:g=${eqBassG},` +
		`equalizer=f=4000:t=q:w=1.0:g=${eqTrebleG},` +
		`acompressor=threshold=${compThr}:ratio=${compRatio}:attack=${compAttack}:release=${compRelease}:makeup=${compMakeup},` +
		`aresample=44100,asplit=3[main][l][r];` +

		// Main center layer
		`[main]aecho=0.8:0.9:${mainEchoDelay}:${mainEchoDecay},` +
		`pan=stereo|c0=${mainGain}*c0|c1=${mainGain}*c0[mainst];` +

		// Left
		`[l]asetrate=44100*${leftPitch.toFixed(2)},aresample=44100,atempo=${leftTempo},` +
		`adelay=${preDelay},aecho=0.7:0.9:${delays}:${decays},` +
		`pan=stereo|c0=${leftPanL}*c0|c1=${leftPanR}*c0,volume=${leftVol}[leftst];` +

		// Right
		`[r]areverse,adelay=${preDelay},aecho=0.7:0.9:${delays}:${decays},areverse,` +
		`pan=stereo|c0=${rightPanL}*c0|c1=${rightPanR}*c0,volume=${rightVol}[rightst];` +

		// Mix + limit
		`[mainst][leftst][rightst]amix=inputs=3:normalize=0,alimiter=limit=${limiterAmp}[outa]`
	].join('');
}

function renderOnce(params, baseOutputPath, suffix, seekSeconds, quickSeconds) {
	return new Promise((resolve, reject) => {
		const graph = buildGraph(params);
		const outParsed = path.parse(baseOutputPath);
		const outPath = suffix
			? path.join(outParsed.dir, `${outParsed.name}${suffix}${outParsed.ext}`)
			: baseOutputPath;

		console.log('Applying Lich King voice effect...');
		console.log(`  In : ${inputPath}`);
		console.log(`  Out: ${outPath}`);

		let cmd = ffmpeg().input(inputPath).complexFilter(graph, 'outa')
			.audioCodec('libmp3lame')
			.audioBitrate('192k')
			.outputOptions([
				'-ar', '44100',
				'-ac', '2',
				'-map_metadata', '-1'
			])
			.on('start', c => console.log(`ffmpeg: ${c}`))
			.on('progress', p => { if (p.percent !== undefined) process.stdout.write(`\rProgress: ${p.percent.toFixed(1)}%   `); })
			.on('error', err => { console.error(`\nError: ${err.message}`); reject(err); })
			.on('end', () => { console.log('\nDone.'); resolve(outPath); })
			.save(outPath);

		if (seekSeconds) cmd = cmd.seekInput(seekSeconds);
		if (quickSeconds) cmd = cmd.duration(quickSeconds);
	});
}

async function main() {
	const seekSeconds = args.ss ? Number(args.ss) : undefined; // --ss 1.5
	const quickSeconds = args.quick ? Number(args.quick) : undefined; // --quick 5

	// Base params from flags (optional)
	const baseParams = {
		globalPitchFactor: args.globalPitch,
		mainEchoDelay: args.mainEchoDelay,
		mainEchoDecay: args.mainEchoDecay,
		mainGain: args.mainGain,
		multiEchoDelays: args.delays,
		multiEchoDecays: args.decays,
		preDelay: args.preDelay,
		leftPitchFactor: args.pitch,
		leftVol: args.leftVol,
		rightVol: args.rightVol,
		leftPanL: args.leftPanL,
		leftPanR: args.leftPanR,
		rightPanL: args.rightPanL,
		rightPanR: args.rightPanR,
		eqBassGain: args.eqBassGain,
		eqTrebleGain: args.eqTrebleGain,
		compThreshold: args.compThreshold,
		compRatio: args.compRatio,
		compAttack: args.compAttack,
		compRelease: args.compRelease,
		compMakeup: args.compMakeup,
		limiterAmp: args.limiterAmp
	};

	// Sweep mode: --sweep param:val1,val2,val3
	const sweep = args.sweep;
	if (sweep) {
		const [param, valuesStr] = String(sweep).split(':');
		if (!param || !valuesStr) {
			console.error('Invalid --sweep format. Use --sweep param:val1,val2,val3');
			process.exit(1);
		}
		const values = valuesStr.split(',').map(v => v.trim()).filter(Boolean);
		for (const v of values) {
			const params = { ...baseParams, [param]: v };
			const suffix = `_${param}-${v.replace(/[^0-9A-Za-z._-]/g,'')}`;
			try {
				await renderOnce(params, outputPath, suffix, seekSeconds, quickSeconds);
			} catch (e) {
				process.exit(1);
			}
		}
		return;
	}

	// Single run
	try {
		await renderOnce(baseParams, outputPath, '', seekSeconds, quickSeconds);
	} catch (e) {
		process.exit(1);
	}
}

await main();


