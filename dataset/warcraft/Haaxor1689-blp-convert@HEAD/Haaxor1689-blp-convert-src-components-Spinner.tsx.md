---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/Spinner.tsx"
ref: HEAD
---

import { type HTMLAttributes } from 'react';
import cls from 'classnames';

type Props = HTMLAttributes<HTMLDivElement> & { size?: number };

const Spinner = ({ size = 50, className, ...props }: Props) => (
	<div
		style={{ width: size, borderWidth: Math.ceil(size * 0.1) }}
		className={cls(
			`border-blue-gray inline-block aspect-square animate-spin rounded-full border border-t-[currentColor] opacity-75`,
			className
		)}
		{...props}
	/>
);

export default Spinner;
