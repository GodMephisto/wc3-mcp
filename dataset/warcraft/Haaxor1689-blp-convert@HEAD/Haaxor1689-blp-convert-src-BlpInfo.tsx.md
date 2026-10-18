---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/BlpInfo.tsx"
ref: HEAD
---

import { type Blp } from 'haax-blp';
import { Eye, EyeOff } from 'lucide-react';

import { toCompression } from './BlpForm.tsx';
import {
	InfoItem,
	InfoPanel,
	InfoSection,
	InfoTitle
} from './components/InfoPanelItems.tsx';
import TextButton from './components/TextButton.tsx';

type Props = {
	index: number;
	setIndex: (index: number) => void;
	info: Blp;
};

const BlpInfo = ({ info, index, setIndex }: Props) => (
	<div className="relative z-10 -mb-3">
		<InfoPanel className="absolute top-2 right-2">
			<InfoSection title="Details" loading={!info} grow>
				<InfoItem title="Dimensions">
					<p>
						{info?.width} x {info?.height}
					</p>
				</InfoItem>
				<InfoItem title="Compression">
					<p>{toCompression(info) ?? 'Unknown'}</p>
				</InfoItem>
				<InfoItem title="Alpha size">
					<p>{info.alphaSize} bits</p>
				</InfoItem>
				{info.mips.length <= 1 ? (
					<InfoItem title="Mipmaps">
						<p>None</p>
					</InfoItem>
				) : (
					<>
						<InfoTitle>Mipmaps</InfoTitle>
						{info.mips.map((mip, i) => (
							<InfoItem key={i} title={`Mip ${i}`}>
								<p>
									{mip.width}x{mip.height}
									<TextButton
										title="View"
										icon={index === i ? Eye : EyeOff}
										iconSize={16}
										active={index === i}
										onClick={() => setIndex(i)}
										className="-my-2 inline-flex"
									/>
								</p>
							</InfoItem>
						))}
					</>
				)}
			</InfoSection>
		</InfoPanel>
	</div>
);
export default BlpInfo;
