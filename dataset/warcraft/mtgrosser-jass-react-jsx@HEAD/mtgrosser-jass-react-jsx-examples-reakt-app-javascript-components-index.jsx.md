---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "examples/reakt/app/javascript/components/index.jsx"
ref: HEAD
---

import { render } from "react-dom";
import Clock from "components/Clock.jsx";

render(
  <Clock />,
  document.getElementById('root')
);
