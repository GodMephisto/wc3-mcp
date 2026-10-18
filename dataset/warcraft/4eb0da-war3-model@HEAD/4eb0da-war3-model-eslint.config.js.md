---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "eslint.config.js"
ref: HEAD
---

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';

export default tseslint.config(
  {
    ignores: [
      'dist/',
      'docs/dist/',
      'node_modules/',
      '*.js',
      'third_party/decoder.js',
    ],
  },
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  {
    rules: {
      '@typescript-eslint/no-unused-vars': [
        'warn',
        {
          argsIgnorePattern: '^_',
          varsIgnorePattern: '^_',
          caughtErrorsIgnorePattern: '^_',
        },
      ],
    },
  },
);
