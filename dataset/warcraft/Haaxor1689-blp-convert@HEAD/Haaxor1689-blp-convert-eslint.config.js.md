---
type: vendor-doc
source_repo: "https://github.com/Haaxor1689/blp-convert"
source_path: "eslint.config.js"
ref: HEAD
---

import js from '@eslint/js';
import eslintConfigPrettier from 'eslint-config-prettier';
import importPlugin from 'eslint-plugin-import';
import preferArrowPlugin from 'eslint-plugin-prefer-arrow';
import pluginReact from 'eslint-plugin-react';
import pluginReactHooks from 'eslint-plugin-react-hooks';
import simpleImportSort from 'eslint-plugin-simple-import-sort';
import globals from 'globals';
import tseslint from 'typescript-eslint';

/**
 * A custom ESLint configuration for libraries that use React.
 *
 * @type {import("eslint").Linter.Config[]} */
const config = [
	js.configs.recommended,
	eslintConfigPrettier,
	pluginReact.configs.flat.recommended,
	...tseslint.configs.recommended,
	{
		languageOptions: {
			...pluginReact.configs.flat.recommended.languageOptions,
			parserOptions: {
				projectService: true,
				tsconfigRootDir: import.meta.dirname
			},
			globals: { ...globals.serviceworker, ...globals.browser }
		}
	},
	{
		plugins: {
			'prefer-arrow': preferArrowPlugin,
			'import': importPlugin,
			'simple-import-sort': simpleImportSort,
			'react-hooks': pluginReactHooks
		},
		settings: { react: { version: 'detect' } },
		rules: {
			// General
			'no-template-curly-in-string': ['error'],
			'no-mixed-spaces-and-tabs': ['error', 'smart-tabs'],
			'no-var': 'error',
			'no-useless-rename': 'error',
			'object-shorthand': ['error', 'always'],
			'comma-dangle': ['error', 'never'],
			'arrow-body-style': ['error', 'as-needed'],
			'eqeqeq': ['error', 'always'],
			'dot-notation': 'error',
			'prefer-arrow-callback': 'error',
			'prefer-const': 'error',
			'prefer-template': 'error',
			'prefer-arrow/prefer-arrow-functions': 'error',
			// Typescript
			'@typescript-eslint/no-explicit-any': 'warn',
			'@typescript-eslint/consistent-type-definitions': ['error', 'type'],
			'@typescript-eslint/prefer-optional-chain': 'error',
			'@typescript-eslint/prefer-nullish-coalescing': 'error',
			'@typescript-eslint/explicit-module-boundary-types': 'off',
			'@typescript-eslint/no-unused-vars': [
				'warn',
				{
					args: 'all',
					argsIgnorePattern: '^_',
					varsIgnorePattern: '^_',
					caughtErrorsIgnorePattern: '^_'
				}
			],
			'@typescript-eslint/consistent-type-imports': [
				'warn',
				{ prefer: 'type-imports', fixStyle: 'inline-type-imports' }
			],
			'@typescript-eslint/no-unused-expressions': [
				'error',
				{ allowShortCircuit: true, allowTernary: true }
			],
			// Import
			'import/first': 'error',
			'import/newline-after-import': 'error',
			'import/no-duplicates': 'error',
			'simple-import-sort/exports': 'error',
			'simple-import-sort/imports': [
				'error',
				{
					groups: [
						// Side effect imports.
						['^\\u0000'],
						// Node.js builtins.
						['^node:'],
						// Packages.
						['^react', '^next', '^@?\\w'],
						// Internal packages.
						['^(@turtle)(/.*|$)'],
						// Absolute imports and other imports such as Vue-style `@/foo`.
						['^#'],
						// Relative imports.
						['^\\.'],
						// Style imports.
						['^.+\\.css$']
					]
				}
			],
			...pluginReactHooks.configs.recommended.rules,
			// React scope no longer necessary with new JSX transform.
			'react/react-in-jsx-scope': 'off',
			'react/function-component-definition': [
				'error',
				{
					namedComponents: 'arrow-function',
					unnamedComponents: 'arrow-function'
				}
			],
			// 'react/react-in-jsx-scope': 'off',
			'react/self-closing-comp': 'error',
			'react/jsx-boolean-value': ['error', 'never'],
			'react/jsx-curly-brace-presence': ['error', 'never'],
			'react/jsx-curly-spacing': ['error', 'never'],
			'react/jsx-equals-spacing': ['error', 'never'],
			'react/jsx-fragments': ['error', 'syntax'],
			'react/jsx-no-useless-fragment': 'error',
			'react/display-name': 'off'
		}
	},
	{ ignores: ['dist/**'] }
];

export default config;
