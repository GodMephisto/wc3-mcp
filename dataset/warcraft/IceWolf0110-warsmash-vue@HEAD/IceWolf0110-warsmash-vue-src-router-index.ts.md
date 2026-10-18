---
type: vendor-doc
source_repo: "https://github.com/IceWolf0110/warsmash-vue"
source_path: "src/router/index.ts"
ref: HEAD
---

import { createRouter, createWebHistory } from 'vue-router'
import routes from '@/router/routes.ts'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes,
})

router.afterEach((to) => {
  document.title = (to.meta?.title as string) ?? 'Not Found'
})

export default router
