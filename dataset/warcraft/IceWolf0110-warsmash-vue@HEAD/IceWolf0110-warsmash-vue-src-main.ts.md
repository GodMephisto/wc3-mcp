---
type: vendor-doc
source_repo: "https://github.com/IceWolf0110/warsmash-vue"
source_path: "src/main.ts"
ref: HEAD
---

import '@/assets/css/app.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')
