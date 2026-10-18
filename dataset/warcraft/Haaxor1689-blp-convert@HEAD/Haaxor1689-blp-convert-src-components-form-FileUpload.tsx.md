---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/form/FileUpload.tsx"
ref: HEAD
---

import { useRef, useState } from 'react';
import cls from 'classnames';
import { ImageUp } from 'lucide-react';

type FileUploadProps = {
	onChange: (file: File | null) => void;
	accept?: string;
};

const FileUpload = ({ onChange, accept }: FileUploadProps) => {
	const [isDragging, setIsDragging] = useState(false);
	const inputRef = useRef<HTMLInputElement>(null);

	const handleDragEnter = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
		setIsDragging(true);
	};

	const handleDragLeave = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
		setIsDragging(false);
	};

	const handleDragOver = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
	};

	const validateFileType = (file: File): boolean => {
		if (!accept) return true;

		const acceptedTypes = accept.split(',').map(type => type.trim());

		return acceptedTypes.some(type => {
			if (type.startsWith('.')) {
				return file.name.toLowerCase().endsWith(type.toLowerCase());
			}
			if (type.endsWith('/*')) {
				const category = type.split('/')[0];
				return file.type.startsWith(`${category}/`);
			}
			return file.type === type;
		});
	};

	const handleDrop = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
		setIsDragging(false);

		const files = e.dataTransfer.files;
		if (files && files.length > 0) {
			const file = files[0];
			if (file && validateFileType(file)) {
				onChange(file);
			}
		}
	};

	const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const files = e.target.files;
		if (files && files.length > 0) {
			const file = files[0];
			if (file) {
				onChange(file);
			}
		}
	};

	const handleClick = () => {
		inputRef.current?.click();
	};

	return (
		<div
			className={cls(
				`hocus:tw-highlight hocus:border-green hocus:bg-dark-gray/5 bg-dark-gray/70 border-blue-gray/50 relative flex h-full max-h-[500px] w-full max-w-[500px] grow cursor-pointer items-center justify-center border-2 border-dashed p-6`,
				isDragging && 'border-green bg-dark-gray/5'
			)}
			onDragEnter={handleDragEnter}
			onDragLeave={handleDragLeave}
			onDragOver={handleDragOver}
			onDrop={handleDrop}
			onClick={handleClick}
		>
			<input
				ref={inputRef}
				type="file"
				accept={accept}
				onChange={handleFileChange}
				className="hidden"
			/>

			<div className="pointer-events-none flex flex-col items-center gap-2 text-center text-inherit">
				<ImageUp size={48} className="" />
				<div className="text-inherit">Click to upload or drag and drop</div>
				{accept && (
					<div className="text-sm text-inherit">Accepted: {accept}</div>
				)}
			</div>
		</div>
	);
};

export default FileUpload;
