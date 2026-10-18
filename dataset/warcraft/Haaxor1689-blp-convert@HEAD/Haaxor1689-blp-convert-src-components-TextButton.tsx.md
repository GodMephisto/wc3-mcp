---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/TextButton.tsx"
ref: HEAD
---

import {
	type FunctionComponent,
	type MouseEventHandler,
	type ReactNode
} from 'react';
import cls from 'classnames';
import { type LucideIcon } from 'lucide-react';

import Spinner from './Spinner.tsx';

export type IconType =
	| FunctionComponent<{ size?: number; className: string }>
	| LucideIcon;

type Props = {
	active?: boolean;
	loading?: boolean;
	disabled?: boolean;
	tabIndex?: number;
	iconSize?: number;
	className?: cls.Value;
} & (
	| { type: 'asChild' }
	| { type: 'submit'; form?: string }
	| {
			type?: never;
			onClick: MouseEventHandler<HTMLButtonElement>;
			onDoubleClick?: MouseEventHandler<HTMLButtonElement>;
			onContextMenu?: MouseEventHandler<HTMLButtonElement>;
	  }
	| {
			href: string;
			type: 'link';
			onClick?: MouseEventHandler<HTMLAnchorElement>;
			external?: boolean;
	  }
) &
	(
		| { children: ReactNode; icon?: IconType; title?: never }
		| { children?: never; icon: IconType; title: string }
	);

// TODO: Make server compatible by changing how Icon is passed
const TextButton = ({
	title,
	active,
	loading,
	disabled,
	tabIndex,
	icon: Icon,
	iconSize,
	className,
	children,
	...props
}: Props) => {
	const Component = props.type === 'link' ? 'a' : 'button';
	return (
		<Component
			{...props}
			title={title ?? (typeof children === 'string' ? children : undefined)}
			tabIndex={!!loading || !!disabled ? -1 : tabIndex}
			className={cls(
				'flex cursor-pointer items-center gap-1 border-0 p-2',
				className,
				{
					'text-warm-green': active && !loading && !disabled,
					'text-gray pointer-events-none': !!loading || !!disabled,
					'hocus:tw-highlight transition-all': !loading && !disabled
				}
			)}
			{...((props.type === 'link'
				? {
						href: props.href,
						onClick: props.onClick,
						target: props.external ? '_blank' : undefined,
						rel: props.external ? 'noopener noreferrer' : undefined
					}
				: props.type === 'submit'
					? { form: props.form }
					: props.type === 'asChild'
						? { type: 'button' }
						: {
								type: 'button',
								onClick: props.onClick,
								onDoubleClick: props.onDoubleClick,
								onContextMenu: props.onContextMenu
								// eslint-disable-next-line @typescript-eslint/no-explicit-any
							}) as any)}
		>
			{loading ? (
				<Spinner size={iconSize ?? 24} />
			) : (
				Icon && <Icon size={iconSize ?? 24} className="shrink-0" />
			)}
			{children && (
				<span className="cursor-pointer [font-size:inherit] text-inherit select-none">
					{children}
				</span>
			)}
		</Component>
	);
};

export default TextButton;
