---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "grammar.js"
ref: HEAD
---

"use strict";

/**
 * @file Parser for the JASS2 language
 * @author Leonardo Julca <ivojulca@gmail.com>
 * @license MIT
 */

/// <reference types="tree-sitter-cli/dsl" />
// @ts-check

const {root, baseDefinitions} = require('./grammar-defs');

const rules = {
	program: root,
	...baseDefinitions.rules,
};

const definitions = {
	...baseDefinitions,
	rules,
};

module.exports = grammar(definitions);
