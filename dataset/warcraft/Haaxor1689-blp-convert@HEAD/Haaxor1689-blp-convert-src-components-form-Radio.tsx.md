---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/form/Radio.tsx"
ref: HEAD
---

import { type ReactNode } from 'react';
import cls from 'classnames';

import TextButton from '..//TextButton.tsx';

const isDeepEqual = (obj1: unknown, obj2: unknown): boolean => {
	if (obj1 === obj2) return true;
	// eslint-disable-next-line eqeqeq
	if (obj1 == null || obj2 == null) return false;
	if (typeof obj1 !== 'object' || typeof obj2 !== 'object') return false;
	const keys1 = Object.keys(obj1);
	const keys2 = Object.keys(obj2);
	if (keys1.length !== keys2.length) return false;
	for (const key of keys1)
		if (
			!keys2.includes(key) ||
			!isDeepEqual(obj1[key as never], obj2[key as never])
		)
			return false;
	return true;
};

const RadioIcon = () => (
	<svg
		width={16}
		height={16}
		viewBox="0 0 12 12"
		fill="none"
		xmlns="http://www.w3.org/2000/svg"
	>
		<circle cx="6" cy="6" r="5.5" stroke="currentColor" />
		<path
			d="M8 6C8 7.10429 7.10429 8 6 8C4.89571 8 4 7.10429 4 6C4 4.8957 4.89571 4 6 4C7.10429 4 8 4.8957 8 6Z"
			fill="white"
		/>
	</svg>
);
RadioIcon.$$typeof = Symbol.for('react.element');

type Props<T> = {
	value: T;
	setValue: (val: T) => void;
	options: { key: string; label: ReactNode; value: T; disabled?: boolean }[];
	disabled?: boolean;
	error?: boolean;
};

const Radio = <T,>({ value, setValue, options, disabled, error }: Props<T>) => (
	<div className="flex flex-wrap justify-start">
		{!options.length ? (
			<p className="text-blueGray p-2">No options</p>
		) : (
			options.map(o => (
				<TextButton
					key={o.key}
					onClick={() => setValue(o.value)}
					disabled={!!disabled || o.disabled}
					icon={RadioIcon}
					className={cls(
						'whitespace-nowrap',
						error && 'text-red!',
						!isDeepEqual(value, o.value)
							? 'text-blueGray **:fill-none'
							: 'text-white'
					)}
				>
					{o.label}
				</TextButton>
			))
		)}
	</div>
);

export default Radio;
