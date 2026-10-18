---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "tsconfig.json"
ref: HEAD
---

{
    "compilerOptions": {
		"allowSyntheticDefaultImports": true,
		"forceConsistentCasingInFileNames": true,
		"isolatedModules": true,
        "module": "esnext",
        // "allowJs": true,
        "resolveJsonModule": true,
        "esModuleInterop": true,
        "moduleResolution": "bundler",
		"noEmitOnError": true,
		"noUnusedLocals": true,
		"noUnusedParameters": true,
        "pretty": true,
        "noEmit": true,
        "lib": ["DOM"],
        "types": ["@webgpu/types"],
        "skipLibCheck": true,
        "strict": false
    },
    "exclude": [
        "node_modules",
        "dist"
    ]
}
