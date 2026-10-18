---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/utils/useCanvasImage.tsx"
ref: HEAD
---

/* eslint-disable react-hooks/rules-of-hooks */
/* eslint-disable react-hooks/exhaustive-deps */
import { type DependencyList, useEffect, useRef, useState } from 'react';
import cls from 'classnames';

import { useKeyPressed } from '../components/KeyboardListener.tsx';

export const drawImageData = (
	ctx: CanvasRenderingContext2D,
	data: ImageData | HTMLImageElement,
	x: number,
	y: number,
	width: number = data.width,
	height: number = data.height
) => {
	ctx.save();

	// Set scale
	const scaleX = width / data.width;
	const scaleY = height / data.height;
	ctx.scale(scaleX, scaleY);

	const scaledX = x / scaleX;
	const scaledY = y / scaleY;

	if (data instanceof HTMLImageElement) {
		ctx.drawImage(data, scaledX, scaledY);
	} else {
		// Create a temporary canvas
		const tempCanvas = document.createElement('canvas');
		tempCanvas.width = data.width;
		tempCanvas.height = data.height;

		// Put the image data onto the temporary canvas
		const tempCtx = tempCanvas.getContext('2d');
		tempCtx?.putImageData(data, 0, 0);

		// Draw the temporary canvas onto the original canvas
		ctx.drawImage(tempCanvas, scaledX, scaledY);
	}

	// Reset scale
	ctx.restore();
};

export const loadRawImage = (src: string) =>
	new Promise<HTMLImageElement>(res => {
		const img = new Image();
		img.addEventListener('load', () => res(img));
		img.src = `local-raw:///${src}`;
	});

export type CanvasLayers = [LayerImage, DependencyList?][];
export type GetCanvasData = (
	layer?: number | 'merged',
	rect?: readonly [number, number, number, number]
) => ImageData | null;

export type SourceImage =
	| {
			buffer: Uint8Array;
			width: number;
			height: number;
	  }
	| string
	| null;

type LayerImage =
	| {
			x?: number;
			y?: number;
			width?: number;
			height?: number;
			source: SourceImage;
	  }
	| ((ctx: CanvasRenderingContext2D) => Promise<void> | void);

type SourceLayer = {
	width: number;
	height: number;
	content?: ImageData | HTMLImageElement;
};

type OtherLayer =
	| {
			x?: number;
			y?: number;
			width?: number;
			height?: number;
			content: ImageData | HTMLImageElement;
	  }
	| ((ctx: CanvasRenderingContext2D) => Promise<void> | void)
	| null;

