---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/InfoPanelItems.tsx"
ref: HEAD
---

import { forwardRef, type ReactElement, type ReactNode, useState } from 'react';
import cls from 'classnames';
import { ChevronDown, ChevronUp } from 'lucide-react';

import Spinner from './Spinner.tsx';
import TextButton from './TextButton.tsx';

const width = 140;

export const InfoPanel = ({
	className,
	children
}: {
	className?: cls.Value;
	children: ReactNode;
}) => (
	<div
		className={cls(
			'tw-surface-2 flex w-[400px] shrink-0 flex-col gap-2 overflow-x-hidden',
			className
		)}
	>
		{children}
	</div>
);

// TODO: Collapsible sections
type SectionProps = {
	title: string;
	loading?: boolean;
	grow?: boolean;
	children: ReactNode;
};

export const InfoSection = ({
	title,
	loading,
	grow,
	children
}: SectionProps) => {
	const [open, setOpen] = useState(true);
	return (
		<>
			<TextButton onClick={() => setOpen(!open)} className="-my-2 *:grow">
				<div className="flex items-center justify-between gap-2">
					<h4 className="tw-color overflow-hidden text-ellipsis whitespace-nowrap">
						{title}
					</h4>
					{open ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
				</div>
			</TextButton>
			{open && (
				<>
					<hr />
					{loading ? (
						<div className={cls('flex items-center justify-center', { grow })}>
							<Spinner />
						</div>
					) : (
						<div
							className={cls(
								'-my-2 flex min-h-0 flex-col gap-2 overflow-x-hidden overflow-y-auto p-2',
								{ grow, 'shrink-0': !grow }
							)}
						>
							{children}
						</div>
					)}
				</>
			)}
		</>
	);
};

export const InfoTitle = ({ children }: { children: ReactNode }) => (
	<p className="l1 text-right" style={{ width }}>
		{children}
	</p>
);

type ItemProps = { title: string; children: ReactElement };
export const InfoItem = ({ title, children }: ItemProps) => (
	<div className="flex items-center gap-2">
		<p
			className="text-blue-gray shrink-0 overflow-hidden text-right break-all"
			style={{ width }}
		>
			{title}:
		</p>
		{children}
	</div>
);

type HoverItemProps = {
	title: string;
	children: ReactElement;
	active?: boolean;
	onClick?: () => void;
	onEnter?: () => void;
	onLeave?: () => void;
};
export const InfoHoverItem = forwardRef<HTMLDivElement, HoverItemProps>(
	({ title, children, active, onClick, onEnter, onLeave }, ref) => (
		<div
			ref={ref}
			className={cls(
				'hocus-within:bg-purple/40 -my-1 flex items-center gap-2 py-1 *:cursor-[inherit] *:overflow-hidden',
				{ 'cursor-pointer': !!onClick, 'bg-purple/60': active }
			)}
			onClick={onClick}
			onMouseEnter={onEnter}
			onMouseLeave={onLeave}
		>
			<p
				className="text-blue-gray shrink-0 overflow-hidden text-right break-all"
				style={{ width }}
			>
				{title}:
			</p>
			{children}
		</div>
	)
);
