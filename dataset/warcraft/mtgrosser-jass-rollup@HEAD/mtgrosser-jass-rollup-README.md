---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-rollup"
source_path: "README.md"
ref: HEAD
---

[![Gem Version](https://badge.fury.io/rb/jass-rollup.svg)](http://badge.fury.io/rb/nodo)  [![build](https://github.com/mtgrosser/jass-rollup/actions/workflows/build.yml/badge.svg)](https://github.com/mtgrosser/jass-rollup/actions/workflows/build.yml)

# Jass::Rollup – Rollup for Sprockets and the Rails asset pipeline

`Jass::Rollup` integrates the [Rollup](https://rollupjs.org) JS bundler with Sprockets and the Rails asset pipeline.

## 💡 Motivation

JavaScript build pipelines tend to be a complicated mess consisting of dev servers,
an infinite number of `npm` dependencies and other "opinionated" conventions (and lack thereof).

The `Jass` gem series provide a straightforward way of integrating modern JS tooling
with the existing Rails asset pipeline, while adhering to established workflows
for asset processing in Rails applications.

## 📦 Installation

### Gemfile

```ruby
gem 'jass-rollup'
```

### JS dependencies

Add `rollup` to your JS dependencies:

```sh
$ yarn add rollup @rollup/plugin-commonjs @rollup/plugin-node-resolve
```

### Node.js

`Jass::Rollup` depends on [Nodo](https://github.com/mtgrosser/nodo), which requires a working Node.js installation.

## ⚡️ Usage

`Jass::Rollup` provides two new directives to use in your JS assets, as well as
a new Sprockets file extension for rollup bundle config files.

### rollup

The `rollup` directive will invoke `Rollup` with the given entry point
relative to your `Rails.root`.

To bundle an entry point as ES module:

```js
//= rollup app/javascript/entry.js
```

To bundle a `npm` module as ES module:

```js
//= rollup vendor/node_modules/rxjs/dist/esm5/index.js
```

### rollup_esm

The `require_esm` directive will bundle a `npm` module referenced by its
name as ES module:

```js
//= rollup_esm currency.js
```

### Bundling with custom rollup configurations

If your Rollup bundle requires extra configuration options, e.g. plugins, the file
extension `.rollup` can be used:

```js
// assets/javascript/main.js.rollup
export default {
  input: 'app/javascript/entry.js',
  output: {
    format: 'es'
  }
};
```

This asset will be bundled to `main.js`.


## 💎 Other Jass gems for the asset pipeline

[Jass::Esbuild](https://github.com/mtgrosser/jass-esbuild) – esbuild support for Sprockets

[Jass::Vue::SFC](https://github.com/mtgrosser/jass-vue-sfc) – Vue Single File Component support for Sprockets

[Jass::React::JSX](https://github.com/mtgrosser/jass-react-jsx) – React JSX support for Sprockets