// TODO: Replace with local image fetching instead of buffer where possible
const useCanvasImage = (
	image: SourceImage | { width: number; height: number },
	options?: {
		dependencies?: DependencyList;
		hideTransparency?: boolean;
		layers?: CanvasLayers;
		additionalDraw?: [(ctx: CanvasRenderingContext2D) => void, DependencyList];
	}
) => {
	const rootRef = useRef<HTMLDivElement>(null);
	const canvasRefs = useRef<(HTMLCanvasElement | null)[]>([]);

	const [scale, setScale] = useState(1);

	const space = useKeyPressed(' ');
	const control = useKeyPressed('Control');

	const [translate, setTranslate] = useState({ x: 0, y: 0 });

	const [source, setSource] = useState<SourceLayer>();
	const [otherSources, setOtherSources] = useState<OtherLayer[]>();

	useEffect(() => {
		// Change scale so that the image fits in the root element
		if (!rootRef.current?.parentElement || !source) return;
		const { width, height } =
			rootRef.current.parentElement.getBoundingClientRect();
		const scaleX = width / source.width;
		const scaleY = height / source.height;
		setScale(Math.min(scaleX, scaleY));
	}, [source]);

	// Create source layer
	useEffect(() => {
		(async () => {
			if (!image) return;
			if (typeof image === 'string') {
				const img = await loadRawImage(image);
				setSource({
					width: img.width,
					height: img.height,
					content: img
				});
			} else {
				setSource({
					width: image.width,
					height: image.height,
					content:
						'buffer' in image
							? new ImageData(
									new Uint8ClampedArray(image.buffer),
									image.width,
									image.height
								)
							: undefined
				});
			}
		})();
	}, [image, ...(options?.dependencies ?? [])]);

	// Draw source layer
	useEffect(() => {
		if (!source) return;
		const ctx = canvasRefs.current[0]?.getContext('2d');
		if (!ctx) return;
		ctx.clearRect(0, 0, source.width, source.height);
		if (source.content) drawImageData(ctx, source.content, 0, 0);
		options?.additionalDraw?.[0](ctx);
	}, [source, ...(options?.additionalDraw?.[1] ?? [])]);

	for (let i = 0; i < (options?.layers?.length ?? 0); i++) {
		// Create other layers
		useEffect(() => {
			(async () => {
				if (!source) return;
				const layer = options?.layers?.[i]?.[0];

				let src: OtherLayer = null;
				if (typeof layer === 'function') {
					src = layer;
				} else if (!layer?.source) {
					src = null;
				} else if (typeof layer.source === 'string') {
					const img = await loadRawImage(layer.source);
					src = {
						x: layer.x,
						y: layer.y,
						width: layer.width,
						height: layer.height,
						content: img
					};
				} else {
					src = {
						x: layer.x,
						y: layer.y,
						width: layer.width,
						height: layer.height,
						content: new ImageData(
							new Uint8ClampedArray(layer.source.buffer),
							layer.source.width,
							layer.source.height
						)
					};
				}
				setOtherSources(s => (s ?? []).toSpliced(i, 1, src));
			})();
		}, [source, ...(options?.layers?.[i]?.[1] ?? [])]);

		// Draw other layers
		useEffect(() => {
			(async () => {
				if (!source) return;

				const ctx = canvasRefs.current[i + 1]?.getContext('2d');
				if (!ctx) return;

				ctx.clearRect(0, 0, source.width, source.height);
				const layer = otherSources?.[i];
				if (!layer) return;

				if (typeof layer === 'function') {
					await layer(ctx);
					return;
				}

				drawImageData(
					ctx,
					layer.content,
					layer.x ?? 0,
					layer.y ?? 0,
					layer.width,
					layer.height
				);
			})();
		}, [otherSources?.[i]]);
	}

	useEffect(() => {
		const moveMouseCb = (e: MouseEvent) => {
			if (!space || (e.buttons & 1) !== 1) return;
			e.preventDefault();
			setTranslate(prev => ({
				x: prev.x + e.movementX * (1 / scale),
				y: prev.y + e.movementY * (1 / scale)
			}));
		};
		const wheelCb = (e: WheelEvent) => {
			if (
				!e.ctrlKey ||
				(e.target !== canvasRefs.current[0] &&
					e.target !== canvasRefs.current[0]?.parentElement)
			)
				return;

			// TODO: Preserve scale and translate when changing tab
			setScale(prev => Math.max(prev * (e.deltaY > 0 ? 0.95 : 1.05), 0.05));
		};

		window.addEventListener('mousemove', moveMouseCb);
		window.addEventListener('wheel', wheelCb);
		return () => {
			window.removeEventListener('mousemove', moveMouseCb);
			window.removeEventListener('wheel', wheelCb);
		};
	}, [scale, space]);

	const getCanvasData: GetCanvasData = (
		layer = 0,
		rect = [0, 0, source?.width ?? 0, source?.height ?? 0]
	) => {
		if (layer === 'merged') {
			// create new canvas and paint all layers on it

			const canvas = document.createElement('canvas');
			canvas.width = rect[2];
			canvas.height = rect[3];
			const ctx = canvas.getContext('2d');
			if (!ctx) return null;

			for (const ref of canvasRefs.current) {
				if (!ref) continue;
				ctx.drawImage(ref, 0, 0);
			}
			return ctx.getImageData(0, 0, rect[2], rect[3]);
		}

		const ctx = canvasRefs.current?.[layer]?.getContext('2d');
		if (!ctx) return null;
		return ctx.getImageData(...rect);
	};

	return {
		props: {
			ref: rootRef,
			className: cls('relative shrink-0', {
				'cursor-grab': space,
				'cursor-zoom-in': control,
				'bg-checkers': !options?.hideTransparency
			}),
			style: {
				width: source?.width,
				height: source?.height,
				backgroundSize: `${(1 / scale) * 20}px ${(1 / scale) * 20}px`,
				transform: `scale(${scale}) translate(${translate.x}px, ${translate.y}px)`
			} as const,
			children: [source, ...(otherSources ?? [])].map((_, i) => (
				<canvas
					key={i}
					ref={el => (canvasRefs.current[i] = el)}
					width={source?.width}
					height={source?.height}
					className="pointer-events-none absolute top-0 left-0"
					style={{ imageRendering: 'pixelated' }}
				/>
			))
		},
		// drawExtra,
		getCanvasData,
		scale
	};
};

export default useCanvasImage;
