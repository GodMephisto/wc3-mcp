---
type: vendor-doc
source_repo: "https://github.com/TrayHard/casc-modern"
source_path: "src/main.tsx"
ref: HEAD
---

import React from "react";
import ReactDOM from "react-dom/client";
import { ConfigProvider, theme } from "antd";
import App from "./App";
import "./index.css";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ConfigProvider theme={{ algorithm: theme.darkAlgorithm }}>
      <App />
    </ConfigProvider>
  </React.StrictMode>
);
