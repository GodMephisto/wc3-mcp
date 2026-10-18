---
type: vendor-doc
source_repo: "https://github.com/IceWolf0110/warsmash-vue"
source_path: "src/stores/menu.ts"
ref: HEAD
---

import { defineStore } from 'pinia'

export const useMenuStore = defineStore('menu', () => {
  const menus = [
    {
      title: 'Home',
      url: '/',
    },
    {
      title: 'Forum',
      url: 'https://www.hiveworkshop.com/forums/warsmash-mod-engine.780',
    },
    {
      title: 'Source',
      url: 'https://github.com/Retera/WarsmashModEngine',
    },
    {
      title: 'License',
      url: 'https://github.com/Retera/WarsmashModEngine/blob/main/LICENSE',
    },
    {
      title: 'Making of',
      url: '/tutorial',
    },
    {
      title: 'Compatibility',
      url: '/compatibility',
    },
    {
      title: 'Ramblings',
      url: '/ramblings',
    },
  ]

  const isExternal = (url: string) => /^https?:\/\//.test(url)

  return { menus, isExternal }
})
