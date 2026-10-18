---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/components/KeyboardListener.tsx"
ref: HEAD
---

import { createContext, type ReactNode, useContext, useState } from 'react';

import useListener from '../utils/useListener.ts';

const KeyboardContext = createContext<Record<string, boolean>>({});

// TODO: Improve performance
export const KeyboardProvider = ({ children }: { children: ReactNode }) => {
	const [keys, setKeys] = useState<Record<string, boolean>>({});

	useListener(
		'keydown',
		e => {
			!keys[e.key] && setKeys({ ...keys, [e.key]: true });
		},
		[keys]
	);
	useListener(
		'keyup',
		e => {
			keys[e.key] && setKeys({ ...keys, [e.key]: false });
		},
		[keys]
	);

	return (
		<KeyboardContext.Provider value={keys}>{children}</KeyboardContext.Provider>
	);
};

export const useKeyPressed = (key: string) => {
	const keys = useContext(KeyboardContext);
	return !!keys[key];
};
