---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/BlpForm.tsx"
ref: HEAD
---

import { useState } from 'react';
import { Blp } from 'haax-blp';
import { Save } from 'lucide-react';

import Dialog, { DialogClose } from './components/Dialog.tsx';
import { Checkbox } from './components/form/Checkbox.tsx';
import Input from './components/form/Input.tsx';
import Radio from './components/form/Radio.tsx';
import { InfoItem } from './components/InfoPanelItems.tsx';
import TextButton from './components/TextButton.tsx';

const Compression = ['DXT1', 'DXT3', 'DXT5', 'Palette', 'None'] as const;
type Compression = (typeof Compression)[number];

export const toCompression = (info: Blp): Compression | null => {
	switch (info.format) {
		case 'COLOR_DXT': {
			switch (info.compression) {
				case 'PIXEL_DXT1':
					if (info.alphaSize > 1) return null;
					return 'DXT1';
				case 'PIXEL_DXT3':
					if (info.alphaSize > 4) return null;
					return 'DXT3';
				case 'PIXEL_DXT5':
					return 'DXT5';
			}
			return null;
		}
		case 'COLOR_PALETTE': {
			return 'Palette';
		}
		case 'COLOR_ARGB8888': {
			if (info.compression !== 'PIXEL_ARGB8888') return null;
			return 'None';
		}
	}
	return null;
};

const fromCompression = (
	compression: Compression,
	alphaSize: number
): Partial<Blp> => {
	switch (compression) {
		case 'DXT1':
			return {
				format: 'COLOR_DXT',
				compression: 'PIXEL_DXT1',
				alphaSize: Math.min(alphaSize, 1)
			};
		case 'DXT3':
			return {
				format: 'COLOR_DXT',
				compression: 'PIXEL_DXT3',
				alphaSize: Math.min(alphaSize, 4)
			};
		case 'DXT5':
			return {
				format: 'COLOR_DXT',
				compression: 'PIXEL_DXT5',
				alphaSize
			};
		case 'Palette':
			return {
				format: 'COLOR_PALETTE',
				compression: 'PIXEL_UNSPECIFIED',
				alphaSize
			};
		case 'None':
			return {
				format: 'COLOR_ARGB8888',
				compression: 'PIXEL_ARGB8888',
				alphaSize
			};
	}
};

export const downloadBlob = (blob: Blob, title: string) => {
	const url = window.URL.createObjectURL(blob);
	const link = document.createElement('a');

	link.href = url;
	link.download = title;

	link.click();
};

const isPowerOfTwo = (x: number) => {
	if (x <= 0) return false;
	const log2 = Math.log2(x);
	return Number.isInteger(log2);
};

type Props = { name: string; info: Blp };

const BlpForm = ({ name, info }: Props) => {
	const [loading, setLoading] = useState(false);

	const [fileName, setFileName] = useState(name.replace('.png', '.blp'));
	const [compression, setCompression] = useState(toCompression(info));
	const [alpha, setAlpha] = useState(info.alphaSize);
	const [mipMaps, setMipMaps] = useState(info.mipMaps === 'MIPS_GENERATED');

	const error =
		compression?.startsWith('DXT') &&
		(!isPowerOfTwo(info.width) || !isPowerOfTwo(info.height))
			? 'Width and Height must be power of two for DXT compression.'
			: !fileName
				? 'Please provide a file name.'
				: undefined;

	return (
		<Dialog
			title="Save as"
			description="Configure the BLP options before saving."
			trigger={
				<TextButton icon={Save} iconSize={20} type="asChild">
					To BLP
				</TextButton>
			}
			actions={[
				error ? (
					<span key="error" className="text-red text-sm">
						{error}
					</span>
				) : null,
				<DialogClose key="save">
					<TextButton
						icon={Save}
						iconSize={20}
						onClick={async () => {
							setLoading(true);
							try {
								const buffer = await Blp.toBuffer({
									...info,
									...fromCompression(compression!, alpha),
									mipMaps: mipMaps ? 'MIPS_GENERATED' : 'MIPS_NONE'
								});

								downloadBlob(
									new Blob([new Uint8Array(buffer)], {
										type: 'application/octet-stream'
									}),
									fileName
								);
							} finally {
								setLoading(false);
							}
						}}
						disabled={!compression || !!error}
						loading={loading}
					>
						Save
					</TextButton>
				</DialogClose>
			]}
		>
			<p className="pb-2">
				You can change these BLP compression options before saving:
			</p>
			<InfoItem title="Name">
				<Input
					value={fileName}
					onChange={e => setFileName(e.currentTarget.value)}
					error={!fileName}
					className="grow"
				/>
			</InfoItem>
			<InfoItem title="Compression">
				<Radio
					value={compression}
					setValue={v => {
						if (v === 'DXT1' && alpha > 1) setAlpha(1);
						if (v === 'DXT3' && alpha > 4) setAlpha(4);
						setCompression(v);
					}}
					options={Compression.map(c => ({
						key: c,
						label: c,
						value: c
					}))}
				/>
			</InfoItem>
			<InfoItem title="Alpha size">
				<Radio
					value={alpha}
					setValue={setAlpha}
					options={[0, 1, 2, 4, 8].map(a => ({
						key: a.toString(),
						label: a,
						value: a,
						disabled:
							(compression === 'DXT1' && a > 1) ||
							(compression === 'DXT3' && a > 4)
					}))}
				/>
			</InfoItem>
			<InfoItem title="Mip maps">
				<Checkbox value={mipMaps} onChange={setMipMaps} label="Generated" />
			</InfoItem>
		</Dialog>
	);
};

export default BlpForm;
