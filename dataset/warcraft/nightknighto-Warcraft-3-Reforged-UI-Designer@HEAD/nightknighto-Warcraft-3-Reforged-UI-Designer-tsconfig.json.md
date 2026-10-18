---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "tsconfig.json"
ref: HEAD
---

{
    "compilerOptions": {
        "module": "CommonJS",
        "strict": false,
        "moduleResolution": "node",
        "sourceMap": true,
        "emitDecoratorMetadata": true,
        "experimentalDecorators": true,
        "removeComments": false,
        "noImplicitAny": false,
        "suppressImplicitAnyIndexErrors": true,
        "outDir": "app",
        "downlevelIteration": true,
        "target": "es6",
        "plugins": [
            {
                "name": "typescript-lit-html-plugin"
            }
        ]
    },
    "exclude": ["node_modules"]
}
