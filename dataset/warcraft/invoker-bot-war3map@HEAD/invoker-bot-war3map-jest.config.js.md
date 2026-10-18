---
type: vendor-doc
source_repo: "https://github.com/invoker-bot/war3map"
source_path: "jest.config.js"
ref: HEAD
---

module.exports={
    testEnvironment: 'node',
    transform: {
      "^.+\\.ts$": "ts-jest"
    },
    moduleFileExtensions: [
      "ts",
      "js"
     // "json",
     // "node",
    ],
    testRegex: '(/__tests__/.*|(\\.|/)(test|spec))\\.ts$',
    coverageDirectory: 'coverage',
    collectCoverageFrom: [
      'src/**/*.{ts,js}',
      '!src/**/index.ts',
    ],


};