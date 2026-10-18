---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/Blp.tsx"
ref: HEAD
---

import { useEffect, useState } from 'react';
import { Blp } from 'haax-blp';
import { Jimp, type JimpInstance } from 'jimp';
import { Image, RotateCcw, X } from 'lucide-react';

import BlpForm, { downloadBlob } from './BlpForm.tsx';
import BlpInfo from './BlpInfo.tsx';
import FileUpload from './components/form/FileUpload.tsx';
import Spinner from './components/Spinner.tsx';
import TextButton from './components/TextButton.tsx';
import useCanvasImage from './utils/useCanvasImage.tsx';

const FileEditorBlp = () => {
	const [file, setFile] = useState<File | null>(null);

	const [info, setInfo] = useState<Blp>();
	const [error, setError] = useState<Error>();
	const [loading, setLoading] = useState(false);

	const reset = () => {
		setFile(null);
		setInfo(undefined);
		setError(undefined);
		setLoading(false);
	};

	useEffect(() => {
		if (!file) {
			reset();
			return;
		}

		const load = async () => {
			const arrayBuffer = await file.arrayBuffer();

			if (file.name.toLowerCase().endsWith('.png')) {
				const image = await Jimp.read(arrayBuffer);
				return {
					signature: 'BLP2',
					version: 1,
					format: 'COLOR_DXT',
					alphaSize: 8,
					compression: 'PIXEL_DXT5',
					mipMaps: 'MIPS_NONE',

					width: image.width,
					height: image.height,

					mips: [
						{
							buffer: new Uint8Array(image.bitmap.data),
							width: image.width,
							height: image.height
						}
					]
				} satisfies Blp;
			}

			return Blp.fromBuffer(new Uint8Array(arrayBuffer));
		};

		setLoading(true);
		load()
			.then(setInfo)
			.catch(setError)
			.finally(() => setLoading(false));
	}, [file]);

	const [index, setIndex] = useState(0);
	const image = info?.mips[index];

	const { props: canvasProps } = useCanvasImage(image ?? null);

	return (
		<>
			<div className="flex justify-between gap-2 pt-2">
				<div className="flex items-center gap-2 p-2">
					<img
						src="/icon.png"
						alt="Blp convert icon"
						className="h-8 shrink-0"
					/>
					<h3 className="tw-color overflow-hidden text-ellipsis whitespace-nowrap">
						{file?.name ?? 'Blp convert'}
					</h3>
				</div>
				{file?.name && info && (
					<div className="flex shrink-0 gap-2">
						<TextButton
							icon={Image}
							iconSize={20}
							onClick={async () => {
								const mip = info.mips[0];
								if (!mip) return;
								const image: JimpInstance = await Jimp.fromBitmap({
									data: mip.buffer,
									width: mip.width,
									height: mip.height
								});

								const buffer = await image.getBuffer('image/png');
								const blob = new Blob([new Uint8Array(buffer)], {
									type: 'image/png'
								});
								downloadBlob(blob, file.name.replace(/\.blp$/i, '.png'));
							}}
						>
							To PNG
						</TextButton>
						<BlpForm name={file?.name} info={info} />
						<TextButton
							icon={X}
							iconSize={20}
							onClick={() => setFile(null)}
							className="text-red"
						>
							Close
						</TextButton>
					</div>
				)}
			</div>

			{info && <BlpInfo index={index} setIndex={setIndex} info={info} />}

			<div className="flex min-h-0 grow flex-col items-center justify-center overflow-hidden">
				{loading ? (
					<Spinner />
				) : error ? (
					<div className="tw-surface-3 flex flex-col items-end gap-2">
						<p className="max-w-3xl text-center text-xl">
							Error:{' '}
							<span className="text-blue-gray">
								{error.message ?? 'Failed to load the texture'}
							</span>
						</p>
						<TextButton icon={RotateCcw} onClick={reset} className="-mb-2">
							Reload
						</TextButton>
					</div>
				) : !info ? (
					<FileUpload
						key={file?.name ?? '_empty_'}
						onChange={setFile}
						accept=".blp,.png"
					/>
				) : (
					<div {...canvasProps} />
				)}
			</div>
		</>
	);
};

export default FileEditorBlp;
