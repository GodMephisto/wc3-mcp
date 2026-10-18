---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/form/Checkbox.tsx"
ref: HEAD
---

import { type ReactNode } from 'react';
import cls from 'classnames';

import TextButton from '../TextButton.tsx';

const CheckboxIcon = () => (
	<svg
		width={16}
		height={16}
		viewBox="0 0 12 12"
		fill="none"
		xmlns="http://www.w3.org/2000/svg"
	>
		<rect x="1" y="1" width="10" height="10" rx="1" stroke="currentColor" />
		<rect x="3.5" y="3.5" width="5" height="5" fill="white" />
	</svg>
);
CheckboxIcon.$$typeof = Symbol.for('react.element');

type Props = {
	label?: ReactNode;
	value: boolean;
	disabled?: boolean;
	onChange: (v: boolean) => void;
	className?: cls.Value;
};

export const Checkbox = ({
	label,
	value,
	onChange,
	disabled,
	className
}: Props) => (
	<TextButton
		icon={CheckboxIcon}
		onClick={() => onChange(!value)}
		disabled={disabled}
		className={cls(
			'text-blueGray',
			{
				'**:fill-none': !value,
				'[&_rect:nth-child(2)]:fill-blueGray': disabled && value
			},
			className
		)}
	>
		{label}
	</TextButton>
);
