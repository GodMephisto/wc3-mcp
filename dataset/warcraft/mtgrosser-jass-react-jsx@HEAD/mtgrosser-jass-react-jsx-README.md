---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "README.md"
ref: HEAD
---

[![Gem Version](https://badge.fury.io/rb/jass-react-jsx.svg)](http://badge.fury.io/rb/jass-react-jsx)
[![build](https://github.com/mtgrosser/jass-react-jsx/actions/workflows/build.yml/badge.svg)](https://github.com/mtgrosser/jass-react-jsx/actions/workflows/build.yml)

# Jass::React::JSX – React JSX support for the Rails asset pipeline

`Jass::React::JSX` enables Sprockets and the Rails asset pipeline to compile [React JSX components](https://reactjs.org/docs/introducing-jsx.html).

JSX components will be compiled to ES modules, which can be imported using the new Rails [Import Maps](https://github.com/rails/importmap-rails) or
regular `<script module>` tags.

## Motivation

Modern browsers support native loading of ES modules using the `import` statement.
By leveraging the new Rails Import Maps, modular JS applications can be built
without having to integrate a complex and tedious JS build pipeline. 

However, framework-specific component formats like the React JSX format could not
be used this way till now.

`Jass::React::JSX` enables the asset pipeline to compile `.jsx` files to ES modules,
allowing to build modular React applications in a clear and straightforward way,
without the necessity of external build tools.

## Installation

### Gemfile

```ruby
gem 'jass-react-jsx'
```

### JS dependencies

Add the Babel transformers to your JS dependencies:
```sh
$ yarn add @babel/core @babel/plugin-transform-react-jsx
```

### Node.js

`Jass::React::JSX` depends on [Nodo](https://github.com/mtgrosser/nodo), which requires a working Node.js installation.

## Usage

Place your `.jsx` components inside your regular `app/javascript` path.

In `app/javascript/components/HelloWorld.jsx`:

```jsx
import { Component } from "react";

export default class HelloWorld extends Component {
  render() {
    return (
      <div>
        <h1>Hello, world!</h1>
        <h2>The current UNIX date is {Date.now()}.</h2>
      </div>
    );
  }
}
```

In your HTML code, load the component as a module:

```erb
<%= javascript_include_tag 'components/HelloWorld.js', module: true %>
```

### Components with imports

In order to use module `import`s within your components, you need to pin them in your Rails importmap:

```ruby
# config/importmap.rb
pin "react", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/react.js"
pin "react-dom", to: "https://cdn.esm.sh/v45/react-dom@17.0.2/es2021/react-dom.js"
pin "react/jsx-runtime", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/jsx-runtime.js"

pin "components/HelloWorld.jsx", to: "components/HelloWorld.js"
```

Then just use them in your JS code:

```js
import { render } from "react-dom";
import HelloWorld from "components/HelloWorld.jsx";
```

## Example Rails app

For an example Rails application check out the [example/reakt](https://github.com/mtgrosser/jass-react-jsx/tree/master/examples/reakt) folder in this repo.

## ⚠️ Limitations

As of v0.8.1, the `rails-importmap` gem doesn't support globbing and reloading of JS modules with a file
extension other than `.js`, therefore a modified version is provided at [mtgrosser/importmap-rails](https://github.com/mtgrosser/importmap-rails).

To use the modified version of `importmap-rails`, add it to your `Gemfile`:

```ruby
gem "importmap-rails", ">= 0.3.4", github: "mtgrosser/importmap-rails", branch: "main"
```

There is a [pull request](https://github.com/rails/importmap-rails/pull/57) which will resolve this issue when accepted.

Also, the following things are not (yet) supported:

- source maps
