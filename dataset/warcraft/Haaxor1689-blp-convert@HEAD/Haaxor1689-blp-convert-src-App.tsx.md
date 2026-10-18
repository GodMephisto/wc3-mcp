---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "src/App.tsx"
ref: HEAD
---

import { Analytics } from '@vercel/analytics/react';

import FileEditorBlp from './Blp.tsx';
import Footer from './Footer.tsx';

const App = () => (
	<div className="bg-dark-gray flex min-h-dvh flex-col items-stretch overflow-auto overflow-x-hidden">
		<main className="flex max-w-7xl grow flex-col gap-3 p-2 pt-0 md:px-6 xl:mx-auto xl:w-full">
			<FileEditorBlp />
		</main>
		<Footer />
		<Analytics />
	</div>
);

export default App;
