---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/Dialog.tsx"
ref: HEAD
---

import { type FormEvent, type ReactNode } from 'react';
import {
	Close,
	Content,
	DialogDescription,
	Overlay,
	Portal,
	Root,
	Title,
	Trigger
} from '@radix-ui/react-dialog';
import { VisuallyHidden } from '@radix-ui/react-visually-hidden';
import cls from 'classnames';
import { X } from 'lucide-react';

import TextButton from './TextButton.tsx';

export const DialogClose = ({ children }: { children: ReactNode }) => (
	<Close asChild>{children}</Close>
);

const FormComponent = ({
	onSubmit,
	children
}: Pick<Props, 'onSubmit' | 'children'>) =>
	!onSubmit ? (
		children
	) : (
		<form onSubmit={onSubmit} className="contents">
			{children}
		</form>
	);

type Props = (
	| { type: 'nonModal' }
	| {
			type: 'controlled';
			open: boolean;
			onOpenChange: (open: boolean) => void;
	  }
	| { type?: never; open?: never; onOpenChange?: never }
) & {
	onSubmit?: (e: FormEvent<HTMLFormElement>) => void;
	title?: string | ReactNode;
	description?: string | ReactNode;
	children?: ReactNode;
	trigger?: ReactNode;
	actions?: ReactNode[];
	noScroll?: boolean;
	className?: cls.Value;
	actionsClassName?: cls.Value;
};

const Dialog = ({
	onSubmit,
	title,
	description,
	children,
	trigger,
	actions,
	noScroll,
	className,
	actionsClassName,
	...props
}: Props) => (
	<Root {...(props.type === 'nonModal' ? { open: true, modal: false } : props)}>
		{trigger && <Trigger asChild>{trigger}</Trigger>}
		<Portal>
			<Overlay className="bg-dark-gray/80 [@supports(backdrop-filter:blur(0))]:bg-dark-gray/50 fixed inset-0 z-50 [@supports(backdrop-filter:blur(0))]:backdrop-blur" />
			<Content
				className={cls(
					'tw-surface-3 fixed top-1/2 left-1/2 z-50 max-h-[calc(100vh-150px)] max-w-3xl -translate-x-1/2 -translate-y-1/2 transform',
					className
				)}
			>
				<VisuallyHidden asChild>
					<Title>{typeof title === 'string' ? title : 'Dialog'}</Title>
				</VisuallyHidden>

				{description && (
					<VisuallyHidden asChild>
						<DialogDescription>{description}</DialogDescription>
					</VisuallyHidden>
				)}

				{title && (
					<>
						{typeof title !== 'string' ? (
							title
						) : (
							<h3 className="tw-color">{title}</h3>
						)}
						<hr />
					</>
				)}

				<FormComponent onSubmit={onSubmit}>
					{children &&
						(noScroll ? (
							children
						) : (
							<div className="-m-3 overflow-auto p-3">{children}</div>
						))}

					{!!actions?.filter(a => a !== null)?.length && (
						<>
							<hr />
							<div
								className={cls(
									'-my-2 flex items-center justify-end',
									actionsClassName
								)}
							>
								{actions}
							</div>
						</>
					)}
				</FormComponent>

				{props.type !== 'nonModal' && (
					<Close asChild>
						<TextButton
							type="submit"
							icon={X}
							title="Close"
							aria-label="Close"
							className="hocus:text-red absolute top-0 right-0"
						/>
					</Close>
				)}
			</Content>
		</Portal>
	</Root>
);

export default Dialog;
