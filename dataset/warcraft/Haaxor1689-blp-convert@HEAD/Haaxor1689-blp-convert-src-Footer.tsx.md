---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/Footer.tsx"
ref: HEAD
---

import { Bug } from 'lucide-react';

import TextButton from './components/TextButton.tsx';

const Footer = () => (
	<footer className="text-blue-gray flex max-w-7xl flex-wrap items-center justify-center gap-3 p-2 md:px-6 md:py-2 xl:mx-auto xl:w-full">
		<p className="text-blue-gray text-sm">Created by Haaxor1689</p>•
		<TextButton
			type="link"
			icon={() => (
				<img
					src="https://storage.ko-fi.com/cdn/logomarkLogo.png"
					alt="Ko-fi logo"
					className="h-4 shrink-0 pr-1"
				/>
			)}
			href="https://ko-fi.com/haaxor1689"
			external
			className="-m-2 *:font-bold"
		>
			Support me
		</TextButton>
		•
		<TextButton
			type="link"
			href="https://discord.gg/pDeTHQH99B"
			icon={Bug}
			iconSize={18}
			external
			className="text-orange -m-2 shrink-0 text-sm"
		>
			Report a bug
		</TextButton>
		•
		<TextButton
			type="link"
			href="https://haaxor1689.dev/privacy-policy"
			className="text-blue-gray -m-2 shrink-0 text-sm"
		>
			Privacy Policy
		</TextButton>
	</footer>
);

export default Footer;
