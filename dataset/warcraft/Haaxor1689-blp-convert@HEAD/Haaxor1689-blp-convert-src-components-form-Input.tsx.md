---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/form/Input.tsx"
ref: HEAD
---

import { type HTMLInputTypeAttribute, type ReactNode, useId } from 'react';
import cls from 'classnames';

export type InputProps = {
	type?: HTMLInputTypeAttribute;
	label?: string;
	required?: boolean;
	disabled?: boolean;
	error?: boolean;
	iconBefore?: ReactNode;
	iconAfter?: ReactNode;
	placeholder?: string;
	autoComplete?: 'off';
	className?: cls.Value;
};

type Props = InputProps & {
	value: string;
	onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
};

const Input = ({
	label,
	required,
	disabled,
	error,
	iconBefore,
	iconAfter,
	className,
	...props
}: Props) => {
	const id = useId();
	return (
		<>
			{label && (
				<label htmlFor={id}>
					{label}
					{required && <span className="text-red">*</span>}:
				</label>
			)}
			<div
				className={cls(
					className,
					'border-blue-gray bg-darker-gray text-blue-gray hocus:border-green relative flex cursor-text items-center gap-2 rounded-sm border-b p-2',
					{
						'border-blue-gray/50 pointer-events-none': disabled,
						'border-red bg-red/10': error
					}
				)}
			>
				{iconBefore}
				<input
					id={id}
					{...props}
					disabled={disabled}
					className="placeholder-blue-gray -m-2 grow cursor-[inherit] appearance-none bg-transparent p-2"
				/>
				{iconAfter}
			</div>
		</>
	);
};

export default Input;
