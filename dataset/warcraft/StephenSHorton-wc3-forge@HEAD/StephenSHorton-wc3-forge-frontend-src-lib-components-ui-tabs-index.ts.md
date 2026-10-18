---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/lib/components/ui/tabs/index.ts"
ref: HEAD
---

import Root from "./tabs.svelte";
import Content from "./tabs-content.svelte";
import List, { tabsListVariants, type TabsListVariant } from "./tabs-list.svelte";
import Trigger from "./tabs-trigger.svelte";

export {
	Root,
	Content,
	List,
	Trigger,
	tabsListVariants,
	type TabsListVariant,
	//
	Root as Tabs,
	Content as TabsContent,
	List as TabsList,
	Trigger as TabsTrigger,
};
