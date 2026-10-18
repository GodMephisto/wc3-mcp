---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/utils/useListener.ts"
ref: HEAD
---

import { type DependencyList, useEffect } from 'react';

const useListener = <K extends keyof HTMLElementEventMap>(
	type: K,
	listener: (this: HTMLDialogElement, ev: HTMLElementEventMap[K]) => any,
	dependencies: DependencyList = [],
	target?: HTMLElement | null
) =>
	useEffect(() => {
		const r = target ?? window;
		r?.addEventListener(type, listener as never);
		return () => r?.removeEventListener(type, listener as never);
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, dependencies);

export default useListener;
