---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "examples/reakt/README.md"
ref: HEAD
---

# Example Rails application with JSX & Import Maps

## Setup

Add the following gems to your `Gemfile`:

```ruby
gem "jass-react-jsx"
gem "importmap-rails", ">= 0.3.4", github: "mtgrosser/importmap-rails", branch: "main"
```

Install the JS dependencies:

```sh
$ yarn add @babel/core @babel/plugin-transform-react-jsx
```

Enable JSX support for import maps by adding the following line inside the config block of
`config/application.rb`:

```ruby
config.importmap.accept = %w[js jsx]
```

Make sure that `app/javascript` is linked by `app/assets/config/manifest.js`:

```js
//= link_tree ../../javascript .js
```

## Define the import map

```ruby
# config/importmap.rb
pin "application", preload: true

pin "react", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/react.js"
pin "react-dom", to: "https://cdn.esm.sh/v45/react-dom@17.0.2/es2021/react-dom.js"
pin "react/jsx-runtime", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/jsx-runtime.js"

pin_all_from "app/javascript/components", under: "components"
```

## Create your JSX components and scripts

```jsx
// app/javascript/components/Clock.jsx

import { Component } from "react";

export default class Clock extends Component {
  // see example file
}
```

If you want to use JSX syntax, your file needs the `.jsx` extension. It will be
made available as `.js` by the Rails asset pipeline.

```jsx
// app/javascript/components/index.jsx

import { render } from "react-dom";
import Clock from "components/Clock.jsx";

render(
  <Clock />,
  document.getElementById('root')
);
```

Import the JSX modules in `app/javascript/application.js`:

```js
import "components";
```

## Load the importmap in your HTML

In the `<head>` of `app/views/layouts/application.html.erb`:

```erb
<%= stylesheet_link_tag "application", "data-turbo-track": "reload" %>
<%= javascript_importmap_tags %>
```
