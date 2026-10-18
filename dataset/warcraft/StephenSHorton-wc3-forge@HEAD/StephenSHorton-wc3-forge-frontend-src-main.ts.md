---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/main.ts"
ref: HEAD
---

import './style.css'
import './debuglog'
import App from './App.svelte'
import { mount } from "svelte";

const app = mount(App, {
  target: document.getElementById('app')
})

export default app
