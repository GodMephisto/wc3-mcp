---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-vue-sfc"
source_path: "README.md"
ref: HEAD
---

[![Gem Version](https://badge.fury.io/rb/jass-vue-sfc.svg)](http://badge.fury.io/rb/jass-vue-sfc)
[![build](https://github.com/mtgrosser/jass-vue-sfc/actions/workflows/build.yml/badge.svg)](https://github.com/mtgrosser/jass-vue-sfc/actions/workflows/build.yml)

# jass-vue-sfc - Vue 3 Single File Components for Rails

**`jass-vue-sfc` provides [Vue Single File Component](https://v3.vuejs.org/guide/single-file-component.html) support for Sprockets and the Rails asset pipeline. No Webpack required!**
 
Vue SFCs are compiled to ES modules, which can be imported using [Import Maps](https://github.com/rails/importmap-rails) or regular `<script module>` tags.

## Why?

Modern browsers support native loading of ES modules using the `import` statement.
By leveraging Import Maps, modular JS applications can be built without having to integrate
a complicated JS build pipeline.

Framework-specific component formats like the Vue SFC format could not be used this way till now.

### Enter `jass-vue-sfc`!

`jass-vue-sfc` enables the asset pipeline to compile `.vue` files to ES modules,
allowing to build modular Vue applications in a clear and straightforward way,
without the need for invasive external build tools like Webpack.

During development, `jass-vue-sfc` will allow you to have fast reloading without the need
for "hot module replacement". When you change a component, just that component will be recompiled.

A JS dev server is no longer required. JS assets will be delivered by the normal Rails dev server.

## Installation

### Gemfile
```ruby
gem 'jass-vue-sfc'
```

### JS dependencies
Add `@vue/compiler-sfc` to your JS dependencies:
```sh
$ yarn add @vue/compiler-sfc
```

### Node.js

`Jass::Vue::SFC` depends on [Nodo](https://github.com/mtgrosser/nodo), which requires a working Node.js installation.

## Usage

Place your `.vue` components inside your regular `app/javascript` path.

In `app/javascript/components/HelloWorld.vue`:

```vue
<script>
export default {
  data() {
    return {
      greeting: 'Hello World!'
    }
  }
}
</script>

<template>
  <p class="greeting">{{ greeting }}</p>
</template>
```

In your HTML code, load the component as a module:

```erb
<%= javascript_include_tag 'components/HelloWorld.js', module: true %>
```

Note the file extension is `.js`, not `.vue`. Sprockets will take care of
converting your `.vue` file into an ES module.


### Components with imports

If you want to use module `import`s within your components, pin them in your Rails importmap:

```
# config/importmap.rb
pin 'vue'
pin 'components/HelloWorld.vue', to: 'components/HelloWorld.js'
```

Then just use them in your component:

```vue
<script>
import Vue from 'vue';
import HelloWorld from 'components/HelloWorld.vue';
...
</script>
```

## ⚠️ Limitations

As of v1.0.3, the `rails-importmap` gem doesn't support globbing and reloading of JS modules with a file
extension other than `.js`, therefore a modified version is provided at [mtgrosser/importmap-rails](https://github.com/mtgrosser/importmap-rails).

To use the modified version of `importmap-rails`, add it to your `Gemfile`:

```ruby
gem "importmap-rails", ">= 1.0.0", github: "mtgrosser/importmap-rails", branch: "config-accept"
```

There is a [pull request](https://github.com/rails/importmap-rails/pull/57) which will resolve this issue when accepted.

Also, the following things are not (yet) supported:

- extracting the `<style>` section of the SFC
- source maps

## Other Jass gems for the asset pipeline

💎 [Jass::Rollup](https://github.com/mtgrosser/jass-rollup) – Rollup.js support for Sprockets

💎 [Jass::Esbuild](https://github.com/mtgrosser/jass-esbuild) – esbuild support for Sprockets

💎 [Jass::React::JSX](https://github.com/mtgrosser/jass-react-jsx) – React JSX support for Sprockets
